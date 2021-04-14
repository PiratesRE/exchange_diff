using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmDatabaseOperationLock : IDisposable
	{
		internal int ThreadId
		{
			get
			{
				return this.m_threadId;
			}
		}

		internal AmDbLockReason Reason
		{
			get
			{
				return this.m_reason;
			}
		}

		internal Guid DatabaseGuid
		{
			get
			{
				return this.m_dbId;
			}
		}

		public static int GetThreadId()
		{
			return Environment.CurrentManagedThreadId;
		}

		public static AmDatabaseOperationLock Lock(Guid dbId, AmDbLockReason reason, TimeSpan? timeout)
		{
			AmDatabaseOperationLock amDatabaseOperationLock = null;
			AmDatabaseOperationLock amDatabaseOperationLock2 = new AmDatabaseOperationLock();
			amDatabaseOperationLock2.m_reason = reason;
			amDatabaseOperationLock2.m_dbId = dbId;
			amDatabaseOperationLock2.m_threadId = AmDatabaseOperationLock.GetThreadId();
			lock (AmDatabaseOperationLock.s_lockTable)
			{
				if (AmDatabaseOperationLock.s_lockTable.TryGetValue(dbId, out amDatabaseOperationLock) && amDatabaseOperationLock != null)
				{
					AmTrace.Error("AmDatabaseOperationLock: conflict on db {0}. Requesting for {1} but held by {2} for {3}", new object[]
					{
						dbId,
						reason,
						amDatabaseOperationLock.ThreadId,
						amDatabaseOperationLock.Reason
					});
					if (timeout == null || timeout.Value.Ticks == 0L || amDatabaseOperationLock.m_waiter != null)
					{
						throw new AmDbLockConflictException(dbId, reason.ToString(), amDatabaseOperationLock.Reason.ToString());
					}
					amDatabaseOperationLock2.m_waiterEvent = new ManualResetEvent(false);
					amDatabaseOperationLock2.m_mustWaitForLock = true;
					amDatabaseOperationLock.m_waiter = amDatabaseOperationLock2;
				}
				else
				{
					AmDatabaseOperationLock.s_lockTable[dbId] = amDatabaseOperationLock2;
					amDatabaseOperationLock2.m_holdingLock = true;
				}
			}
			if (amDatabaseOperationLock2.m_mustWaitForLock)
			{
				amDatabaseOperationLock2.WaitForLock(timeout.Value, amDatabaseOperationLock);
			}
			AmTrace.Debug("AmDatabaseOperationLock({0},{1}) : lock obtained", new object[]
			{
				dbId,
				reason
			});
			return amDatabaseOperationLock2;
		}

		[Conditional("DEBUG")]
		public static void AssertLockIsHeldByMe(Guid dbId)
		{
			lock (AmDatabaseOperationLock.s_lockTable)
			{
				AmDatabaseOperationLock amDatabaseOperationLock = null;
				if (AmDatabaseOperationLock.s_lockTable.TryGetValue(dbId, out amDatabaseOperationLock) && amDatabaseOperationLock != null)
				{
					AmDatabaseOperationLock.GetThreadId();
				}
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Unlock()
		{
			lock (this)
			{
				if (this.m_holdingLock)
				{
					AmTrace.Debug("AmDatabaseOperationLock({0},{1}) : releasing lock", new object[]
					{
						this.DatabaseGuid,
						this.Reason
					});
					lock (AmDatabaseOperationLock.s_lockTable)
					{
						AmDatabaseOperationLock amDatabaseOperationLock = null;
						if (AmDatabaseOperationLock.s_lockTable.TryGetValue(this.DatabaseGuid, out amDatabaseOperationLock) && amDatabaseOperationLock == this)
						{
							if (amDatabaseOperationLock.m_waiter != null)
							{
								AmTrace.Debug("AmDatabaseOperationLock({0},{1}) : granting lock to waiter:{2}", new object[]
								{
									this.DatabaseGuid,
									this.Reason,
									amDatabaseOperationLock.m_waiter.Reason
								});
								AmDatabaseOperationLock.s_lockTable[this.DatabaseGuid] = amDatabaseOperationLock.m_waiter;
								amDatabaseOperationLock.m_waiter.m_holdingLock = true;
								amDatabaseOperationLock.m_waiter.m_waiterEvent.Set();
								amDatabaseOperationLock.m_waiter = null;
							}
							else
							{
								AmDatabaseOperationLock.s_lockTable[this.DatabaseGuid] = null;
							}
						}
					}
				}
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			lock (this)
			{
				if (!this.m_disposed)
				{
					this.Unlock();
					if (this.m_waiterEvent != null)
					{
						((IDisposable)this.m_waiterEvent).Dispose();
					}
					this.m_disposed = true;
				}
			}
		}

		private void WaitForLock(TimeSpan timeout, AmDatabaseOperationLock origHolder)
		{
			AmDatabaseOperationLock amDatabaseOperationLock = null;
			long num = timeout.Ticks / 10000L;
			int num2 = (int)TimeSpan.FromHours(1.0).TotalMilliseconds;
			int num3;
			if (num > (long)num2)
			{
				num3 = num2;
			}
			else
			{
				num3 = (int)num;
			}
			AmTrace.Debug("AmDatabaseOperationLock({0},{1}) : waiting {2} ms for lock", new object[]
			{
				this.DatabaseGuid,
				this.Reason,
				num3
			});
			if (this.m_waiterEvent.WaitOne(num3, false))
			{
				return;
			}
			AmTrace.Error("AmDatabaseOperationLock: timeout", new object[0]);
			lock (AmDatabaseOperationLock.s_lockTable)
			{
				AmDatabaseOperationLock.s_lockTable.TryGetValue(this.DatabaseGuid, out amDatabaseOperationLock);
				if (amDatabaseOperationLock == origHolder)
				{
					amDatabaseOperationLock.m_waiter = null;
					throw new AmDbLockConflictException(this.DatabaseGuid, this.Reason.ToString(), origHolder.Reason.ToString());
				}
				AmTrace.Debug("AmDatabaseOperationLock: got lock after unlikely race", new object[0]);
			}
		}

		private static Dictionary<Guid, AmDatabaseOperationLock> s_lockTable = new Dictionary<Guid, AmDatabaseOperationLock>();

		private bool m_disposed;

		private bool m_holdingLock;

		private bool m_mustWaitForLock;

		private AmDatabaseOperationLock m_waiter;

		private ManualResetEvent m_waiterEvent;

		private int m_threadId;

		private AmDbLockReason m_reason;

		private Guid m_dbId;
	}
}
