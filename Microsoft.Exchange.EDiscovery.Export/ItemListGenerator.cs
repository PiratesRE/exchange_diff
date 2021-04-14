using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class ItemListGenerator : IBatchDataReader<ItemListGenerator.ItemIdsDataBatch>, IBatchDataWriter<ItemListGenerator.ItemIdsDataBatch>, IDisposable
	{
		public ItemListGenerator(SourceInformationCollection allSourceInformation, ITarget target, IProgressController progressController)
		{
			this.AllSourceInformation = allSourceInformation;
			this.target = target;
			this.includeDuplicates = target.ExportContext.ExportMetadata.IncludeDuplicates;
			this.progressController = progressController;
		}

		public event EventHandler<DataBatchEventArgs<ItemListGenerator.ItemIdsDataBatch>> DataBatchRead;

		public event EventHandler AbortingOnError;

		public bool DoUnSearchable
		{
			get
			{
				return this.doUnSearchable;
			}
			set
			{
				this.doUnSearchable = value;
			}
		}

		public DataRetriever DataRetriever { get; set; }

		public SourceInformationCollection AllSourceInformation { get; set; }

		public Task WritingTask
		{
			get
			{
				return this.writingTask;
			}
			set
			{
				this.writingTask = value;
			}
		}

		public void StartReading()
		{
			this.GenerateItemList();
		}

		public void WriteDataBatchLegacy(ItemListGenerator.ItemIdsDataBatch dataBatch)
		{
			Tracer.TraceInformation("ItemListGenerator: Legacy Writing item ID batch...", new object[0]);
			this.SaveItemIdListToCache(dataBatch);
			Tracer.TraceInformation("ItemListGenerator: Legacy Item ID batch written...", new object[0]);
		}

		public void WriteDataBatch(ItemListGenerator.ItemIdsDataBatch dataBatch)
		{
			Tracer.TraceInformation("ItemListGenerator: Writing item ID batch...", new object[0]);
			this.FlushItemIds(dataBatch);
			Tracer.TraceInformation("ItemListGenerator: Item ID batch written...", new object[0]);
		}

		public void Close()
		{
			Tracer.TraceInformation("ItemListGenerator: Closing ItemListGenerator...", new object[0]);
			if (this.itemListCache != null)
			{
				this.FlushItemListCache(this.itemListCache.Values);
			}
			if (this.unsearchableItemListCache != null)
			{
				this.FlushItemListCache(this.unsearchableItemListCache.Values);
			}
			foreach (string sourceId in this.failedMailboxes)
			{
				this.ResetStatusForFailedMailbox(sourceId);
			}
			Tracer.TraceInformation("ItemListGenerator: ItemListGenerator closed...", new object[0]);
		}

		public void InitItemList()
		{
			Tracer.TraceInformation("ItemListGenerator.InitItemList: Entering method...", new object[0]);
			this.itemListCache = new Dictionary<string, IItemIdList>();
			if (this.target.ExportContext.ExportMetadata.IncludeUnsearchableItems)
			{
				this.unsearchableItemListCache = new Dictionary<string, IItemIdList>();
			}
			this.failedMailboxes = new HashSet<string>();
			if (!this.target.ExportContext.ExportMetadata.IncludeDuplicates)
			{
				this.uniqueItemIds = new Dictionary<object, BasicItemId>();
				if (this.target.ExportContext.IsResume)
				{
					Tracer.TraceInformation("ItemListGenerator.InitItemList: Reconstructing unique hash map...", new object[0]);
					this.RecreateUniqueItemHashMap();
				}
			}
		}

		public void WriteDataBatchItemListGen(object sender, DataBatchEventArgs<ItemListGenerator.ItemIdsDataBatch> args)
		{
			ScenarioData scenarioData = ScenarioData.Current;
			ExportException ex = AsynchronousTaskHandler.WaitForAsynchronousTask(this.writingTask);
			this.writingTask = ((ex != null || args.DataBatch == null) ? null : Task.Factory.StartNew(delegate(object dataBatch)
			{
				ScenarioData scenarioData;
				using (new ScenarioData(scenarioData))
				{
					this.WriteDataBatch((ItemListGenerator.ItemIdsDataBatch)dataBatch);
				}
			}, args.DataBatch));
			if (ex != null)
			{
				throw ex;
			}
		}

		public void WriteDataBatchDataRetriever(object sender, DataBatchEventArgs<List<ItemInformation>> args)
		{
			if (this.DataRetriever == null || this.DataRetriever.DataWriter == null)
			{
				return;
			}
			ScenarioData scenarioData = ScenarioData.Current;
			ExportException ex = AsynchronousTaskHandler.WaitForAsynchronousTask(this.writingTask);
			this.writingTask = ((ex != null || args.DataBatch == null) ? null : Task.Factory.StartNew(delegate(object dataBatch)
			{
				ScenarioData scenarioData;
				using (new ScenarioData(scenarioData))
				{
					this.DataRetriever.DataWriter.WriteDataBatch((List<ItemInformation>)dataBatch);
				}
			}, args.DataBatch));
			if (ex != null)
			{
				throw ex;
			}
		}

		public void DoExportForSourceMailbox(SourceInformation sourceMailbox)
		{
			Tracer.TraceInformation("ItemListGenerator.DoExportForSourceMailbox: Generating item list for mailbox '{0}'", new object[]
			{
				sourceMailbox.Configuration.Id
			});
			ErrorRecord errorRecord = null;
			sourceMailbox.Configuration.Id.StartsWith("\\");
			ScenarioData.Current["PM"] = sourceMailbox.Configuration.LegacyExchangeDN.GetHashCode().ToString();
			if (sourceMailbox.Status.ItemCount < 0)
			{
				sourceMailbox.Status.ItemCount = 0;
			}
			if (sourceMailbox.Status.UnsearchableItemCount < 0)
			{
				sourceMailbox.Status.UnsearchableItemCount = 0;
			}
			try
			{
				bool flag = this.target.ExportContext.ExportMetadata.IncludeSearchableItems && !this.DoUnSearchable;
				bool flag2 = this.target.ExportContext.ExportMetadata.IncludeUnsearchableItems && this.DoUnSearchable;
				bool flag3 = false;
				Dictionary<string, string> dictionary = null;
				dictionary = ItemListGenerator.PrepareAllFolders(sourceMailbox.ServiceClient, sourceMailbox.Configuration.Id);
				if (flag)
				{
					bool errorHappened = false;
					try
					{
						bool flag4 = false;
						Tracer.TraceInformation("ItemListGenerator.GenerateItemListForSourceMailbox: call GetAllItemIdsFromServer for primary for mailbox: '{0}'", new object[]
						{
							sourceMailbox.Configuration.Id
						});
						this.GetAllItemIdsFromServer<ItemId>(sourceMailbox, new ItemListGenerator.GetItemIdPageFromServerDelegate<ItemId>(this.GetItemIdPageFromServer), dictionary, !flag, false, ref flag4, out flag3);
						if (dictionary != null && dictionary.Values.Contains("\\archive"))
						{
							Tracer.TraceInformation("ItemListGenerator.GenerateItemListForSourceMailbox: call GetAllItemIdsFromServer for archive for mailbox: '{0}'", new object[]
							{
								sourceMailbox.Configuration.Id
							});
							this.GetAllItemIdsFromServer<ItemId>(sourceMailbox, new ItemListGenerator.GetItemIdPageFromServerDelegate<ItemId>(this.GetItemIdPageFromServer), dictionary, !flag, true, ref flag4, out flag3);
						}
						ExportException ex = AsynchronousTaskHandler.WaitForAsynchronousTask(this.writingTask);
						if (ex != null)
						{
							errorHappened = true;
							throw ex;
						}
					}
					finally
					{
						this.DataRetriever.DataWriter.ExitDataContext(errorHappened);
					}
				}
				bool isUnsearchable = true;
				if (flag2)
				{
					bool errorHappened2 = false;
					try
					{
						bool flag5 = false;
						Tracer.TraceInformation("ItemListGenerator.GenerateItemListForSourceMailbox: call GetAllItemIdsFromServer isunsearchable for primary for mailbox: '{0}'", new object[]
						{
							sourceMailbox.Configuration.Id
						});
						this.GetAllItemIdsFromServer<ItemId>(sourceMailbox, new ItemListGenerator.GetItemIdPageFromServerDelegate<ItemId>(this.GetItemIdPageFromServer), dictionary, isUnsearchable, false, ref flag5, out flag3);
						if (!flag3)
						{
							Tracer.TraceInformation("ItemListGenerator.GenerateItemListForSourceMailbox: call GetAllItemIdsFromServer isunsearchable for newschemafailed for mailbox: '{0}'", new object[]
							{
								sourceMailbox.Configuration.Id
							});
							this.GetAllItemIdsFromServer<UnsearchableItemId>(sourceMailbox, new ItemListGenerator.GetItemIdPageFromServerDelegate<UnsearchableItemId>(this.GetUnsearchableItemIdPageFromServer), dictionary, isUnsearchable, false, ref flag5, out flag3);
						}
						else if (dictionary != null && dictionary.Values.Contains("\\archive"))
						{
							Tracer.TraceInformation("ItemListGenerator.GenerateItemListForSourceMailbox: call GetAllItemIdsFromServer isunsearchable for archive for mailbox: '{0}'", new object[]
							{
								sourceMailbox.Configuration.Id
							});
							this.GetAllItemIdsFromServer<ItemId>(sourceMailbox, new ItemListGenerator.GetItemIdPageFromServerDelegate<ItemId>(this.GetItemIdPageFromServer), dictionary, isUnsearchable, true, ref flag5, out flag3);
						}
						ExportException ex2 = AsynchronousTaskHandler.WaitForAsynchronousTask(this.writingTask);
						if (ex2 != null)
						{
							errorHappened2 = true;
							throw ex2;
						}
					}
					finally
					{
						this.DataRetriever.DataWriter.ExitDataContext(errorHappened2);
					}
				}
			}
			catch (ExportException ex3)
			{
				Tracer.TraceInformation("ItemListGenerator.DoExportForSourceMailbox: Error for mailbox '{0}'. Exception: {1}", new object[]
				{
					sourceMailbox.Configuration.Id,
					ex3
				});
				errorRecord = new ErrorRecord
				{
					SourceId = sourceMailbox.Configuration.Id,
					Item = null,
					ErrorType = ex3.ErrorType,
					DiagnosticMessage = ex3.Message,
					Time = DateTime.UtcNow
				};
			}
			if (ScenarioData.Current.ContainsKey("PM"))
			{
				ScenarioData.Current.Remove("PM");
			}
			if (errorRecord != null)
			{
				this.AbortForSourceMailbox(errorRecord);
			}
		}

		void IDisposable.Dispose()
		{
			this.Close();
		}

		private static Dictionary<string, string> PrepareAllFolders(ISourceDataProvider serviceClient, string sourceId)
		{
			List<string> list = new List<string>
			{
				serviceClient.GetRootFolder(sourceId, false),
				serviceClient.GetRootFolder(sourceId, true)
			};
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add(list[0], "\\root");
			serviceClient.GetAllFolders(sourceId, null, true, false, dictionary);
			Tracer.TraceInformation("ItemListGenerator.PrepareAllFolders: {0} folders found in primary mailbox", new object[]
			{
				dictionary.Count
			});
			if (list[1] != null)
			{
				Tracer.TraceInformation("ItemListGenerator.PrepareAllFolders: Archive found for mailbox '{0}'", new object[]
				{
					sourceId
				});
				dictionary.Add(list[1], "\\archive");
				serviceClient.GetAllFolders(sourceId, null, true, true, dictionary);
			}
			Tracer.TraceInformation("ItemListGenerator.PrepareAllFolders: {0} folders found in primary + archive mailboxes", new object[]
			{
				dictionary.Count
			});
			return dictionary;
		}

		private static void UpdateItemId(Dictionary<string, string> allFolders, ItemId itemId, string sourceId)
		{
			string text = null;
			if (itemId.ParentFolder != null)
			{
				if (allFolders != null && !sourceId.StartsWith("\\"))
				{
					allFolders.TryGetValue(itemId.ParentFolder, out text);
					if (string.IsNullOrEmpty(text))
					{
						Tracer.TraceError("ItemListGenerator.UpdateItemId: Parent folder path not found. Item id: {0}; Parent folder Id: {1}", new object[]
						{
							itemId.Id,
							itemId.ParentFolder
						});
					}
					itemId.ParentFolder = text;
				}
				else
				{
					itemId.ParentFolder = sourceId;
				}
			}
			itemId.SourceId = sourceId;
		}

		private void GenerateItemList()
		{
			Tracer.TraceInformation("ItemListGenerator.GenerateItemList: Entering method...", new object[0]);
			this.itemListCache = new Dictionary<string, IItemIdList>();
			if (this.target.ExportContext.ExportMetadata.IncludeUnsearchableItems)
			{
				this.unsearchableItemListCache = new Dictionary<string, IItemIdList>();
			}
			this.failedMailboxes = new HashSet<string>();
			if (!this.target.ExportContext.ExportMetadata.IncludeDuplicates)
			{
				this.uniqueItemIds = new Dictionary<object, BasicItemId>();
				if (this.target.ExportContext.IsResume)
				{
					Tracer.TraceInformation("ItemListGenerator.GenerateItemList: Reconstructing unique hash map...", new object[0]);
					this.RecreateUniqueItemHashMap();
				}
			}
			Tracer.TraceInformation("ItemListGenerator.GenerateItemList: Starting generating list...", new object[0]);
			foreach (SourceInformation sourceInformation in this.AllSourceInformation.Values)
			{
				if (this.progressController.IsStopRequested)
				{
					Tracer.TraceInformation("ItemListGenerator.GenerateItemList: Stop requested point 3", new object[0]);
					break;
				}
				if (!this.target.ExportContext.IsResume || !sourceInformation.Status.IsSearchCompleted(this.target.ExportContext.ExportMetadata.IncludeSearchableItems, this.target.ExportContext.ExportMetadata.IncludeUnsearchableItems))
				{
					this.GenerateItemListForSourceMailbox(sourceInformation);
				}
			}
		}

		private void RecreateUniqueItemHashMap()
		{
			SourceInformation sourceInformation = this.AllSourceInformation.Values.FirstOrDefault((SourceInformation source) => source.ServiceClient != null);
			foreach (SourceInformation sourceInformation2 in this.AllSourceInformation.Values)
			{
				if (this.progressController.IsStopRequested)
				{
					Tracer.TraceInformation("ItemListGenerator.RecreateUniqueItemHashMap: Stop requested point 1", new object[0]);
					break;
				}
				if (sourceInformation2.Status.ItemCount > 0)
				{
					IItemIdList itemIdList = this.target.CreateItemIdList(sourceInformation2.Configuration.Id, false);
					this.AddItemIdListToUniqueItemHashMap(itemIdList, sourceInformation.ServiceClient);
				}
				if (sourceInformation2.Status.UnsearchableItemCount > 0)
				{
					IItemIdList itemIdList2 = this.target.CreateItemIdList(sourceInformation2.Configuration.Id, true);
					this.AddItemIdListToUniqueItemHashMap(itemIdList2, sourceInformation.ServiceClient);
				}
			}
		}

		private void AddItemIdListToUniqueItemHashMap(IItemIdList itemIdList, ISourceDataProvider itemHashProvider)
		{
			try
			{
				foreach (ItemId itemId in itemIdList.ReadItemIds())
				{
					if (this.progressController.IsStopRequested)
					{
						Tracer.TraceInformation("ItemListGenerator.GenerateItemList: Stop requested point 2", new object[0]);
						break;
					}
					if (!string.IsNullOrEmpty(itemId.UniqueHash))
					{
						object itemHashObject = itemHashProvider.GetItemHashObject(itemId.UniqueHash);
						if (itemHashObject != null && !this.uniqueItemIds.ContainsKey(itemHashObject))
						{
							this.uniqueItemIds.Add(itemHashObject, new BasicItemId
							{
								Id = itemId.Id,
								SourceId = itemId.SourceId
							});
						}
					}
				}
			}
			catch (ExportException ex)
			{
				Tracer.TraceError("ItemListGenerator.AddItemIdListToUniqueItemHashMap: {0}", new object[]
				{
					ex
				});
				throw new ExportException(ExportErrorType.FailedToRecreateUniqueItemHashMap, ex);
			}
		}

		private void GenerateItemListForSourceMailbox(SourceInformation sourceMailbox)
		{
			Tracer.TraceInformation("ItemListGenerator.GenerateItemListForSourceMailbox: Generating item list for mailbox '{0}'", new object[]
			{
				sourceMailbox.Configuration.Id
			});
			ErrorRecord errorRecord = null;
			bool flag = sourceMailbox.Configuration.Id.StartsWith("\\");
			ScenarioData.Current["PM"] = sourceMailbox.Configuration.LegacyExchangeDN.GetHashCode().ToString();
			if (sourceMailbox.Status.ItemCount < 0)
			{
				sourceMailbox.Status.ItemCount = 0;
			}
			if (sourceMailbox.Status.UnsearchableItemCount < 0)
			{
				sourceMailbox.Status.UnsearchableItemCount = 0;
			}
			try
			{
				Dictionary<string, string> dictionary = null;
				if (!flag)
				{
					dictionary = ItemListGenerator.PrepareAllFolders(sourceMailbox.ServiceClient, sourceMailbox.Configuration.Id);
				}
				bool flag2 = false;
				bool flag3 = false;
				bool isUnsearchable = false;
				if (this.target.ExportContext.ExportMetadata.IncludeSearchableItems)
				{
					Tracer.TraceInformation("ItemListGenerator.GenerateItemListForSourceMailbox: call GetAllItemIdsFromServer for primary for mailbox: '{0}'", new object[]
					{
						sourceMailbox.Configuration.Id
					});
					this.GetAllItemIdsFromServer<ItemId>(sourceMailbox, new ItemListGenerator.GetItemIdPageFromServerDelegate<ItemId>(this.GetItemIdPageFromServer), dictionary, isUnsearchable, false, ref flag2, out flag3);
				}
				if (this.target.ExportContext.ExportMetadata.IncludeSearchableItems && dictionary != null && dictionary.Values.Contains("\\archive"))
				{
					Tracer.TraceInformation("ItemListGenerator.GenerateItemListForSourceMailbox: call GetAllItemIdsFromServer for archive for mailbox: '{0}'", new object[]
					{
						sourceMailbox.Configuration.Id
					});
					this.GetAllItemIdsFromServer<ItemId>(sourceMailbox, new ItemListGenerator.GetItemIdPageFromServerDelegate<ItemId>(this.GetItemIdPageFromServer), dictionary, isUnsearchable, true, ref flag2, out flag3);
				}
				isUnsearchable = true;
				if (this.target.ExportContext.ExportMetadata.IncludeUnsearchableItems)
				{
					flag2 = false;
					Tracer.TraceInformation("ItemListGenerator.GenerateItemListForSourceMailbox: call GetAllItemIdsFromServer isunsearchable for primary for mailbox: '{0}'", new object[]
					{
						sourceMailbox.Configuration.Id
					});
					this.GetAllItemIdsFromServer<ItemId>(sourceMailbox, new ItemListGenerator.GetItemIdPageFromServerDelegate<ItemId>(this.GetItemIdPageFromServer), dictionary, isUnsearchable, false, ref flag2, out flag3);
					if (!flag3)
					{
						Tracer.TraceInformation("ItemListGenerator.GenerateItemListForSourceMailbox: call GetAllItemIdsFromServer isunsearchable for newschemafailed for mailbox: '{0}'", new object[]
						{
							sourceMailbox.Configuration.Id
						});
						this.GetAllItemIdsFromServer<UnsearchableItemId>(sourceMailbox, new ItemListGenerator.GetItemIdPageFromServerDelegate<UnsearchableItemId>(this.GetUnsearchableItemIdPageFromServer), dictionary, isUnsearchable, false, ref flag2, out flag3);
					}
					else if (dictionary != null && dictionary.Values.Contains("\\archive"))
					{
						Tracer.TraceInformation("ItemListGenerator.GenerateItemListForSourceMailbox: call GetAllItemIdsFromServer isunsearchable for archive for mailbox: '{0}'", new object[]
						{
							sourceMailbox.Configuration.Id
						});
						this.GetAllItemIdsFromServer<ItemId>(sourceMailbox, new ItemListGenerator.GetItemIdPageFromServerDelegate<ItemId>(this.GetItemIdPageFromServer), dictionary, isUnsearchable, true, ref flag2, out flag3);
					}
				}
			}
			catch (ExportException ex)
			{
				Tracer.TraceInformation("ItemListGenerator.GenerateItemListForSourceMailbox: Error for mailbox '{0}'. Exception: {1}", new object[]
				{
					sourceMailbox.Configuration.Id,
					ex
				});
				errorRecord = new ErrorRecord
				{
					SourceId = sourceMailbox.Configuration.Id,
					Item = null,
					ErrorType = ex.ErrorType,
					DiagnosticMessage = ex.Message,
					Time = DateTime.UtcNow
				};
			}
			if (ScenarioData.Current.ContainsKey("PM"))
			{
				ScenarioData.Current.Remove("PM");
			}
			if (errorRecord != null)
			{
				this.AbortForSourceMailbox(errorRecord);
			}
		}

		private List<ItemId> GetItemIdPageFromServer(SourceInformation sourceMailbox, ref string pageItemReference, out bool newSchemaSearchSucceeded, bool isUnsearchable, bool isArchive)
		{
			List<ErrorRecord> list = null;
			List<ItemId> list2 = null;
			int num = 0;
			int retryInterval = ConstantProvider.RetryInterval;
			bool flag = sourceMailbox.Configuration.Id.StartsWith("\\");
			string text;
			do
			{
				text = pageItemReference;
				if (num > 0)
				{
					Thread.Sleep(retryInterval);
					Tracer.TraceInformation("ItemListGenerator.GetItemIdPageFromServer: Retry on failed mailbox. Error: '{0}'", new object[]
					{
						list[0].DiagnosticMessage
					});
				}
				list2 = sourceMailbox.ServiceClient.SearchMailboxes(flag ? sourceMailbox.Configuration.Name : sourceMailbox.Configuration.Id, sourceMailbox.Configuration.SourceFilter, this.target.ExportContext.ExportMetadata.Language, new string[]
				{
					flag ? sourceMailbox.Configuration.Id : sourceMailbox.Configuration.LegacyExchangeDN
				}, isUnsearchable, ref text, out list, out newSchemaSearchSucceeded, isArchive, (sourceMailbox.Configuration.SearchName != null) ? sourceMailbox.Configuration.SearchName : this.target.ExportContext.ExportMetadata.ExportName);
			}
			while (list != null && list.Count > 0 && num++ < 3);
			if (list != null && list.Count > 0)
			{
				Tracer.TraceInformation("ItemListGenerator.GetItemIdPageFromServer: SearchMailboxes returned failed response for mailbox '{0}'", new object[]
				{
					sourceMailbox.Configuration.Id
				});
				throw new ExportException(list[0].ErrorType, list[0].DiagnosticMessage);
			}
			pageItemReference = text;
			List<ItemId> list3 = new List<ItemId>();
			foreach (ItemId itemId in list2)
			{
				if (itemId.NeedsDGExpansion)
				{
					list3.Add(itemId);
				}
			}
			if (list3.Count > 0)
			{
				sourceMailbox.ServiceClient.UpdateDGExpansion(flag ? sourceMailbox.Configuration.Name : sourceMailbox.Configuration.Id, list3);
			}
			return list2;
		}

		private List<UnsearchableItemId> GetUnsearchableItemIdPageFromServer(SourceInformation sourceMailbox, ref string pageItemReference, out bool newSchemaSearchSucceeded, bool isUnsearchable, bool isArchive)
		{
			bool flag = sourceMailbox.Configuration.Id.StartsWith("\\");
			List<UnsearchableItemId> unsearchableItems = sourceMailbox.ServiceClient.GetUnsearchableItems(flag ? sourceMailbox.Configuration.Name : sourceMailbox.Configuration.Id, flag ? sourceMailbox.Configuration.Id : sourceMailbox.Configuration.LegacyExchangeDN, ref pageItemReference);
			if (unsearchableItems != null && unsearchableItems.Count > 0)
			{
				foreach (IGrouping<string, UnsearchableItemId> source in from itemId in unsearchableItems
				group itemId by sourceMailbox.ServiceClient.GetPhysicalPartitionIdentifier(itemId))
				{
					sourceMailbox.ServiceClient.FillInUnsearchableItemIds(flag ? sourceMailbox.Configuration.Name : sourceMailbox.Configuration.Id, source.ToList<UnsearchableItemId>());
				}
			}
			newSchemaSearchSucceeded = true;
			return unsearchableItems;
		}

		private void GetAllItemIdsFromServer<T>(SourceInformation sourceMailbox, ItemListGenerator.GetItemIdPageFromServerDelegate<T> serverCall, Dictionary<string, string> allFolders, bool isUnsearchable, bool isArchive, ref bool isContextEntered, out bool newSchemaSearchSucceeded) where T : ItemId, new()
		{
			string text = null;
			string text2 = null;
			List<T> list = null;
			bool flag = true;
			newSchemaSearchSucceeded = false;
			while (flag)
			{
				list = serverCall(sourceMailbox, ref text, out newSchemaSearchSucceeded, isUnsearchable, isArchive);
				if (isUnsearchable && !newSchemaSearchSucceeded)
				{
					return;
				}
				if (list != null && list.Count > 0)
				{
					string a = text2;
					T t = list[0];
					if (a == t.Id)
					{
						list.RemoveAt(0);
					}
					if (list.Count > 0)
					{
						T t2 = list[list.Count - 1];
						text2 = t2.Id;
						string id = sourceMailbox.Configuration.Id;
						foreach (T t3 in list)
						{
							ItemId itemId = t3;
							ItemListGenerator.UpdateItemId(allFolders, itemId, id);
						}
						ItemListGenerator.ItemIdsDataBatch itemIdsDataBatch = new ItemListGenerator.ItemIdsDataBatch
						{
							ItemIds = list.Cast<ItemId>(),
							IsUnsearchable = isUnsearchable
						};
						IList<ItemId> list2 = null;
						this.SaveItemIdListToCache(itemIdsDataBatch);
						if (itemIdsDataBatch.IsUnsearchable)
						{
							if (this.unsearchableItemListCache.ContainsKey(id))
							{
								list2 = this.unsearchableItemListCache[id].MemoryCache;
							}
						}
						else if (this.itemListCache.ContainsKey(id))
						{
							list2 = this.itemListCache[id].MemoryCache;
						}
						if (!isContextEntered && this.DataRetriever != null && this.DataRetriever.DataWriter != null)
						{
							this.DataRetriever.DataWriter.EnterDataContext(this.DataRetriever.DataContext);
							if (this.DataRetriever.DataContext.ItemCount > 0)
							{
								isContextEntered = true;
							}
						}
						if (this.DataRetriever != null && list2 != null)
						{
							int count = list2.Count;
							bool flag2 = false;
							List<ItemId> list3 = new List<ItemId>(list2.Count);
							list3.AddRange(list2);
							while (count-- > 0 && list3.Count > 0)
							{
								this.DataRetriever.ProcessItems(ref list3);
								List<ItemInformation> items = this.DataRetriever.ProcessBatchData();
								if (!flag2)
								{
									this.OnDataBatchRead(itemIdsDataBatch);
									flag2 = true;
								}
								if (this.progressController.IsStopRequested)
								{
									Tracer.TraceInformation("ItemListGenerator.GetAllItemIdsFromServer: Stop requested for mailbox '{0}'", new object[]
									{
										sourceMailbox.Configuration.Id
									});
									break;
								}
								this.DataRetriever.OnDataBatchRead(items);
							}
							if (count <= 0 && list3.Count > 0)
							{
								Tracer.TraceError("ItemListGenerator.GetAllItemIdsFromServer: error: count <= 0 && tempBatch.Count > 0, count: {0}; tempBatch.Count: {1}", new object[]
								{
									count,
									list3.Count
								});
							}
						}
					}
					else
					{
						Tracer.TraceInformation("ItemListGenerator.GenerateItemListForSourceMailbox: Reset pageItemReference to null because there is only one item in the page and it is the same one as the last of the last page. pageItemReference was '{0}'.", new object[]
						{
							text ?? "null"
						});
						text = null;
					}
				}
				flag = (text != null);
				Tracer.TraceInformation("ItemListGenerator.GenerateItemListForSourceMailbox: pageItemReference='{0}' for mailbox '{1}'; current item count in page: {2}.", new object[]
				{
					text ?? "null",
					sourceMailbox.Configuration.Id,
					(list == null) ? 0 : list.Count
				});
			}
		}

		private void OnDataBatchRead(ItemListGenerator.ItemIdsDataBatch itemIds)
		{
			EventHandler<DataBatchEventArgs<ItemListGenerator.ItemIdsDataBatch>> dataBatchRead = this.DataBatchRead;
			if (dataBatchRead != null)
			{
				dataBatchRead(this, new DataBatchEventArgs<ItemListGenerator.ItemIdsDataBatch>
				{
					DataBatch = itemIds
				});
			}
		}

		private void AbortForSourceMailbox(ErrorRecord error)
		{
			EventHandler abortingOnError = this.AbortingOnError;
			if (abortingOnError != null)
			{
				abortingOnError(this, EventArgs.Empty);
			}
			this.failedMailboxes.Add(error.SourceId);
			ProgressRecord progressRecord = new ProgressRecord();
			progressRecord.ReportSourceError(error);
			this.progressController.ReportProgress(progressRecord);
		}

		private void ResetStatusForFailedMailbox(string sourceId)
		{
			this.AllSourceInformation[sourceId].Status = new SourceInformation.SourceStatus();
			if (this.itemListCache != null && this.itemListCache.ContainsKey(sourceId))
			{
				this.itemListCache.Remove(sourceId);
			}
			if (this.unsearchableItemListCache != null && this.unsearchableItemListCache.ContainsKey(sourceId))
			{
				this.unsearchableItemListCache.Remove(sourceId);
			}
			try
			{
				this.target.RemoveItemIdList(sourceId, false);
			}
			catch (ExportException ex)
			{
				Tracer.TraceError("ItemListGenerator.ResetStatusForFailedMailbox: Failed to remove item list for mailbox '{0}'. Exception: {1}", new object[]
				{
					sourceId,
					ex
				});
			}
			try
			{
				this.target.RemoveItemIdList(sourceId, true);
			}
			catch (ExportException ex2)
			{
				Tracer.TraceError("ItemListGenerator.ResetStatusForFailedMailbox: Failed to remove unsearchable item list for mailbox '{0}'. Exception: {1}", new object[]
				{
					sourceId,
					ex2
				});
			}
		}

		private void FlushItemIds(ItemListGenerator.ItemIdsDataBatch dataBatch)
		{
			foreach (ItemId itemId in dataBatch.ItemIds)
			{
				if (this.progressController.IsStopRequested)
				{
					Tracer.TraceInformation("ItemListGenerator.FlushItemIds: Stop requested", new object[0]);
					break;
				}
				if (itemId.Id != null)
				{
					string sourceId = itemId.SourceId;
					if (dataBatch.IsUnsearchable)
					{
						if (this.unsearchableItemListCache.ContainsKey(sourceId))
						{
							this.unsearchableItemListCache[sourceId].Flush();
							break;
						}
						break;
					}
					else
					{
						if (this.itemListCache.ContainsKey(sourceId))
						{
							this.itemListCache[sourceId].Flush();
							break;
						}
						break;
					}
				}
			}
		}

		private void SaveItemIdListToCache(ItemListGenerator.ItemIdsDataBatch dataBatch)
		{
			string text = string.Empty;
			foreach (ItemId itemId in dataBatch.ItemIds)
			{
				if (this.progressController.IsStopRequested)
				{
					Tracer.TraceInformation("ItemListGenerator.SaveItemIdListToCache: Stop requested", new object[0]);
					break;
				}
				if (itemId.Id != null)
				{
					text = itemId.SourceId;
					long num = 0L;
					bool flag = false;
					if (!this.includeDuplicates && !string.IsNullOrEmpty(itemId.UniqueHash))
					{
						BasicItemId basicItemId = null;
						object itemHashObject = this.AllSourceInformation[itemId.SourceId].ServiceClient.GetItemHashObject(itemId.UniqueHash);
						if (itemHashObject != null)
						{
							if (!this.uniqueItemIds.TryGetValue(itemHashObject, out basicItemId))
							{
								this.uniqueItemIds.Add(itemHashObject, new BasicItemId
								{
									Id = itemId.Id,
									SourceId = itemId.SourceId
								});
								num = (long)((ulong)itemId.Size);
							}
							else if (this.failedMailboxes.Contains(basicItemId.SourceId))
							{
								Tracer.TraceInformation("ItemListGenerator.SaveItemIdListToCache: Found item id of failed mailbox '{0}' in unique hash map. Replacing it with the item id in mailbox '{1}'", new object[]
								{
									basicItemId.SourceId,
									itemId.SourceId
								});
								this.uniqueItemIds[itemHashObject] = new BasicItemId
								{
									Id = itemId.Id,
									SourceId = itemId.SourceId
								};
								num = (long)((ulong)itemId.Size);
							}
							else
							{
								if (itemId.Id == basicItemId.Id)
								{
									Tracer.TraceInformation("ItemListGenerator.SaveItemIdListToCache: Found exactly same item id '{0}', skipping it.", new object[]
									{
										basicItemId.Id
									});
									this.AllSourceInformation[text].Status.UnsearchableDuplicateItemCount += 1L;
									this.AllSourceInformation[text].Status.UnsearchableItemCount++;
									continue;
								}
								itemId.PrimaryItemId = basicItemId.Id;
								flag = true;
							}
						}
					}
					else
					{
						num = (long)((ulong)itemId.Size);
					}
					this.AllSourceInformation[text].Status.TotalSize += num;
					if (dataBatch.IsUnsearchable)
					{
						if (!this.unsearchableItemListCache.ContainsKey(text))
						{
							this.unsearchableItemListCache[text] = this.target.CreateItemIdList(text, true);
						}
						this.AllSourceInformation[text].Status.UnsearchableItemCount++;
						if (flag)
						{
							this.AllSourceInformation[text].Status.UnsearchableDuplicateItemCount += 1L;
						}
						this.unsearchableItemListCache[text].WriteItemId(itemId);
					}
					else
					{
						if (!this.itemListCache.ContainsKey(text))
						{
							this.itemListCache[text] = this.target.CreateItemIdList(text, false);
						}
						this.AllSourceInformation[text].Status.ItemCount++;
						if (flag)
						{
							this.AllSourceInformation[text].Status.DuplicateItemCount += 1L;
						}
						this.itemListCache[text].WriteItemId(itemId);
					}
				}
			}
		}

		private void FlushItemListCache(IEnumerable<IItemIdList> itemIdLists)
		{
			foreach (IItemIdList itemIdList in itemIdLists)
			{
				if (this.progressController.IsStopRequested)
				{
					Tracer.TraceInformation("ItemListGenerator.FlushItemListCache: Stop requested", new object[0]);
					break;
				}
				try
				{
					if (!this.failedMailboxes.Contains(itemIdList.SourceId))
					{
						Tracer.TraceInformation("ItemListGenerator.FlushItemListCache: Flushing item id list for mailbox '{0}'", new object[]
						{
							itemIdList.SourceId
						});
						itemIdList.Flush();
					}
				}
				catch (ExportException ex)
				{
					Tracer.TraceInformation("ItemListGenerator.FlushItemListCache: Error when flushing item id list for mailbox '{0}'. Exception: {1}", new object[]
					{
						itemIdList.SourceId,
						ex
					});
					this.AbortForSourceMailbox(new ErrorRecord
					{
						SourceId = itemIdList.SourceId,
						Item = null,
						ErrorType = ex.ErrorType,
						DiagnosticMessage = ex.Message,
						Time = DateTime.UtcNow
					});
				}
			}
		}

		private readonly ITarget target;

		private readonly bool includeDuplicates;

		private readonly IProgressController progressController;

		private bool doUnSearchable;

		private Dictionary<string, IItemIdList> itemListCache;

		private Dictionary<string, IItemIdList> unsearchableItemListCache;

		private Dictionary<object, BasicItemId> uniqueItemIds;

		private HashSet<string> failedMailboxes;

		private Task writingTask;

		private delegate List<T> GetItemIdPageFromServerDelegate<T>(SourceInformation sourceMailbox, ref string pageItemReference, out bool newSchemaSearchSucceeded, bool isUnsearchable, bool isArchive) where T : ItemId, new();

		public class ItemIdsDataBatch
		{
			public IEnumerable<ItemId> ItemIds { get; set; }

			public bool IsUnsearchable { get; set; }
		}
	}
}
