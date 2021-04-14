using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class SharedObjectPropertyBagDataCache : IComponentData
	{
		internal SharedObjectPropertyBagDataCache(MailboxState mailboxState, int capacity, TimeSpan timeToLive)
		{
			this.mailboxState = mailboxState;
			this.dataCache = new Dictionary<ExchangeId, SharedObjectPropertyBagData>(capacity);
			this.evictionPolicy = new LRU2WithTimeToLiveExpirationPolicy<ExchangeId>(capacity, timeToLive, false);
		}

		public MailboxState MailboxState
		{
			get
			{
				return this.mailboxState;
			}
		}

		internal static void Initialize()
		{
			if (SharedObjectPropertyBagDataCache.mailboxStateSlot == -1)
			{
				SharedObjectPropertyBagDataCache.mailboxStateSlot = MailboxState.AllocateComponentDataSlot(false);
			}
			Mailbox.RegisterOnDisconnectAction(delegate(Context context, Mailbox mailbox)
			{
				SharedObjectPropertyBagDataCache cacheForMailbox = SharedObjectPropertyBagDataCache.GetCacheForMailbox(mailbox.SharedState);
				cacheForMailbox.OnMailboxDisconnect(context, mailbox);
			});
		}

		internal static SharedObjectPropertyBagDataCache GetCacheForMailboxNoCreate(MailboxState mailboxState)
		{
			return (SharedObjectPropertyBagDataCache)mailboxState.GetComponentData(SharedObjectPropertyBagDataCache.mailboxStateSlot);
		}

		internal static SharedObjectPropertyBagDataCache GetCacheForMailbox(MailboxState mailboxState)
		{
			SharedObjectPropertyBagDataCache sharedObjectPropertyBagDataCache = SharedObjectPropertyBagDataCache.GetCacheForMailboxNoCreate(mailboxState);
			if (sharedObjectPropertyBagDataCache == null)
			{
				int capacity = (mailboxState.MailboxType == MailboxInfo.MailboxType.Private) ? ConfigurationSchema.DefaultMailboxSharedObjectPropertyBagDataCacheSize.Value : ConfigurationSchema.PublicFolderMailboxSharedObjectPropertyBagDataCacheSize.Value;
				sharedObjectPropertyBagDataCache = new SharedObjectPropertyBagDataCache(mailboxState, capacity, ConfigurationSchema.SharedObjectPropertyBagDataCacheTimeToLive.Value);
				try
				{
					mailboxState.AddReference();
					SharedObjectPropertyBagDataCache sharedObjectPropertyBagDataCache2 = (SharedObjectPropertyBagDataCache)mailboxState.CompareExchangeComponentData(SharedObjectPropertyBagDataCache.mailboxStateSlot, null, sharedObjectPropertyBagDataCache);
					if (sharedObjectPropertyBagDataCache2 != null)
					{
						sharedObjectPropertyBagDataCache = sharedObjectPropertyBagDataCache2;
					}
				}
				finally
				{
					mailboxState.ReleaseReference();
				}
			}
			return sharedObjectPropertyBagDataCache;
		}

		internal static DataRow LoadDataRow(Context context, bool newBag, Table table, bool writeThrough, ColumnValue[] initialValues)
		{
			if (newBag)
			{
				return Factory.CreateDataRow(context.Culture, context, table, writeThrough, initialValues);
			}
			return Factory.OpenDataRow(context.Culture, context, table, writeThrough, initialValues);
		}

		bool IComponentData.DoCleanup(Context context)
		{
			bool result;
			using (LockManager.Lock(this.dataCache, LockManager.LockType.LeafMonitorLock, context.Diagnostics))
			{
				List<ExchangeId> list = new List<ExchangeId>(this.dataCache.Count);
				foreach (KeyValuePair<ExchangeId, SharedObjectPropertyBagData> keyValuePair in this.dataCache)
				{
					if (!keyValuePair.Value.IsInUse)
					{
						list.Add(keyValuePair.Key);
					}
				}
				foreach (ExchangeId exchangeId in list)
				{
					if (ExTraceGlobals.FolderTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.FolderTracer.TraceDebug<ExchangeId>(0L, "Dropping unused folder {0} from cache", exchangeId);
					}
					SharedObjectPropertyBagData sharedData = this.dataCache[exchangeId];
					this.RemoveDataFromTheCache(sharedData, exchangeId);
				}
				result = (this.dataCache.Count == 0);
			}
			return result;
		}

		internal SharedObjectPropertyBagData LoadPropertyBagData(Context context, Mailbox mailbox, ExchangeId propertyBagId, bool newBag, Table table, bool writeThrough, params ColumnValue[] initialValues)
		{
			SharedObjectPropertyBagData sharedObjectPropertyBagData = null;
			if (!newBag)
			{
				using (LockManager.Lock(this.dataCache, LockManager.LockType.LeafMonitorLock, context.Diagnostics))
				{
					if (this.dataCache.TryGetValue(propertyBagId, out sharedObjectPropertyBagData))
					{
						if (sharedObjectPropertyBagData.DataRow != null && !sharedObjectPropertyBagData.DataRow.IsDead)
						{
							this.MarkAsActiveInTheCacheNoLock(sharedObjectPropertyBagData, propertyBagId);
							sharedObjectPropertyBagData.IncrementUsage();
							return sharedObjectPropertyBagData;
						}
						sharedObjectPropertyBagData = null;
					}
				}
			}
			DataRow dataRow = null;
			SharedObjectPropertyBagData sharedObjectPropertyBagData2 = null;
			SharedObjectPropertyBagData result;
			try
			{
				dataRow = SharedObjectPropertyBagDataCache.LoadDataRow(context, newBag, table, writeThrough, initialValues);
				using (LockManager.Lock(this.dataCache, LockManager.LockType.LeafMonitorLock, context.Diagnostics))
				{
					if (this.dataCache.TryGetValue(propertyBagId, out sharedObjectPropertyBagData))
					{
						if (sharedObjectPropertyBagData.DataRow == null || sharedObjectPropertyBagData.DataRow.IsDead)
						{
							if (sharedObjectPropertyBagData.DataRow != null)
							{
								sharedObjectPropertyBagData.DataRow.Dispose();
								sharedObjectPropertyBagData.DataRow = null;
							}
							sharedObjectPropertyBagData.DataRow = dataRow;
							dataRow = null;
						}
					}
					else
					{
						sharedObjectPropertyBagData2 = new SharedObjectPropertyBagData(context, mailbox, this, propertyBagId, dataRow);
						dataRow = null;
						this.dataCache.Add(propertyBagId, sharedObjectPropertyBagData2);
						sharedObjectPropertyBagData = sharedObjectPropertyBagData2;
						sharedObjectPropertyBagData2 = null;
					}
					this.MarkAsActiveInTheCacheNoLock(sharedObjectPropertyBagData, propertyBagId);
					sharedObjectPropertyBagData.IncrementUsage();
					result = sharedObjectPropertyBagData;
				}
			}
			finally
			{
				if (dataRow != null)
				{
					dataRow.Dispose();
				}
				if (sharedObjectPropertyBagData2 != null)
				{
					sharedObjectPropertyBagData2.Dispose();
				}
			}
			return result;
		}

		internal bool IsCleanupRequired()
		{
			bool result;
			using (LockManager.Lock(this.dataCache, LockManager.LockType.LeafMonitorLock))
			{
				result = (this.evictionPolicy.CountOfKeysToCleanup > ConfigurationSchema.SharedObjectPropertyBagDataCacheCleanupMultiplier.Value * this.evictionPolicy.Capacity);
			}
			return result;
		}

		internal void MarkAsActiveInTheCache(SharedObjectPropertyBagData sharedData, ExchangeId propertyBagId)
		{
			using (LockManager.Lock(this.dataCache, LockManager.LockType.LeafMonitorLock))
			{
				this.MarkAsActiveInTheCacheNoLock(sharedData, propertyBagId);
			}
		}

		internal void ReleasePropertyBagData(SharedObjectPropertyBagData sharedData, ExchangeId propertyBagId)
		{
			using (LockManager.Lock(this.dataCache, LockManager.LockType.LeafMonitorLock))
			{
				sharedData.DecrementUsage();
				if (!sharedData.IsInUse && (sharedData.DataRow == null || sharedData.DataRow.IsDead || sharedData.DataRow.IsDirty || sharedData.DataRow.IsNew))
				{
					this.RemoveDataFromTheCache(sharedData, propertyBagId);
				}
			}
		}

		private void MarkAsActiveInTheCacheNoLock(SharedObjectPropertyBagData sharedData, ExchangeId propertyBagId)
		{
			if (sharedData.IsActiveInTheCache)
			{
				this.evictionPolicy.KeyAccess(propertyBagId);
				return;
			}
			this.evictionPolicy.Insert(propertyBagId);
			sharedData.IsActiveInTheCache = true;
		}

		private void OnMailboxDisconnect(Context context, Mailbox mailbox)
		{
			if (!mailbox.SharedState.IsMailboxLockedExclusively())
			{
				return;
			}
			using (LockManager.Lock(this.dataCache, LockManager.LockType.LeafMonitorLock, context.Diagnostics))
			{
				this.evictionPolicy.EvictionCheckpoint();
				IList<ExchangeId> keysToCleanup = this.evictionPolicy.GetKeysToCleanup(true);
				for (int i = 0; i < keysToCleanup.Count; i++)
				{
					SharedObjectPropertyBagData sharedObjectPropertyBagData;
					if (this.dataCache.TryGetValue(keysToCleanup[i], out sharedObjectPropertyBagData))
					{
						if (object.ReferenceEquals(mailbox.DataRow, sharedObjectPropertyBagData.DataRow))
						{
							this.evictionPolicy.Insert(keysToCleanup[i]);
						}
						else
						{
							sharedObjectPropertyBagData.IsActiveInTheCache = false;
							if (ExTraceGlobals.FolderTracer.IsTraceEnabled(TraceType.DebugTrace))
							{
								ExTraceGlobals.FolderTracer.TraceDebug<ExchangeId>(0L, "Discarding folder {0} cache", keysToCleanup[i]);
							}
							sharedObjectPropertyBagData.DiscardCache(context);
						}
					}
				}
			}
		}

		private void RemoveDataFromTheCache(SharedObjectPropertyBagData sharedData, ExchangeId propertyBagId)
		{
			this.dataCache.Remove(propertyBagId);
			if (sharedData.IsActiveInTheCache)
			{
				this.evictionPolicy.Remove(propertyBagId);
			}
			sharedData.Dispose();
		}

		private static int mailboxStateSlot = -1;

		private MailboxState mailboxState;

		private Dictionary<ExchangeId, SharedObjectPropertyBagData> dataCache;

		private EvictionPolicy<ExchangeId> evictionPolicy;
	}
}
