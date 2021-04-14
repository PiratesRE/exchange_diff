using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class Pool<TItem> where TItem : class
	{
		internal Pool(int capacity, int maxCapacity, TimeSpan expiryInterval) : this(capacity, maxCapacity, true, expiryInterval, ContentAggregationConfig.PoolExpiryCheckInterval)
		{
		}

		internal Pool(int capacity, int maxCapacity, bool assertOnMaxCapacity, TimeSpan expiryInterval, TimeSpan expiryCheckInterval)
		{
			this.itemsAvailable = new Queue<PoolItem<TItem>>(capacity);
			this.itemsInUse = new Dictionary<uint, PoolItem<TItem>>(maxCapacity);
			this.maxCapacity = maxCapacity;
			this.assertOnMaxCapacity = assertOnMaxCapacity;
			this.maxNumberOfAttemptsBeforePoolBackOff = ContentAggregationConfig.MaxNumberOfAttemptsBeforePoolBackOff;
			this.BackOffInterval = ContentAggregationConfig.PoolBackOffTimeInterval;
			this.expiryInterval = expiryInterval;
			this.expiryTimer = new GuardedTimer(new TimerCallback(this.ExpireUnusedItems), null, expiryCheckInterval, expiryCheckInterval);
			this.syncLogSession = this.GetSyncLogSession();
		}

		internal int ItemsAvailableCount
		{
			get
			{
				int count;
				lock (this.syncRoot)
				{
					count = this.itemsAvailable.Count;
				}
				return count;
			}
		}

		internal int ItemsInUseCount
		{
			get
			{
				int count;
				lock (this.syncRoot)
				{
					count = this.itemsInUse.Count;
				}
				return count;
			}
		}

		private bool MaximumCapacityReached
		{
			get
			{
				bool result;
				lock (this.syncRoot)
				{
					result = (this.itemsInUse.Count + this.itemsAvailable.Count >= this.maxCapacity);
				}
				return result;
			}
		}

		internal void AddDiagnosticInfoTo(XElement parentElement)
		{
			lock (this.syncRoot)
			{
				XElement xelement = new XElement("ItemsInPool");
				xelement.Add(new XElement("count", this.ItemsAvailableCount));
				foreach (PoolItem<TItem> poolItem in this.itemsAvailable)
				{
					XElement xelement2 = new XElement("Item");
					xelement2.Add(new XElement("poolItemId", poolItem.ID));
					xelement2.Add(new XElement("creationTime", poolItem.CreationTime.ToString("o")));
					xelement2.Add(new XElement("lastUsedTime", poolItem.LastUsedTime.ToString("o")));
					xelement.Add(xelement2);
				}
				parentElement.Add(xelement);
				XElement xelement3 = new XElement("ItemsInUse");
				xelement3.Add(new XElement("count", this.ItemsInUseCount));
				foreach (PoolItem<TItem> poolItem2 in this.itemsInUse.Values)
				{
					XElement xelement4 = new XElement("Item");
					xelement4.Add(new XElement("poolItemId", poolItem2.ID));
					xelement4.Add(new XElement("creationTime", poolItem2.CreationTime.ToString("o")));
					xelement4.Add(new XElement("lastUsedTime", poolItem2.LastUsedTime.ToString("o")));
					xelement3.Add(xelement4);
				}
				parentElement.Add(xelement3);
			}
		}

		internal void Shutdown()
		{
			lock (this.syncRoot)
			{
				this.shuttingDown = true;
				while (this.itemsAvailable.Count > 0)
				{
					PoolItem<TItem> poolItem = this.itemsAvailable.Dequeue();
					this.DestroyItem(poolItem.Item);
				}
				if (this.expiryTimer != null)
				{
					this.expiryTimer.Dispose(false);
					this.expiryTimer = null;
				}
			}
		}

		internal PoolItem<TItem> GetItem(out bool needsBackOff)
		{
			needsBackOff = false;
			PoolItem<TItem> poolItem;
			lock (this.syncRoot)
			{
				if (this.shuttingDown)
				{
					this.syncLogSession.LogVerbose((TSLID)91UL, Guid.Empty, null, "We're shutting down, so we won't create a new item.", new object[0]);
					return null;
				}
				if (this.itemsAvailable.Count > 0)
				{
					poolItem = this.itemsAvailable.Dequeue();
				}
				else
				{
					if (this.MaximumCapacityReached)
					{
						this.syncLogSession.LogError((TSLID)92UL, Guid.Empty, null, "Maximum capacity for resource pool is reached. A resource leak is very likely to be happening.", new object[0]);
						StackTrace stackTrace = new StackTrace();
						this.LogEvent(TransportSyncManagerEventLogConstants.Tuple_SyncManagerResourcePoolLimitReached, null, new object[]
						{
							this.maxCapacity,
							stackTrace.ToString()
						});
						needsBackOff = true;
						return null;
					}
					ExDateTime utcNow = ExDateTime.UtcNow;
					if (this.poolInBackOffMode)
					{
						if (utcNow - this.backOffStartedTime < this.BackOffInterval)
						{
							needsBackOff = true;
							this.syncLogSession.LogError((TSLID)93UL, Guid.Empty, null, "Failed to create new item in the pool as we're backing off.", new object[0]);
							return null;
						}
						this.backOffStartedTime = ExDateTime.MinValue;
						this.poolInBackOffMode = false;
						this.failureCount = 0;
					}
					bool flag2 = false;
					TItem titem = this.CreateItem(out flag2);
					if (flag2)
					{
						if ((int)(this.failureCount += 1) >= this.maxNumberOfAttemptsBeforePoolBackOff)
						{
							this.poolInBackOffMode = (needsBackOff = true);
							this.backOffStartedTime = utcNow;
						}
					}
					else
					{
						this.backOffStartedTime = ExDateTime.MinValue;
						this.poolInBackOffMode = (needsBackOff = false);
						this.failureCount = 0;
					}
					if (titem == null)
					{
						this.syncLogSession.LogError((TSLID)94UL, Guid.Empty, null, "Failed to create new item in the pool.", new object[0]);
						return null;
					}
					TItem item = titem;
					uint num = Pool<TItem>.currentId;
					Pool<TItem>.currentId = num + 1U;
					poolItem = new PoolItem<TItem>(item, num);
				}
				this.itemsInUse.Add(poolItem.ID, poolItem);
			}
			poolItem.LastUsedTime = ExDateTime.UtcNow;
			return poolItem;
		}

		internal void ReturnItem(PoolItem<TItem> poolItem)
		{
			this.ReturnItem(poolItem, true);
		}

		internal void ReturnItem(PoolItem<TItem> poolItem, bool reuse)
		{
			SyncUtilities.ThrowIfArgumentNull("item", poolItem);
			lock (this.syncRoot)
			{
				this.itemsInUse.Remove(poolItem.ID);
				if (this.shuttingDown || !reuse || this.MaximumCapacityReached)
				{
					this.DestroyItem(poolItem.Item);
					poolItem = null;
				}
				else
				{
					poolItem.LastUsedTime = ExDateTime.UtcNow;
					this.itemsAvailable.Enqueue(poolItem);
				}
			}
		}

		protected virtual GlobalSyncLogSession GetSyncLogSession()
		{
			return ContentAggregationConfig.SyncLogSession;
		}

		protected virtual bool LogEvent(ExEventLog.EventTuple eventTuple, string periodicKey, params object[] messageArgs)
		{
			return ContentAggregationConfig.EventLogger.LogEvent(eventTuple, periodicKey, messageArgs);
		}

		protected abstract void DestroyItem(TItem item);

		protected abstract TItem CreateItem(out bool needsBackOff);

		private void ExpireUnusedItems(object state)
		{
			lock (this.syncRoot)
			{
				if (!this.shuttingDown)
				{
					int count = this.itemsAvailable.Count;
					for (int i = 0; i < count; i++)
					{
						PoolItem<TItem> poolItem = this.itemsAvailable.Dequeue();
						if (ExDateTime.UtcNow - poolItem.LastUsedTime >= this.expiryInterval)
						{
							this.DestroyItem(poolItem.Item);
						}
						else
						{
							this.itemsAvailable.Enqueue(poolItem);
						}
					}
				}
			}
		}

		private readonly GlobalSyncLogSession syncLogSession;

		private readonly int maxNumberOfAttemptsBeforePoolBackOff;

		private readonly TimeSpan BackOffInterval;

		private readonly object syncRoot = new object();

		private readonly Queue<PoolItem<TItem>> itemsAvailable;

		private readonly Dictionary<uint, PoolItem<TItem>> itemsInUse;

		private readonly int maxCapacity;

		private readonly bool assertOnMaxCapacity;

		private static uint currentId;

		private bool shuttingDown;

		private byte failureCount;

		private bool poolInBackOffMode;

		private ExDateTime backOffStartedTime;

		private TimeSpan expiryInterval;

		private GuardedTimer expiryTimer;
	}
}
