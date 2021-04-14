using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.InfoWorker.Common.ELC;

namespace Microsoft.Exchange.InfoWorker.Common.Search
{
	internal class SearchResultProcessor
	{
		public SearchResultProcessor(MailboxSession source, MailboxSession target, StoreId targetRootId, string[] targetSubfolders, List<SearchMailboxAction> actions, ref HashSet<StoreObjectId> unsearchableItemSet, SearchCommunicator communicator, SearchMailboxWorker worker)
		{
			if (worker == null || source == null || communicator == null)
			{
				return;
			}
			this.searchWorker = worker;
			this.searchCommunicator = communicator;
			this.sourceMailbox = source;
			this.targetMailbox = target;
			this.searchActions = actions;
			this.unsearchableItemSet = unsearchableItemSet;
			this.sourceThrottler = new ResponseThrottler(this.searchCommunicator.AbortEvent);
			this.targetThrottler = new ResponseThrottler(this.searchCommunicator.AbortEvent);
			this.InitializeFolderMap(targetRootId, targetSubfolders);
		}

		internal void Process(Folder[] folders)
		{
			double mailboxProgress = this.searchWorker.MailboxProgress;
			int totalItemCount = folders.Aggregate(0, (int s, Folder f) => s + f.ItemCount) + ((this.unsearchableItemSet == null) ? 0 : this.unsearchableItemSet.Count);
			int num = 0;
			int num2 = 0;
			while (num2 < folders.Length && !this.searchCommunicator.IsAborted)
			{
				double maxProgress = this.CalcProgress(num + folders[num2].ItemCount, totalItemCount, mailboxProgress, 100.0);
				this.ProcessFolderItems(folders[num2], this.sourceMailbox, this.targetMailbox, maxProgress);
				num += folders[num2].ItemCount;
				num2++;
			}
			this.AfterEnumerate();
		}

		internal void ReportActionException(Exception e)
		{
			this.errorDuringAction = true;
			this.searchWorker.ReportActionException(e);
		}

		internal bool IsAborted()
		{
			return this.searchCommunicator.IsAborted;
		}

		internal void ReportLogs(StreamLogItem.LogItem logItem)
		{
			lock (this.searchCommunicator)
			{
				this.searchCommunicator.ReportLogs(logItem);
			}
		}

		internal string GetSourceUserName()
		{
			return this.searchWorker.SourceUser.Id.DistinguishedName;
		}

		internal void BackOffFromSourceStore()
		{
			if (this.sourceMailbox != null)
			{
				this.sourceThrottler.BackOffFromStore(this.sourceMailbox);
			}
		}

		internal void BackOffFromTargetStore()
		{
			if (this.targetMailbox != null)
			{
				this.targetThrottler.BackOffFromStore(this.targetMailbox);
			}
		}

		internal int WorkerId
		{
			get
			{
				return this.searchWorker.WorkerId;
			}
		}

		private void ProcessFolderItems(Folder folder, MailboxSession sourceMailbox, MailboxSession targetMailbox, double maxProgress)
		{
			double mailboxProgress = this.searchWorker.MailboxProgress;
			int resultItemsCount = this.searchWorker.SearchResult.ResultItemsCount;
			using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, new SortBy[]
			{
				new SortBy(StoreObjectSchema.ParentEntryId, SortOrder.Ascending)
			}, SearchResultProcessor.ItemPreloadProperties))
			{
				while (!this.searchCommunicator.IsAborted)
				{
					this.BackOffFromSourceStore();
					object[][] rows = queryResult.GetRows(this.batchedItemBuffer.Length);
					if (rows == null || rows.Length <= 0)
					{
						break;
					}
					for (int i = 0; i < rows.Length; i++)
					{
						this.ProcessSingleResult(rows[i]);
						this.searchWorker.SearchResult.ResultItemsCount++;
						StoreId storeId = (StoreId)rows[i][0];
						StoreObjectId storeObjectId = StoreId.GetStoreObjectId(storeId);
						if (this.unsearchableItemSet != null && storeObjectId != null && this.unsearchableItemSet.Contains(storeObjectId))
						{
							this.unsearchableItemSet.Remove(storeObjectId);
						}
						if (SearchResultProcessor.PropertyExists(rows[i][1]))
						{
							this.searchWorker.SearchResult.ResultItemsSize += (int)rows[i][1];
						}
						else
						{
							SearchResultProcessor.Tracer.TraceDebug<StoreId>((long)this.GetHashCode(), "Unable to retrieve message size for message {0}", storeId);
						}
						int num = this.searchWorker.SearchResult.ResultItemsCount - resultItemsCount;
						double progress = this.CalcProgress(num, Math.Max(queryResult.EstimatedRowCount, num), mailboxProgress, maxProgress);
						this.UpdateProgress(progress, 10.0);
					}
				}
			}
		}

		public void AfterEnumerate()
		{
			if (this.fetchedItemCount > 0)
			{
				this.ProcessCurrentBatch();
			}
		}

		internal void CheckTargetMailboxAvailableSpace()
		{
			Unlimited<ByteQuantifiedSize> availableSpace = this.GetAvailableSpace();
			ByteQuantifiedSize value = this.CalculateBatchSize(this.batchedItemBuffer, this.fetchedItemCount);
			if (SearchResultProcessor.MinimumSpaceRequired > value)
			{
				value = SearchResultProcessor.MinimumSpaceRequired;
			}
			if (!availableSpace.IsUnlimited && availableSpace.Value < value)
			{
				throw new SearchMailboxException(Strings.TargetMailboxOutOfSpace);
			}
		}

		private void ProcessSingleResult(object[] result)
		{
			if (SearchResultProcessor.PropertyExists(result[0]) && SearchResultProcessor.PropertyExists(result[2]))
			{
				StoreId storeId = (StoreId)result[0];
				StoreId id = (StoreId)result[2];
				if (this.currentFolderId == null)
				{
					this.currentFolderId = id;
				}
				if (this.fetchedItemCount >= this.batchedItemBuffer.Length || !this.currentFolderId.Equals(id))
				{
					this.ProcessCurrentBatch();
					this.fetchedItemCount = 0;
					this.currentFolderId = id;
					if (this.searchCommunicator.IsAborted)
					{
						return;
					}
				}
				this.batchedItemBuffer[this.fetchedItemCount++] = result;
				return;
			}
			SearchResultProcessor.Tracer.TraceDebug((long)this.GetHashCode(), "Item is skipped because the itemId or ParentItemId is unavailable");
		}

		public void InitializeFolderMap(StoreId targetRootId, string[] targetSubfolders)
		{
			StoreId defaultFolderId = this.sourceMailbox.GetDefaultFolderId(DefaultFolderType.Root);
			StoreId defaultFolderId2 = this.sourceMailbox.GetDefaultFolderId(DefaultFolderType.RecoverableItemsRoot);
			FolderNode folderNode2;
			if (this.targetMailbox != null)
			{
				FolderNode folderNode = new FolderNode(null, targetRootId, null, null);
				folderNode2 = folderNode;
				foreach (string displayName in targetSubfolders)
				{
					FolderNode folderNode3 = new FolderNode(null, null, displayName, folderNode2);
					folderNode2 = folderNode3;
				}
			}
			else
			{
				folderNode2 = new FolderNode(defaultFolderId, null, Strings.PrimaryMailbox, null);
			}
			FolderNode folderNode4 = folderNode2;
			folderNode4.SourceFolderId = defaultFolderId;
			this.folderNodeMap = new Dictionary<StoreId, FolderNode>();
			this.folderNodeMap.Add(folderNode4.SourceFolderId, folderNode4);
			string displayName2 = null;
			if (this.searchWorker.SearchDumpster || this.searchWorker.IncludeUnsearchableItems)
			{
				string[] uniqueFolderName = this.GetUniqueFolderName(this.sourceMailbox, defaultFolderId, new string[]
				{
					Strings.RecoverableItems,
					Strings.Unsearchable
				});
				displayName2 = uniqueFolderName[0];
				string text = uniqueFolderName[1];
			}
			if (this.searchWorker.SearchDumpster && defaultFolderId2 != null)
			{
				this.folderNodeMap.Add(defaultFolderId2, new FolderNode(defaultFolderId2, null, displayName2, folderNode4));
			}
		}

		private string[] GetUniqueFolderName(MailboxSession mailboxStore, StoreId folderId, string[] suggestedNames)
		{
			List<string> subFolderNames = new List<string>();
			using (Folder folder = Folder.Bind(mailboxStore, folderId))
			{
				using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, null, null, new PropertyDefinition[]
				{
					FolderSchema.DisplayName
				}))
				{
					ElcMailboxHelper.ForeachQueryResult(queryResult, delegate(object[] rowProps, ref bool breakLoop)
					{
						if (SearchResultProcessor.PropertyExists(rowProps[0]))
						{
							subFolderNames.Add((string)rowProps[0]);
						}
					});
				}
			}
			string[] array = new string[suggestedNames.Length];
			for (int i = 0; i < suggestedNames.Length; i++)
			{
				string folderName = suggestedNames[i];
				List<string> list = (from x in subFolderNames
				where x.StartsWith(folderName, StringComparison.OrdinalIgnoreCase)
				select x).ToList<string>();
				for (int j = 0; j < list.Count + 1; j++)
				{
					if (list.Find((string x) => x.Equals(folderName, StringComparison.OrdinalIgnoreCase)) == null)
					{
						break;
					}
					folderName = string.Format("{0}-{1}", suggestedNames[i], j + 1);
				}
				array[i] = folderName;
			}
			return array;
		}

		private void ProcessCurrentBatch()
		{
			this.UpdateFolderMap(this.currentFolderId);
			this.errorDuringAction = false;
			foreach (SearchMailboxAction searchMailboxAction in this.searchActions)
			{
				if (!this.errorDuringAction)
				{
					searchMailboxAction.PerformBatchOperation(this.batchedItemBuffer, this.fetchedItemCount, this.currentFolderId, this.sourceMailbox, this.targetMailbox, this.folderNodeMap, this);
				}
			}
			this.errorDuringAction = false;
		}

		private void UpdateFolderMap(StoreId sourceFolderId)
		{
			Stack<FolderNode> stack = new Stack<FolderNode>();
			StoreId storeId = sourceFolderId;
			while (!this.folderNodeMap.ContainsKey(storeId))
			{
				using (Folder folder = Folder.Bind(this.sourceMailbox, storeId, new PropertyDefinition[]
				{
					StoreObjectSchema.ParentItemId,
					FolderSchema.DisplayName,
					StoreObjectSchema.IsSoftDeleted
				}))
				{
					bool valueOrDefault = folder.GetValueOrDefault<bool>(StoreObjectSchema.IsSoftDeleted);
					stack.Push(new FolderNode(storeId, null, folder.DisplayName, valueOrDefault, null));
					if (storeId.Equals(folder.ParentId))
					{
						throw new CorruptDataException(Strings.CorruptedFolder(this.sourceMailbox.MailboxOwner.MailboxInfo.DisplayName));
					}
					storeId = folder.ParentId;
				}
			}
			FolderNode parent = this.folderNodeMap[storeId];
			while (stack.Count > 0)
			{
				FolderNode folderNode = stack.Pop();
				folderNode.Parent = parent;
				this.folderNodeMap.Add(folderNode.SourceFolderId, folderNode);
				parent = folderNode;
			}
		}

		private static bool PropertyExists(object property)
		{
			return property != null && !(property is PropertyError);
		}

		private double CalcProgress(int copiedItemCount, int totalItemCount, double startProgress, double maxProgress)
		{
			if (totalItemCount == 0)
			{
				return maxProgress;
			}
			double val = startProgress + (maxProgress - startProgress) * (double)copiedItemCount / (double)totalItemCount;
			return Math.Min(Math.Min(val, startProgress), maxProgress);
		}

		private void UpdateProgress(double progress, double minstep)
		{
			if (progress >= this.searchWorker.MailboxProgress + minstep)
			{
				this.UpdateProgress(progress);
			}
		}

		private void UpdateProgress(double progress)
		{
			this.searchWorker.MailboxProgress = progress;
			lock (this.searchCommunicator)
			{
				this.searchCommunicator.UpdateProgress(this.searchWorker);
			}
		}

		private Unlimited<ByteQuantifiedSize> GetAvailableSpace()
		{
			if (this.targetMailbox == null)
			{
				return Unlimited<ByteQuantifiedSize>.UnlimitedValue;
			}
			this.targetMailbox.Mailbox.ForceReload(new PropertyDefinition[]
			{
				MailboxSchema.QuotaUsedExtended
			});
			ulong num = (ulong)((long)this.targetMailbox.Mailbox.TryGetProperty(MailboxSchema.QuotaUsedExtended));
			Unlimited<ByteQuantifiedSize> result;
			if (this.searchWorker.TargetMailboxQuota.IsUnlimited)
			{
				result = this.searchWorker.TargetMailboxQuota;
			}
			else if (this.searchWorker.TargetMailboxQuota.Value < new ByteQuantifiedSize(num))
			{
				result = new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.Zero);
			}
			else
			{
				result = this.searchWorker.TargetMailboxQuota - num;
			}
			return result;
		}

		private ByteQuantifiedSize CalculateBatchSize(object[][] batchedItemBuffer, int fetchedItemCount)
		{
			ByteQuantifiedSize byteQuantifiedSize = ByteQuantifiedSize.Zero;
			for (int i = 0; i < fetchedItemCount; i++)
			{
				if (SearchResultProcessor.PropertyExists(batchedItemBuffer[i][1]))
				{
					byteQuantifiedSize += (int)batchedItemBuffer[i][1];
				}
			}
			return byteQuantifiedSize;
		}

		private static readonly Trace Tracer = ExTraceGlobals.SearchTracer;

		private object[][] batchedItemBuffer = new object[ResponseThrottler.MaxBulkSize][];

		private static readonly ByteQuantifiedSize MinimumSpaceRequired = new ByteQuantifiedSize(1048576UL);

		private StoreId currentFolderId;

		private int fetchedItemCount;

		private Dictionary<StoreId, FolderNode> folderNodeMap;

		private MailboxSession sourceMailbox;

		private MailboxSession targetMailbox;

		private ResponseThrottler sourceThrottler;

		private ResponseThrottler targetThrottler;

		private SearchCommunicator searchCommunicator;

		private SearchMailboxWorker searchWorker;

		private bool errorDuringAction;

		private List<SearchMailboxAction> searchActions;

		private HashSet<StoreObjectId> unsearchableItemSet;

		private static readonly PropertyDefinition[] ItemPreloadProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			ItemSchema.Size,
			StoreObjectSchema.ParentItemId,
			ItemSchema.Subject,
			MessageItemSchema.IsRead,
			ItemSchema.SentTime,
			ItemSchema.ReceivedTime,
			ItemSchema.Sender,
			MessageItemSchema.SenderSmtpAddress
		};

		internal enum ItemPropertyIndex
		{
			Id,
			Size,
			ParentItemId,
			Subject,
			IsRead,
			SentTime,
			ReceivedTime,
			Sender,
			SenderSmtpAddress
		}
	}
}
