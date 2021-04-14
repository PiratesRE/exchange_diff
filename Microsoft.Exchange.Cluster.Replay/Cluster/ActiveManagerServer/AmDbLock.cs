using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmDbLock
	{
		internal AmDbLock()
		{
		}

		internal bool IsExiting
		{
			get
			{
				return this.m_isExiting;
			}
			set
			{
				this.m_isExiting = value;
			}
		}

		internal bool Lock(Guid dbGuid, AmDbLockReason lockReason)
		{
			bool flag = false;
			int num = 0;
			int currentManagedThreadId = Environment.CurrentManagedThreadId;
			AmTrace.Debug("Trying to acquire lock for db={0} requester={1} reason={2}", new object[]
			{
				dbGuid,
				currentManagedThreadId,
				lockReason
			});
			do
			{
				lock (this.m_locker)
				{
					AmDbLock.LockData lockData = null;
					if (this.m_lockTable.TryGetValue(dbGuid, out lockData))
					{
						AmTrace.Debug("lock already held for db={0} heldby={1} current={2} reason={3}", new object[]
						{
							dbGuid,
							lockData.ThreadId,
							currentManagedThreadId,
							lockData.Reason
						});
						if (lockData.ThreadId == currentManagedThreadId)
						{
							lockData.RefCount++;
							flag = true;
							AmTrace.Debug("same thread has requested lock db={0} heldby={1} refcount={2}", new object[]
							{
								dbGuid,
								lockData.ThreadId,
								lockData.RefCount
							});
						}
					}
					else
					{
						lockData = new AmDbLock.LockData(currentManagedThreadId, lockReason);
						this.m_lockTable[dbGuid] = lockData;
						flag = true;
						AmTrace.Debug("lock created for db={0} owner={1} reason={2}", new object[]
						{
							dbGuid,
							lockData.ThreadId,
							lockData.Reason
						});
					}
				}
				if (flag)
				{
					break;
				}
				AmTrace.Debug("sleeping to get the lock again for db={0} requester={1} reason={2} sleptSoFar={3}", new object[]
				{
					dbGuid,
					currentManagedThreadId,
					lockReason,
					num
				});
				Thread.Sleep(500);
				num += 500;
				AmDbLock.WarnIfSleepingForEver(dbGuid, num, lockReason);
			}
			while (!this.IsExiting);
			if (this.IsExiting && flag)
			{
				this.Release(dbGuid, lockReason);
				flag = false;
			}
			AmTrace.Debug("Got lock!! db={0} requester={1}", new object[]
			{
				dbGuid,
				currentManagedThreadId
			});
			return flag;
		}

		internal void Release(Guid dbGuid, AmDbLockReason lockReason)
		{
			int currentManagedThreadId = Environment.CurrentManagedThreadId;
			AmTrace.Debug("releasing lock for db={0} requester={1} reason={2}", new object[]
			{
				dbGuid,
				currentManagedThreadId,
				lockReason
			});
			lock (this.m_locker)
			{
				AmDbLock.LockData lockData = null;
				if (this.m_lockTable.TryGetValue(dbGuid, out lockData) && lockData.ThreadId == currentManagedThreadId)
				{
					lockData.RefCount--;
					if (lockData.RefCount == 0)
					{
						this.m_lockTable.Remove(dbGuid);
					}
				}
			}
			AmTrace.Debug("lock released for db={0} requester={1}", new object[]
			{
				dbGuid,
				currentManagedThreadId
			});
		}

		private static void WarnIfSleepingForEver(Guid dbGuid, int timeSlept, AmDbLockReason lockReason)
		{
			if (timeSlept % 10000 == 0)
			{
				ReplayEventLogConstants.Tuple_DatabaseOperationLockIsTakingLongTime.LogEvent(null, new object[]
				{
					dbGuid,
					timeSlept,
					lockReason
				});
			}
		}

		private const int LockRetrySleepTime = 500;

		private const int WarnTimeDuration = 10000;

		private bool m_isExiting;

		private object m_locker = new object();

		private Dictionary<Guid, AmDbLock.LockData> m_lockTable = new Dictionary<Guid, AmDbLock.LockData>();

		internal class LockData
		{
			internal LockData(int threadId, AmDbLockReason lockReason)
			{
				this.m_reason = lockReason;
				this.m_threadId = threadId;
				this.m_refCount = 1;
			}

			internal int RefCount
			{
				get
				{
					return this.m_refCount;
				}
				set
				{
					this.m_refCount = value;
				}
			}

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

			private int m_refCount;

			private int m_threadId;

			private AmDbLockReason m_reason;
		}
	}
}
