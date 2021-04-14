using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Web;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter;
using Microsoft.Exchange.AirSync.SchemaConverter.AirSync;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal class MailboxSearchProvider : DisposeTrackableBase, ISearchProvider
	{
		internal MailboxSearchProvider(MailboxSession mailboxSession, SyncStateStorage syncStateStorage, IAirSyncContext context)
		{
			if (context.Request.Version < 120)
			{
				throw new SearchProtocolErrorException
				{
					ErrorStringForProtocolLogger = "SearchProtocolError10"
				};
			}
			this.mailboxSession = mailboxSession;
			this.syncStateStorage = syncStateStorage;
			this.context = context;
			this.versionFactory = AirSyncProtocolVersionParserBuilder.FromVersion(context.Request.Version);
			AirSyncCounters.NumberOfMailboxSearches.Increment();
		}

		public int NumberResponses
		{
			get
			{
				return this.numberResponses;
			}
		}

		public int Folders
		{
			get
			{
				if (this.folderScope != null)
				{
					return this.folderScope.Length;
				}
				return 0;
			}
		}

		public bool DeepTraversal
		{
			get
			{
				return this.deepTraversal;
			}
		}

		public bool RightsManagementSupport
		{
			get
			{
				return this.mailboxSchemaOptions.RightsManagementSupport;
			}
		}

		private CustomSyncState FolderIdMappingSyncState
		{
			get
			{
				if (this.folderIdMappingSyncState == null)
				{
					this.folderIdMappingSyncState = this.syncStateStorage.GetCustomSyncState(new FolderIdMappingSyncStateInfo(), new PropertyDefinition[0]);
				}
				return this.folderIdMappingSyncState;
			}
		}

		public void BuildResponse(XmlElement responseNode)
		{
			MailboxSearchProvider.BuildResponseState buildResponseState = MailboxSearchProvider.BuildResponseState.Seeking;
			int num = -1;
			bool flag = false;
			this.totalRowCount = 0;
			this.numberResponses = 0;
			XmlNode xmlNode = responseNode.OwnerDocument.CreateElement("Status", "Search:");
			responseNode.AppendChild(xmlNode);
			xmlNode.InnerText = 1.ToString(CultureInfo.InvariantCulture);
			foreach (MailboxSearchCriteriaBuilder.SchemaCacheItem schemaCacheItem in this.schemaCache.Values)
			{
				schemaCacheItem.SchemaState.Options.Clear();
				this.mailboxSchemaOptions.PopulateOptionsCollection(this.context.Request.DeviceIdentity.DeviceType, schemaCacheItem.SchemaState.Options);
			}
			bool flag2 = false;
			int num2 = 0;
			bool hasBodyPartPreferences = this.mailboxSchemaOptions.HasBodyPartPreferences;
			if (this.queryFilter != null)
			{
				using (QueryResult queryResult = this.searchFolder.ItemQuery(ItemQueryType.None, null, this.defaultSortBy, MailboxSearchProvider.queryColumns))
				{
					this.uncappedResultCount = queryResult.EstimatedRowCount;
					num2 = this.uncappedResultCount;
					while (!flag2 && num <= this.maxRange)
					{
						object[][] rows = queryResult.GetRows(this.maxRange + 1);
						flag2 = (rows.Length == 0);
						for (int i = 0; i < rows.Length; i++)
						{
							StoreObjectId objectId = (rows[i][0] as VersionedId).ObjectId;
							string itemClass = rows[i][1] as string;
							StoreObjectId parentFolderId = rows[i][2] as StoreObjectId;
							string innerText = null;
							try
							{
								AirSyncDataObject airSyncDataObject = null;
								IServerDataObject serverDataObject = null;
								foreach (string text in this.schemaCache.Keys)
								{
									MailboxSearchCriteriaBuilder.SchemaCacheItem schemaCacheItem2 = this.schemaCache[text];
									if (schemaCacheItem2.MailboxDataObject.CanConvertItemClassUsingCurrentSchema(itemClass))
									{
										innerText = text;
										airSyncDataObject = schemaCacheItem2.AirSyncDataObject;
										serverDataObject = schemaCacheItem2.MailboxDataObject;
										break;
									}
								}
								if (airSyncDataObject != null)
								{
									if (this.searchCriteriaBuilder.DoesMatchCriteria(parentFolderId, objectId))
									{
										num++;
										if (MailboxSearchProvider.BuildResponseState.BuildingResponse == buildResponseState || (buildResponseState == MailboxSearchProvider.BuildResponseState.Seeking && num >= this.minRange))
										{
											buildResponseState = MailboxSearchProvider.BuildResponseState.BuildingResponse;
											buildResponseState = MailboxSearchProvider.BuildResponseState.BuildingResponse;
											Item item = null;
											try
											{
												item = this.BindToItemWithItemClass(objectId, itemClass, serverDataObject.GetPrefetchProperties());
												if (this.searchCriteriaBuilder.ExcludedFolders.Contains(item.ParentId))
												{
													num--;
												}
												else
												{
													XmlNode xmlNode2 = responseNode.OwnerDocument.CreateElement("Result", "Search:");
													XmlNode xmlNode3 = responseNode.OwnerDocument.CreateElement("Properties", "Search:");
													XmlNode xmlNode4 = responseNode.OwnerDocument.CreateElement("Class", "AirSync:");
													xmlNode4.InnerText = innerText;
													XmlNode xmlNode5 = responseNode.OwnerDocument.CreateElement("LongId", "Search:");
													XmlNode xmlNode6 = responseNode.OwnerDocument.CreateElement("CollectionId", "AirSync:");
													xmlNode5.InnerText = HttpUtility.UrlEncode(item.Id.ObjectId.ToBase64String());
													if (hasBodyPartPreferences)
													{
														if (this.searchCriteriaBuilder.Conversation == null)
														{
															throw new InvalidOperationException("Conversation object should not be null here when body part is requested!");
														}
														this.searchCriteriaBuilder.Conversation.LoadItemParts(new List<StoreObjectId>
														{
															item.StoreObjectId
														});
													}
													if (this.mailboxSchemaOptions.RightsManagementSupport)
													{
														Command.CurrentCommand.DecodeIrmMessage(item, false);
													}
													try
													{
														serverDataObject.Bind(item);
														airSyncDataObject.Bind(xmlNode3);
														airSyncDataObject.CopyFrom(serverDataObject);
													}
													finally
													{
														airSyncDataObject.Unbind();
														serverDataObject.Unbind();
													}
													if (this.mailboxSchemaOptions.RightsManagementSupport)
													{
														Command.CurrentCommand.SaveLicense(item);
													}
													xmlNode2.AppendChild(xmlNode4);
													xmlNode2.AppendChild(xmlNode5);
													if (this.FolderIdMappingSyncState == null)
													{
														throw new SearchNeedToFolderSyncException();
													}
													string text2 = ((FolderIdMapping)this.FolderIdMappingSyncState[CustomStateDatumType.IdMapping])[MailboxSyncItemId.CreateForNewItem(item.ParentId)];
													if (text2 == null)
													{
														throw new SearchNeedToFolderSyncException();
													}
													xmlNode6.InnerText = text2;
													xmlNode2.AppendChild(xmlNode6);
													xmlNode2.AppendChild(xmlNode3);
													responseNode.AppendChild(xmlNode2);
													this.numberResponses++;
													if (this.minRange + this.numberResponses > this.maxRange)
													{
														flag = true;
														buildResponseState = MailboxSearchProvider.BuildResponseState.CountingTotal;
													}
												}
											}
											catch
											{
												num--;
												throw;
											}
											finally
											{
												if (item != null)
												{
													item.Dispose();
												}
											}
										}
									}
								}
							}
							catch (Exception ex)
							{
								if (ex is SearchNeedToFolderSyncException)
								{
									throw;
								}
								if (!SyncCommand.IsItemSyncTolerableException(ex) && !SyncCommand.IsObjectNotFound(ex))
								{
									throw;
								}
							}
						}
					}
					if (!flag && this.uncappedResultCount > queryResult.EstimatedRowCount)
					{
						this.haveUnretrievableResults = true;
					}
				}
			}
			if (this.numberResponses == 0)
			{
				XmlNode newChild = responseNode.OwnerDocument.CreateElement("Result", "Search:");
				responseNode.AppendChild(newChild);
				if (this.minRange >= GlobalSettings.MaxMailboxSearchResults)
				{
					xmlNode.InnerText = 12.ToString(CultureInfo.InvariantCulture);
					return;
				}
			}
			else if (this.rangeSpecified)
			{
				XmlNode xmlNode7 = responseNode.OwnerDocument.CreateElement("Range", "Search:");
				XmlNode xmlNode8 = responseNode.OwnerDocument.CreateElement("Total", "Search:");
				xmlNode7.InnerText = this.minRange.ToString(CultureInfo.InvariantCulture) + "-" + (this.minRange + this.numberResponses - 1).ToString(CultureInfo.InvariantCulture);
				responseNode.AppendChild(xmlNode7);
				if (this.haveUnretrievableResults)
				{
					this.totalRowCount = this.uncappedResultCount;
					xmlNode.InnerText = 12.ToString(CultureInfo.InvariantCulture);
				}
				else if (this.uncappedResultCount > num2 || !flag2)
				{
					this.totalRowCount = this.uncappedResultCount;
				}
				else
				{
					this.totalRowCount = num + 1;
					if (this.minRange + this.numberResponses == GlobalSettings.MaxMailboxSearchResults)
					{
						xmlNode.InnerText = 12.ToString(CultureInfo.InvariantCulture);
					}
				}
				xmlNode8.InnerText = this.totalRowCount.ToString(CultureInfo.InvariantCulture);
				responseNode.AppendChild(xmlNode8);
			}
		}

		public void Execute()
		{
			Folder folder = null;
			try
			{
				folder = Folder.Bind(this.mailboxSession, this.mailboxSession.GetDefaultFolderId(DefaultFolderType.SearchFolders));
				string text = "EAS Search Device:" + this.context.Request.DeviceIdentity.DeviceId;
				bool flag = !this.forceResetSearchCriteria;
				AirSyncDiagnostics.TraceDebug<bool>(ExTraceGlobals.AlgorithmTracer, this, "reuseExistingSearchFolderCriteria = !this.forceResetSearchCriteria, therefore reuseExistingSearchFolderCriteria = {0}", flag);
				if (this.queryFilter != null)
				{
					StoreObjectId storeObjectId = null;
					using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, null, MailboxSearchProvider.displayNameSortBy, new PropertyDefinition[]
					{
						FolderSchema.Id,
						StoreObjectSchema.DisplayName
					}))
					{
						QueryFilter seekFilter = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.DisplayName, text);
						if (queryResult.SeekToCondition(SeekReference.OriginBeginning, seekFilter))
						{
							object[][] rows = queryResult.GetRows(1);
							storeObjectId = ((VersionedId)rows[0][0]).ObjectId;
						}
					}
					SearchFolderCriteria searchFolderCriteria = null;
					if (storeObjectId != null)
					{
						this.searchFolder = SearchFolder.Bind(this.mailboxSession, storeObjectId, new PropertyDefinition[]
						{
							MailboxSearchProvider.SearchFolderTextDescription,
							FolderSchema.ItemCount,
							FolderSchema.UnreadCount,
							FolderSchema.SearchFolderItemCount
						});
						try
						{
							searchFolderCriteria = this.searchFolder.GetSearchCriteria();
							AirSyncDiagnostics.TraceDebug<SearchFolderCriteria>(ExTraceGlobals.AlgorithmTracer, this, "Existing search folder has search criteria {0}", searchFolderCriteria);
						}
						catch (StoragePermanentException)
						{
							flag = false;
							AirSyncDiagnostics.TraceDebug(ExTraceGlobals.AlgorithmTracer, this, "GetSearchCriteria() threw StoragePermanentException due to a pre-existing restriction on the search folder.  This condition is expected and handled.");
						}
						catch (ArgumentException)
						{
							flag = false;
							AirSyncDiagnostics.TraceDebug(ExTraceGlobals.AlgorithmTracer, this, "GetSearchCriteria() threw ArgumentException due to a pre-existing restriction on the search folder.  This condition is expected and handled.");
						}
						catch (ObjectNotInitializedException)
						{
							flag = false;
							AirSyncDiagnostics.TraceDebug(ExTraceGlobals.AlgorithmTracer, this, "Search folder has null search criteria, reuseExistingSearchFolderCriteria = false;");
						}
						if (flag)
						{
							if (searchFolderCriteria.FolderScope == null || searchFolderCriteria.FolderScope.Length != this.folderScope.Length)
							{
								if (searchFolderCriteria.FolderScope == null)
								{
									AirSyncDiagnostics.TraceDebug(ExTraceGlobals.AlgorithmTracer, this, "searchCriteria.FolderScope is null.");
								}
								else
								{
									AirSyncDiagnostics.TraceDebug<int>(ExTraceGlobals.AlgorithmTracer, this, "searchCriteria.FolderScope is not null and searchCriteria.FolderScope.Length is {0}", searchFolderCriteria.FolderScope.Length);
								}
								flag = false;
								AirSyncDiagnostics.TraceDebug(ExTraceGlobals.AlgorithmTracer, this, "therefore reuseExistingSearchFolderCriteria = false");
							}
							else
							{
								for (int i = 0; i < this.folderScope.Length; i++)
								{
									bool flag2 = false;
									for (int j = 0; j < searchFolderCriteria.FolderScope.Length; j++)
									{
										if (searchFolderCriteria.FolderScope[j].Equals(this.folderScope[i]))
										{
											flag2 = true;
											break;
										}
									}
									if (!flag2)
									{
										flag = false;
										AirSyncDiagnostics.TraceDebug<StoreId>(ExTraceGlobals.AlgorithmTracer, this, "foundIt == false for this.folderScope[idx1] == {0}, therefore reuseExistingSearchFolderCriteria = false", this.folderScope[i]);
										break;
									}
									AirSyncDiagnostics.TraceDebug<StoreId, bool>(ExTraceGlobals.AlgorithmTracer, this, "foundIt == true for this.folderScope[idx1] == {0}, therefore reuseExistingSearchFolderCriteria stays at value: {1}", this.folderScope[i], flag);
								}
							}
						}
						if (flag)
						{
							string text2 = this.searchFolder.TryGetProperty(MailboxSearchProvider.SearchFolderTextDescription) as string;
							if (searchFolderCriteria.SearchQuery == null)
							{
								AirSyncDiagnostics.TraceDebug(ExTraceGlobals.AlgorithmTracer, this, "searchCriteria.SearchQuery == null, therefore reuseExistingSearchFolderCriteria = false");
								flag = false;
							}
							else if (text2 != this.queryFilter.ToString())
							{
								AirSyncDiagnostics.TraceDebug<string, QueryFilter>(ExTraceGlobals.AlgorithmTracer, this, "oldSearchFilter == {0}, and this.queryFilter.ToString() == {1}, therefore reuseExistingSearchFolderCriteria = false", text2, this.queryFilter);
								flag = false;
							}
							else if (searchFolderCriteria.DeepTraversal != this.deepTraversal)
							{
								AirSyncDiagnostics.TraceDebug<bool, bool>(ExTraceGlobals.AlgorithmTracer, this, "searchCriteria.DeepTraversal == {0}, and this.deepTraversal == {1}, therefore reuseExistingSearchFolderCriteria = false", searchFolderCriteria.DeepTraversal, this.deepTraversal);
								flag = false;
							}
						}
					}
					else
					{
						this.searchFolder = SearchFolder.Create(this.mailboxSession, folder.Id.ObjectId, text, CreateMode.OpenIfExists);
						AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.AlgorithmTracer, this, "Created a new search folder with display name: {0}", text);
						this.searchFolder.Save();
						this.searchFolder.Load(new PropertyDefinition[]
						{
							FolderSchema.ItemCount,
							FolderSchema.UnreadCount,
							FolderSchema.SearchFolderItemCount
						});
						flag = false;
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.AlgorithmTracer, this, "Search folder was just recreated, therefore reuseExistingSearchFolderCriteria = false");
					}
					if (!flag)
					{
						searchFolderCriteria = new SearchFolderCriteria(this.queryFilter, this.folderScope);
						searchFolderCriteria.DeepTraversal = this.deepTraversal;
						searchFolderCriteria.MaximumResultsCount = new int?(GlobalSettings.MaxMailboxSearchResults);
						AirSyncDiagnostics.TraceDebug<QueryFilter, int, bool>(ExTraceGlobals.AlgorithmTracer, this, "Created search criteria on search folder with queryFilter=={0}, folderScope.Length=={1}, deepTraversal=={2}", this.queryFilter, this.folderScope.Length, this.deepTraversal);
						Stopwatch stopwatch = Stopwatch.StartNew();
						IAsyncResult asyncResult = this.searchFolder.BeginApplyOneTimeSearch(searchFolderCriteria, null, null);
						int millisecondsTimeout = this.mailboxSession.Mailbox.IsContentIndexingEnabled ? this.maximumSearchTime : this.maximumSearchTimeNoContentIndexing;
						bool flag3 = asyncResult.AsyncWaitHandle.WaitOne(millisecondsTimeout, false);
						stopwatch.Stop();
						Command.CurrentCommand.Context.ProtocolLogger.SetValue(ProtocolLoggerData.SearchQueryTime, (int)stopwatch.ElapsedMilliseconds);
						if (!flag3)
						{
							AirSyncDiagnostics.TraceDebug(ExTraceGlobals.AlgorithmTracer, this, "MailboxSearchProvider.Execute. Search timed out.");
							throw new SearchTimeOutException();
						}
						this.searchFolder[MailboxSearchProvider.SearchFolderTextDescription] = ((this.queryFilter == null) ? "null" : this.queryFilter.ToString());
						this.searchFolder.EndApplyOneTimeSearch(asyncResult);
						this.searchFolder.Save();
						this.searchFolder.Load(new PropertyDefinition[]
						{
							FolderSchema.ItemCount,
							FolderSchema.UnreadCount,
							FolderSchema.SearchFolderItemCount
						});
					}
				}
			}
			catch (SearchTimeOutException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_InvalidSyncKey, innerException, false)
				{
					ErrorStringForProtocolLogger = "GeneralMbxSearchError"
				};
			}
			finally
			{
				if (this.folderIdMappingSyncState != null)
				{
					this.folderIdMappingSyncState.Dispose();
				}
				this.folderIdMappingSyncState = null;
				if (folder != null)
				{
					folder.Dispose();
				}
				folder = null;
			}
		}

		public void ParseMailboxSearchOptions(XmlElement optionsNode)
		{
			foreach (object obj in optionsNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if ("Search:" == xmlNode.NamespaceURI)
				{
					string name;
					if ((name = xmlNode.Name) != null)
					{
						if (name == "Range")
						{
							this.ParseRangeOption(xmlNode);
							continue;
						}
						if (name == "DeepTraversal")
						{
							this.deepTraversal = true;
							continue;
						}
						if (name == "RebuildResults")
						{
							this.forceResetSearchCriteria = true;
							continue;
						}
					}
					throw new SearchProtocolErrorException
					{
						ErrorStringForProtocolLogger = "BadNode(" + xmlNode.Name + ")MbxSearchOptions"
					};
				}
			}
		}

		public void ParseOptions(XmlElement optionsNode)
		{
			if (optionsNode != null)
			{
				this.ParseMailboxSearchOptions(optionsNode);
				this.mailboxSchemaOptions.Parse(optionsNode);
			}
		}

		public void ParseQueryNode(XmlElement queryNode)
		{
			this.searchCriteriaBuilder = new MailboxSearchCriteriaBuilder(this.context.Request.Culture);
			if (!this.mailboxSession.Mailbox.IsContentIndexingEnabled)
			{
				AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_ContentIndexingNotEnabled, "ContentIndexingNotEnabled", new string[]
				{
					this.mailboxSession.MailboxOwner.MailboxInfo.Location.ServerFqdn
				});
			}
			this.queryFilter = this.searchCriteriaBuilder.ParseTopLevelClassAndFolders(queryNode, this.mailboxSession.Mailbox.IsContentIndexingEnabled, this.versionFactory, this.context);
			this.schemaCache = this.searchCriteriaBuilder.SchemaCache;
			if (this.FolderIdMappingSyncState == null || !this.FolderIdMappingSyncState.Contains(CustomStateDatumType.FullFolderTree))
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.AlgorithmTracer, this, "FolderIdMappingSyncState error: {0}", (this.FolderIdMappingSyncState != null) ? "no FullFolderTree" : "FolderIdMappingSyncState is null");
				throw new SearchNeedToFolderSyncException();
			}
			List<string> list = this.searchCriteriaBuilder.FolderScope;
			FolderIdMapping folderIdMapping = (FolderIdMapping)this.FolderIdMappingSyncState[CustomStateDatumType.IdMapping];
			FolderTree folderTree = (FolderTree)this.FolderIdMappingSyncState[CustomStateDatumType.FullFolderTree];
			if (list.Count == 0)
			{
				this.folderScope = new StoreId[]
				{
					this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Root)
				};
			}
			else
			{
				int num = 0;
				this.folderScope = new StoreId[list.Count];
				foreach (string text in list)
				{
					StoreObjectId storeObjectId = null;
					if (text == "0")
					{
						storeObjectId = this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Root);
					}
					else
					{
						SyncCollection.CollectionTypes collectionType = AirSyncUtility.GetCollectionType(text);
						if (collectionType != SyncCollection.CollectionTypes.Mailbox && collectionType != SyncCollection.CollectionTypes.Unknown)
						{
							throw new AirSyncPermanentException(StatusCode.InvalidCombinationOfIDs, false)
							{
								ErrorStringForProtocolLogger = "BadIdComboInMbxSearch"
							};
						}
						MailboxSyncItemId mailboxSyncItemId = folderIdMapping[text] as MailboxSyncItemId;
						if (mailboxSyncItemId != null)
						{
							if (folderTree.IsSharedFolder(mailboxSyncItemId))
							{
								throw new AirSyncPermanentException(StatusCode.Sync_InvalidParameters, false)
								{
									ErrorStringForProtocolLogger = "AccessDeniedInMbxSearch"
								};
							}
							storeObjectId = (StoreObjectId)mailboxSyncItemId.NativeId;
						}
					}
					if (storeObjectId == null)
					{
						throw new SearchNeedToFolderSyncException();
					}
					try
					{
						using (Folder.Bind(this.mailboxSession, storeObjectId))
						{
						}
					}
					catch (ObjectNotFoundException)
					{
						throw new SearchNeedToFolderSyncException();
					}
					this.folderScope[num++] = storeObjectId;
				}
			}
			foreach (object obj in folderTree)
			{
				ISyncItemId syncItemId = (ISyncItemId)obj;
				MailboxSyncItemId mailboxSyncItemId2 = syncItemId as MailboxSyncItemId;
				if (mailboxSyncItemId2 != null && (folderTree.IsHidden(syncItemId) || folderTree.IsSharedFolder(syncItemId)))
				{
					this.searchCriteriaBuilder.ExcludedFolders.Add((StoreObjectId)mailboxSyncItemId2.NativeId);
				}
			}
			if (this.mailboxSchemaOptions.HasBodyPartPreferences)
			{
				if (this.searchCriteriaBuilder.Conversation == null)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.AlgorithmTracer, this, "<BodyPartPreference> is only supported when search for ConversationId");
					throw new AirSyncPermanentException(StatusCode.Sync_ObjectNotFound, false)
					{
						ErrorStringForProtocolLogger = "BodyPartNotSupported"
					};
				}
				this.searchCriteriaBuilder.Conversation.LoadBodySummaries();
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing)
			{
				if (this.searchFolder != null)
				{
					this.searchFolder.Dispose();
					this.searchFolder = null;
				}
				if (this.folderIdMappingSyncState != null)
				{
					this.folderIdMappingSyncState.Dispose();
					this.folderIdMappingSyncState = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxSearchProvider>(this);
		}

		protected void ParseRangeOption(XmlNode rangeNode)
		{
			string[] array = rangeNode.InnerText.Split(new char[]
			{
				'-'
			});
			this.minRange = int.Parse(array[0], CultureInfo.InvariantCulture);
			this.maxRange = int.Parse(array[1], CultureInfo.InvariantCulture);
			if (this.minRange > this.maxRange)
			{
				throw new SearchProtocolErrorException
				{
					ErrorStringForProtocolLogger = "MinMoreThanMaxInMailboxSearch"
				};
			}
			this.rangeSpecified = true;
		}

		private Item BindToItemWithItemClass(StoreObjectId id, string itemClass, PropertyDefinition[] properties)
		{
			Item result;
			if (ObjectClass.IsReport(itemClass))
			{
				result = ReportMessage.Bind(this.mailboxSession, id, properties);
			}
			else if (ObjectClass.IsMessage(itemClass, false))
			{
				result = MessageItem.Bind(this.mailboxSession, id, properties);
			}
			else if (ObjectClass.IsPost(itemClass))
			{
				result = PostItem.Bind(this.mailboxSession, id, properties);
			}
			else if (ObjectClass.IsCalendarItem(itemClass))
			{
				result = CalendarItem.Bind(this.mailboxSession, id, properties);
			}
			else if (ObjectClass.IsMeetingRequest(itemClass))
			{
				result = MeetingRequest.Bind(this.mailboxSession, id, properties);
			}
			else if (ObjectClass.IsMeetingResponse(itemClass))
			{
				result = MeetingResponse.Bind(this.mailboxSession, id, properties);
			}
			else if (ObjectClass.IsMeetingCancellation(itemClass))
			{
				result = MeetingCancellation.Bind(this.mailboxSession, id, properties);
			}
			else if (ObjectClass.IsContact(itemClass))
			{
				result = Contact.Bind(this.mailboxSession, id, properties);
			}
			else if (ObjectClass.IsTask(itemClass))
			{
				Task task = Task.Bind(this.mailboxSession, id, true, properties);
				task.SuppressRecurrenceAdjustment = true;
				result = task;
			}
			else if (ObjectClass.IsDistributionList(itemClass))
			{
				result = DistributionList.Bind(this.mailboxSession, id, properties);
			}
			else if (ObjectClass.IsGenericMessage(itemClass))
			{
				result = MessageItem.Bind(this.mailboxSession, id, properties);
			}
			else
			{
				result = Item.Bind(this.mailboxSession, id, properties);
			}
			return result;
		}

		private const int IdxId = 0;

		private const int IdxItemClass = 1;

		private const int IdxParentItemId = 2;

		private static readonly PropertyTagPropertyDefinition SearchFolderTextDescription = PropertyTagPropertyDefinition.CreateCustom("SearchFolderTextDescription", 2081161247U);

		private static readonly PropertyDefinition[] queryColumns = new PropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.ItemClass,
			StoreObjectSchema.ParentItemId
		};

		private static readonly SortBy[] displayNameSortBy = new SortBy[]
		{
			new SortBy(FolderSchema.DisplayName, SortOrder.Ascending)
		};

		private IAirSyncContext context;

		private bool deepTraversal;

		private SortBy[] defaultSortBy = new SortBy[]
		{
			new SortBy(StoreObjectSchema.CreationTime, SortOrder.Descending)
		};

		private CustomSyncState folderIdMappingSyncState;

		private StoreId[] folderScope;

		private bool forceResetSearchCriteria;

		private bool haveUnretrievableResults;

		private MailboxSchemaOptionsParser mailboxSchemaOptions = new MailboxSchemaOptionsParser();

		private MailboxSession mailboxSession;

		private int maximumSearchTime = (int)GlobalSettings.MailboxSearchTimeout.TotalMilliseconds;

		private int maximumSearchTimeNoContentIndexing = (int)GlobalSettings.MailboxSearchTimeoutNoContentIndexing.TotalMilliseconds;

		private int maxRange = GlobalSettings.MaxMailboxSearchResults;

		private int minRange;

		private int numberResponses;

		private QueryFilter queryFilter;

		private bool rangeSpecified;

		private Dictionary<string, MailboxSearchCriteriaBuilder.SchemaCacheItem> schemaCache;

		private SearchFolder searchFolder;

		private SyncStateStorage syncStateStorage;

		private int totalRowCount;

		private int uncappedResultCount;

		private IAirSyncVersionFactory versionFactory;

		private MailboxSearchCriteriaBuilder searchCriteriaBuilder;

		private enum BuildResponseState
		{
			Seeking,
			BuildingResponse,
			CountingTotal
		}
	}
}
