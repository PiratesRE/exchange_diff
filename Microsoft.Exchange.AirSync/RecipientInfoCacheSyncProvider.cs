using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class RecipientInfoCacheSyncProvider : ISyncProvider, IDisposeTrackable, IDisposable
	{
		public RecipientInfoCacheSyncProvider(MailboxSession mailboxSession, int maxEntries)
		{
			if (maxEntries <= 0)
			{
				throw new ArgumentException("MaxEntries must be a positive number");
			}
			this.recipientInfoCache = RecipientInfoCache.Create(mailboxSession, "OWA.AutocompleteCache");
			try
			{
				this.entryList = this.recipientInfoCache.Load("AutoCompleteCache");
				for (int i = this.entryList.Count - 1; i >= 0; i--)
				{
					if (string.IsNullOrEmpty(this.entryList[i].SmtpAddress) || (!string.Equals(this.entryList[i].RoutingType, "SMTP", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.entryList[i].RoutingType, "EX", StringComparison.OrdinalIgnoreCase)))
					{
						this.entryList.RemoveAt(i);
					}
				}
			}
			catch (CorruptDataException)
			{
				this.entryList = new List<RecipientInfoCacheEntry>(0);
				this.recipientInfoCache.Save(this.entryList, "AutoCompleteCache", 100);
			}
			this.maxNumEntries = maxEntries;
			this.disposeTracker = this.GetDisposeTracker();
		}

		public ISyncLogger SyncLogger
		{
			get
			{
				return AirSyncDiagnostics.GetSyncLogger();
			}
		}

		internal StoreObjectId ItemId
		{
			get
			{
				return this.recipientInfoCache.ItemId;
			}
		}

		private Dictionary<RecipientInfoCacheSyncItemId, RecipientInfoCacheSyncItem> FastCache
		{
			get
			{
				if (this.fastCacheNotDirectlyUsed == null)
				{
					this.fastCacheNotDirectlyUsed = new Dictionary<RecipientInfoCacheSyncItemId, RecipientInfoCacheSyncItem>(this.entryList.Count);
					foreach (RecipientInfoCacheEntry item in this.entryList)
					{
						RecipientInfoCacheSyncItem recipientInfoCacheSyncItem = RecipientInfoCacheSyncItem.Bind(item);
						this.fastCacheNotDirectlyUsed[(RecipientInfoCacheSyncItemId)recipientInfoCacheSyncItem.Id] = recipientInfoCacheSyncItem;
					}
				}
				return this.fastCacheNotDirectlyUsed;
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RecipientInfoCacheSyncProvider>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void BindToFolderSync(FolderSync folderSync)
		{
			this.CheckDisposed("BindToFolderSync");
		}

		public ISyncWatermark CreateNewWatermark()
		{
			this.CheckDisposed("CreateNewWatermark");
			return RecipientInfoCacheSyncWatermark.Create();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void DisposeNewOperationsCachedData()
		{
			this.CheckDisposed("DisposeNewOperationsCachedData");
		}

		public ISyncItem GetItem(ISyncItemId id, params PropertyDefinition[] specifiedPrefetchProperties)
		{
			this.CheckDisposed("GetItem");
			RecipientInfoCacheSyncItemId recipientInfoCacheSyncItemId = id as RecipientInfoCacheSyncItemId;
			if (recipientInfoCacheSyncItemId == null)
			{
				return null;
			}
			if (this.FastCache.ContainsKey(recipientInfoCacheSyncItemId))
			{
				return this.FastCache[recipientInfoCacheSyncItemId];
			}
			return null;
		}

		public ISyncWatermark GetMaxItemWatermark(ISyncWatermark currentWatermark)
		{
			this.CheckDisposed("GetMaxItemWatermark");
			if (this.entryList.Count <= this.maxNumEntries)
			{
				return RecipientInfoCacheSyncWatermark.Create(this.entryList, this.recipientInfoCache.LastModifiedTime);
			}
			if (!this.entryListIsSorted)
			{
				this.entryList.Sort();
				this.entryListIsSorted = true;
			}
			return RecipientInfoCacheSyncWatermark.Create(this.entryList.GetRange(this.entryList.Count - this.maxNumEntries, this.maxNumEntries), this.recipientInfoCache.LastModifiedTime);
		}

		public bool GetNewOperations(ISyncWatermark minSyncWatermark, ISyncWatermark maxSyncWatermark, bool enumerateDeletes, int numOperations, QueryFilter filter, Dictionary<ISyncItemId, ServerManifestEntry> newServerManifest)
		{
			this.CheckDisposed("GetNewOperations");
			AirSyncDiagnostics.TraceInfo<int>(ExTraceGlobals.SyncTracer, this, "RecipientInfoCacheSyncProvider.GetNewOperations. numOperations = {0}", numOperations);
			if (newServerManifest == null)
			{
				throw new ArgumentNullException("newServerManifest");
			}
			if (!enumerateDeletes)
			{
				throw new NotImplementedException("enumerateDeletes is false!");
			}
			if (filter != null)
			{
				throw new NotImplementedException("filter is non-null! Filters are not supported on RecipientInfoCacheSyncProvider");
			}
			return this.ComputeNewItems(minSyncWatermark as RecipientInfoCacheSyncWatermark, maxSyncWatermark as RecipientInfoCacheSyncWatermark, numOperations, newServerManifest);
		}

		public OperationResult DeleteItems(params ISyncItemId[] syncIds)
		{
			return OperationResult.Failed;
		}

		public List<IConversationTreeNode> GetInFolderItemsForConversation(ConversationId conversationId)
		{
			this.CheckDisposed("GetInFolderItemsForConversation");
			return null;
		}

		public ISyncItemId CreateISyncItemIdForNewItem(StoreObjectId itemId)
		{
			this.CheckDisposed("CreateISyncItemIdForNewItem");
			return null;
		}

		private bool ComputeNewItems(RecipientInfoCacheSyncWatermark minWatermark, RecipientInfoCacheSyncWatermark maxWatermark, int numOperations, Dictionary<ISyncItemId, ServerManifestEntry> newServerManifest)
		{
			if (minWatermark == null)
			{
				throw new ArgumentNullException("minWatermark");
			}
			if (maxWatermark == null)
			{
				maxWatermark = (RecipientInfoCacheSyncWatermark)this.GetMaxItemWatermark(null);
			}
			List<RecipientInfoCacheSyncItemId> list = new List<RecipientInfoCacheSyncItemId>(minWatermark.Entries.Count);
			bool flag = false;
			foreach (KeyValuePair<RecipientInfoCacheSyncItemId, long> keyValuePair in minWatermark.Entries)
			{
				if (!maxWatermark.Entries.ContainsKey(keyValuePair.Key))
				{
					if (numOperations != -1 && newServerManifest.Count >= numOperations)
					{
						flag = true;
						break;
					}
					ServerManifestEntry serverManifestEntry = new ServerManifestEntry(keyValuePair.Key);
					serverManifestEntry.ChangeType = ChangeType.Delete;
					newServerManifest[keyValuePair.Key] = serverManifestEntry;
					list.Add(keyValuePair.Key);
				}
			}
			foreach (RecipientInfoCacheSyncItemId key in list)
			{
				minWatermark.Entries.Remove(key);
			}
			if (flag)
			{
				return true;
			}
			foreach (KeyValuePair<RecipientInfoCacheSyncItemId, long> keyValuePair2 in maxWatermark.Entries)
			{
				bool flag2 = !minWatermark.Entries.ContainsKey(keyValuePair2.Key) || minWatermark.Entries[keyValuePair2.Key].CompareTo(keyValuePair2.Value) != 0;
				if (flag2)
				{
					if (numOperations != -1 && newServerManifest.Count >= numOperations)
					{
						return true;
					}
					minWatermark.Entries[keyValuePair2.Key] = keyValuePair2.Value;
					ServerManifestEntry serverManifestEntry2 = new ServerManifestEntry(keyValuePair2.Key);
					serverManifestEntry2.ChangeType = ChangeType.Add;
					Dictionary<RecipientInfoCacheSyncItemId, long> dictionary = new Dictionary<RecipientInfoCacheSyncItemId, long>(1);
					dictionary[keyValuePair2.Key] = keyValuePair2.Value;
					serverManifestEntry2.Watermark = RecipientInfoCacheSyncWatermark.Create(dictionary, maxWatermark.LastModifiedTime);
					newServerManifest[keyValuePair2.Key] = serverManifestEntry2;
				}
			}
			minWatermark.LastModifiedTime = maxWatermark.LastModifiedTime;
			return false;
		}

		private void CheckDisposed(string methodName)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				this.disposed = true;
				this.InternalDispose(disposing);
			}
		}

		private void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.recipientInfoCache != null)
				{
					this.recipientInfoCache.Dispose();
					this.recipientInfoCache = null;
				}
				if (this.fastCacheNotDirectlyUsed != null)
				{
					foreach (RecipientInfoCacheSyncItem recipientInfoCacheSyncItem in this.fastCacheNotDirectlyUsed.Values)
					{
						recipientInfoCacheSyncItem.Dispose();
					}
					this.fastCacheNotDirectlyUsed.Clear();
					this.fastCacheNotDirectlyUsed = null;
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
		}

		private readonly DisposeTracker disposeTracker;

		private int maxNumEntries = int.MaxValue;

		private bool disposed;

		private RecipientInfoCache recipientInfoCache;

		private List<RecipientInfoCacheEntry> entryList;

		private bool entryListIsSorted;

		private Dictionary<RecipientInfoCacheSyncItemId, RecipientInfoCacheSyncItem> fastCacheNotDirectlyUsed;
	}
}
