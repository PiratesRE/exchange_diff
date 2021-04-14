using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal sealed class StateLock : IIdentityGuid
	{
		private StateLock(string dbName, string identity)
		{
			this.m_databaseName = dbName;
			this.m_identity = identity;
		}

		public static StateLock GetNewOrExistingStateLock(string dbName, string identity)
		{
			return SuspendLockTable.GetNewOrExistingStateLock(dbName, identity);
		}

		internal static StateLock ConstructStateLock(string dbName, string identity)
		{
			return new StateLock(dbName, identity);
		}

		public bool SuspendWanted { get; set; }

		public bool TryEnterSuspend(bool fWait, out LockOwner currentOwner)
		{
			ExTraceGlobals.StateLockTracer.TraceDebug<string, bool>((long)this.GetHashCode(), "{0}: TryEnterSuspend(fWait={1})", this.m_databaseName, fWait);
			if (this.CurrentOwner == LockOwner.Suspend)
			{
				currentOwner = LockOwner.Suspend;
				ExTraceGlobals.StateLockTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: TryEnterSuspend(): Suspend already owns the lock. Leaving.", this.m_databaseName);
				return true;
			}
			this.SuspendWanted = true;
			bool flag = this.TryEnterInternal(LockOwner.Suspend, fWait, new TimeSpan?(TimeSpan.FromMilliseconds((double)RegistryParameters.SuspendLockTimeoutInMsec)), out currentOwner);
			if (!flag)
			{
				ExTraceGlobals.StateLockTracer.TraceDebug<string, LockOwner>((long)this.GetHashCode(), "{0}: TryEnterSuspend(): Could not acquire Suspend lock since '{1}' owns the lock.", this.m_databaseName, currentOwner);
			}
			return flag;
		}

		public bool TryEnterAcll(bool fWait, TimeSpan? timeout, out LockOwner currentOwner)
		{
			ExTraceGlobals.StateLockTracer.TraceDebug<string, bool>((long)this.GetHashCode(), "{0}: TryEnterAcll(fWait={1})", this.m_databaseName, fWait);
			return this.TryEnterInternal(LockOwner.AttemptCopyLastLogs, fWait, timeout, out currentOwner);
		}

		public bool TryEnterAcll(bool fWait, TimeSpan? timeout, out LockOwner currentOwner, ActionToRunBeforeWaitingForLock actionBeforeWaitForLock)
		{
			ExTraceGlobals.StateLockTracer.TraceDebug<string, bool>((long)this.GetHashCode(), "{0}: TryEnterAcll(fWait={1})", this.m_databaseName, fWait);
			return this.TryEnterInternal(LockOwner.AttemptCopyLastLogs, fWait, timeout, out currentOwner, actionBeforeWaitForLock);
		}

		public bool TryEnter(LockOwner attemptOwner, bool fWait)
		{
			LockOwner lockOwner;
			return this.TryEnter(attemptOwner, fWait, out lockOwner);
		}

		public bool TryEnter(LockOwner attemptOwner, bool fWait, out LockOwner currentOwner)
		{
			ExTraceGlobals.StateLockTracer.TraceDebug<string, LockOwner, bool>((long)this.GetHashCode(), "{0}: TryEnter(attemptOwner={1}, fWait={2})", this.m_databaseName, attemptOwner, fWait);
			if (attemptOwner == LockOwner.Suspend)
			{
				throw new ArgumentException("StateLock.TryEnter is not for use with LockOwner.Suspend. Use StateLockRemote.TryEnter or StateLock.TryEnterSuspend instead.", "attemptOwner");
			}
			if (attemptOwner == LockOwner.AttemptCopyLastLogs)
			{
				throw new ArgumentException("StateLock.TryEnter is not for use with LockOwner.AttemptCopyLastLogs. Use StateLock.TryEnterAcll instead.", "attemptOwner");
			}
			return this.TryEnterInternal(attemptOwner, fWait, new TimeSpan?(TimeSpan.FromMilliseconds((double)RegistryParameters.SuspendLockTimeoutInMsec)), out currentOwner);
		}

		private bool TryEnterInternal(LockOwner attemptOwner, bool fWait, TimeSpan? timeout, out LockOwner currentOwner)
		{
			return this.TryEnterInternal(attemptOwner, fWait, timeout, out currentOwner, null);
		}

		private bool TryEnterInternal(LockOwner attemptOwner, bool fWait, TimeSpan? timeout, out LockOwner currentOwner, ActionToRunBeforeWaitingForLock actionBeforeWaitForLock)
		{
			ExTraceGlobals.StateLockTracer.TraceFunction((long)this.GetHashCode(), "TryEnterInternal {0}: attemptOwner='{1}', fWait='{2}', timeout='{3}'", new object[]
			{
				this.m_databaseName,
				attemptOwner,
				fWait,
				(timeout != null) ? timeout.Value.ToString() : "<null>"
			});
			bool flag = false;
			LockOwner? lockOwner = null;
			lock (this)
			{
				if (attemptOwner == LockOwner.AttemptCopyLastLogs)
				{
					if (this.CurrentOwner == LockOwner.AttemptCopyLastLogs)
					{
						currentOwner = LockOwner.AttemptCopyLastLogs;
						ExTraceGlobals.StateLockTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: TryEnterAcll(): ACLL already owns the lock. Is another thread trying to run ACLL? Leaving.", this.m_databaseName);
						return false;
					}
					if (this.m_pendingOwners.Contains(attemptOwner))
					{
						currentOwner = LockOwner.AttemptCopyLastLogs;
						ExTraceGlobals.StateLockTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: TryEnterAcll(): ACLL is already on the pending owners list. Is another thread trying to run ACLL? Leaving.", this.m_databaseName);
						return false;
					}
				}
				if (!fWait)
				{
					if (attemptOwner == this.CurrentOwner)
					{
						ExTraceGlobals.StateLockTracer.TraceDebug<string>((long)this.GetHashCode(), "TryEnterInternal {0}: attemptOwner == this.CurrentOwner", this.m_databaseName);
						LockOwner lockOwner2;
						if (this.ShouldGiveUpLock(out lockOwner2))
						{
							flag = false;
							lockOwner = new LockOwner?(lockOwner2);
							ExTraceGlobals.StateLockTracer.TraceDebug<string, LockOwner>((long)this.GetHashCode(), "TryEnterInternal {0}: fGotLock='false' because of ShouldGiveUpLock(), highestPending='{1}'", this.m_databaseName, lockOwner2);
						}
						else
						{
							this.m_depthCount++;
							flag = true;
							ExTraceGlobals.StateLockTracer.TraceDebug<string, int>((long)this.GetHashCode(), "TryEnterInternal {0}: fGotLock='true', m_depthCount='{1}'", this.m_databaseName, this.m_depthCount);
						}
					}
					else
					{
						ExTraceGlobals.StateLockTracer.TraceDebug<string>((long)this.GetHashCode(), "TryEnterInternal {0}: attemptOwner != this.CurrentOwner", this.m_databaseName);
						flag = this.Arbitrate(new LockOwner?(attemptOwner));
					}
				}
				else
				{
					if (attemptOwner != LockOwner.Suspend)
					{
						this.AddToPendingList(attemptOwner);
					}
					flag = this.Arbitrate(null);
				}
				lockOwner = new LockOwner?(lockOwner ?? this.CurrentOwner);
			}
			if (!flag && fWait)
			{
				if (actionBeforeWaitForLock != null)
				{
					actionBeforeWaitForLock();
				}
				flag = this.WaitForLock(attemptOwner, null, timeout ?? TimeSpan.FromMilliseconds((double)RegistryParameters.SuspendLockTimeoutInMsec));
				lockOwner = new LockOwner?(this.CurrentOwner);
				if (!flag)
				{
					this.RemovePendingLock(attemptOwner);
				}
			}
			currentOwner = lockOwner.Value;
			ExTraceGlobals.StateLockTracer.TraceDebug<string, bool, LockOwner>((long)this.GetHashCode(), "{0}: TryEnter returning, fGotLock={1}, currentOwner={2}", this.m_databaseName, flag, currentOwner);
			return flag;
		}

		public void LeaveSuspend()
		{
			lock (this)
			{
				ExTraceGlobals.StateLockTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: LeaveSuspend()", this.m_databaseName);
				this.ClearSuspendWanted();
				if (this.CurrentOwner == LockOwner.Suspend)
				{
					this.Arbitrate(null);
				}
				DiagCore.RetailAssert(this.CurrentOwner != LockOwner.Suspend, "LeaveSuspend() failed to change current owner from Suspend.", new object[0]);
			}
		}

		public void LeaveAttemptCopyLastLogs()
		{
			lock (this)
			{
				if (this.CurrentOwner == LockOwner.AttemptCopyLastLogs)
				{
					this.Leave(LockOwner.AttemptCopyLastLogs);
				}
			}
		}

		public void Leave(LockOwner leavingOwner)
		{
			ExTraceGlobals.StateLockTracer.TraceDebug<string, LockOwner>((long)this.GetHashCode(), "{0}: Leave(leavingOwner={1})", this.m_databaseName, leavingOwner);
			if (leavingOwner == LockOwner.Suspend)
			{
				throw new ArgumentException("leavingOwner");
			}
			lock (this)
			{
				if (this.CurrentOwner == leavingOwner && --this.m_depthCount == 0)
				{
					ExTraceGlobals.StateLockTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: leaving owner has lock, releasing it", this.m_databaseName);
					this.SetCurrentOwner(LockOwner.Idle);
					this.Arbitrate(null);
				}
			}
		}

		public LockOwner CurrentOwner
		{
			get
			{
				return this.m_currentOwner;
			}
		}

		public TimeSpan LockHeldDuration
		{
			get
			{
				return DateTime.UtcNow.Subtract(this.m_lockAcquiredTimeUtc);
			}
		}

		public bool ShouldGiveUpLock()
		{
			LockOwner lockOwner;
			return this.ShouldGiveUpLock(out lockOwner);
		}

		public bool ShouldGiveUpLock(out LockOwner highestPending)
		{
			LockOwner owner;
			this.UpdateHighestPriorityPending(false, out owner, out highestPending);
			bool flag = this.Priority(owner) < this.Priority(highestPending);
			ExTraceGlobals.StateLockTracer.TraceDebug<string, bool>((long)this.GetHashCode(), "{0}: ShouldGiveUpLock returns {1}", this.m_databaseName, flag);
			return flag;
		}

		public bool Arbitrate(LockOwner? attemptOwner)
		{
			ExTraceGlobals.StateLockTracer.TraceDebug<string, LockOwner?>((long)this.GetHashCode(), "{0}: Arbitrate(attemptOwner = {1})", this.m_databaseName, attemptOwner);
			bool result;
			lock (this)
			{
				bool suspendWanted = this.SuspendWanted;
				if (this.CurrentOwner == LockOwner.Idle || (this.CurrentOwner == LockOwner.Suspend && !suspendWanted))
				{
					LockOwner lockOwner = this.HighestPriorityPending;
					bool flag2 = false;
					if (attemptOwner != null && (attemptOwner == LockOwner.Suspend || this.Priority(attemptOwner.Value) > this.Priority(lockOwner)))
					{
						lockOwner = attemptOwner.Value;
						flag2 = true;
						ExTraceGlobals.StateLockTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: AttemptOwner is highest priority", this.m_databaseName);
					}
					ExTraceGlobals.StateLockTracer.TraceDebug<string, LockOwner>((long)this.GetHashCode(), "{0}: Highest priority pending is {1}", this.m_databaseName, lockOwner);
					if (lockOwner != this.CurrentOwner)
					{
						this.SetCurrentOwner(lockOwner);
						if (!flag2 && lockOwner != LockOwner.Suspend)
						{
							this.RemoveFromPendingList(lockOwner);
						}
					}
					ExTraceGlobals.StateLockTracer.TraceDebug<string, LockOwner>((long)this.GetHashCode(), "{0}: CurrentOwner is now {1}", this.m_databaseName, lockOwner);
					result = flag2;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private bool WaitForLock(LockOwner desiredLockOwner, WaitHandle cancelEvent, TimeSpan timeout)
		{
			ExTraceGlobals.StateLockTracer.TraceDebug<string, LockOwner>((long)this.GetHashCode(), "{0}: WaitForLock(desiredLockOwner = {1})", this.m_databaseName, desiredLockOwner);
			bool flag = false;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			while (!flag)
			{
				if (desiredLockOwner != LockOwner.Suspend && this.CurrentOwner == LockOwner.Suspend)
				{
					flag = true;
					ExTraceGlobals.StateLockTracer.TraceDebug<string, LockOwner>((long)this.GetHashCode(), "{0}: WaitForLock() returning 'false' because Suspend has the lock and '{1}' wants it.", this.m_databaseName, desiredLockOwner);
					break;
				}
				if (desiredLockOwner != LockOwner.AttemptCopyLastLogs && this.CurrentOwner == LockOwner.AttemptCopyLastLogs)
				{
					flag = true;
					ExTraceGlobals.StateLockTracer.TraceDebug<string, LockOwner>((long)this.GetHashCode(), "{0}: WaitForLock() returning 'false' because AttemptCopyLastLogs has the lock and '{1}' wants it.", this.m_databaseName, desiredLockOwner);
					break;
				}
				if (desiredLockOwner == LockOwner.Suspend && !this.SuspendWanted)
				{
					flag = true;
					ExTraceGlobals.StateLockTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: WaitForLock() returning 'false' because SuspendWanted is no longer true.", this.m_databaseName);
					break;
				}
				if (this.CurrentOwner == desiredLockOwner)
				{
					ExTraceGlobals.StateLockTracer.TraceDebug<string, LockOwner, long>((long)this.GetHashCode(), "{0}: WaitForLock() returning 'true' because '{1}' got the lock after {2}ms.", this.m_databaseName, desiredLockOwner, stopwatch.ElapsedMilliseconds);
					break;
				}
				TimeSpan elapsed = stopwatch.Elapsed;
				if (elapsed > timeout)
				{
					flag = true;
					ExTraceGlobals.StateLockTracer.TraceError((long)this.GetHashCode(), "{0}: WaitForLock() waited '{1}' to enter '{2}' and will now timeout. CurrentOwner = '{3}'", new object[]
					{
						this.m_databaseName,
						elapsed,
						desiredLockOwner,
						this.CurrentOwner
					});
					break;
				}
				if (cancelEvent != null)
				{
					flag = cancelEvent.WaitOne(100, false);
				}
				else
				{
					Thread.Sleep(100);
				}
			}
			ExTraceGlobals.StateLockTracer.TraceDebug<string, bool>((long)this.GetHashCode(), "{0}: WaitForLock() returning {1}", this.m_databaseName, !flag);
			return !flag;
		}

		private void RemovePendingLock(LockOwner attemptOwner)
		{
			lock (this)
			{
				ExTraceGlobals.StateLockTracer.TraceDebug<string, LockOwner>((long)this.GetHashCode(), "{0}: RemovePendingLock({1})", this.m_databaseName, attemptOwner);
				if (attemptOwner == LockOwner.Suspend)
				{
					this.ClearSuspendWanted();
				}
				else
				{
					this.RemoveFromPendingList(attemptOwner);
				}
			}
		}

		private void ClearSuspendWanted()
		{
			ExTraceGlobals.StateLockTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: ClearSuspendWanted()", this.m_databaseName);
			this.SuspendWanted = false;
			this.UpdateHighestPriorityPending(false);
		}

		private void AddToPendingList(LockOwner pendingOwner)
		{
			lock (this)
			{
				ExTraceGlobals.StateLockTracer.TraceDebug<string, LockOwner>((long)this.GetHashCode(), "{0}: AddToPendingList({1})", this.m_databaseName, pendingOwner);
				this.m_pendingOwners.Add(pendingOwner);
				this.UpdateHighestPriorityPending(true);
			}
		}

		private void RemoveFromPendingList(LockOwner removingOwner)
		{
			lock (this)
			{
				ExTraceGlobals.StateLockTracer.TraceDebug<string, LockOwner>((long)this.GetHashCode(), "{0}: RemoveFromPendingList({1})", this.m_databaseName, removingOwner);
				this.m_pendingOwners.Remove(removingOwner);
				this.UpdateHighestPriorityPending(true);
			}
		}

		private void UpdateHighestPriorityPending(bool fPendingListChanged)
		{
			LockOwner lockOwner;
			LockOwner lockOwner2;
			this.UpdateHighestPriorityPending(fPendingListChanged, out lockOwner, out lockOwner2);
		}

		private void UpdateHighestPriorityPending(bool fPendingListChanged, out LockOwner currentOwner, out LockOwner highestPriorityPending)
		{
			LockOwner lockOwner = LockOwner.Idle;
			highestPriorityPending = LockOwner.Idle;
			lock (this)
			{
				bool suspendWanted = this.SuspendWanted;
				currentOwner = this.CurrentOwner;
				if (suspendWanted && currentOwner != LockOwner.Suspend)
				{
					this.m_highestPriorityPending = LockOwner.Suspend;
				}
				else if (fPendingListChanged || (!suspendWanted && currentOwner == LockOwner.Suspend))
				{
					foreach (LockOwner lockOwner2 in this.m_pendingOwners)
					{
						if (this.Priority(lockOwner2) > this.Priority(lockOwner))
						{
							lockOwner = lockOwner2;
						}
					}
					this.m_highestPriorityPending = lockOwner;
				}
				highestPriorityPending = this.m_highestPriorityPending;
			}
			ExTraceGlobals.StateLockTracer.TraceDebug<string, LockOwner>((long)this.GetHashCode(), "{0}: m_highestPriorityPending = {1}", this.m_databaseName, this.m_highestPriorityPending);
		}

		private void SetCurrentOwner(LockOwner newCurrentOwner)
		{
			lock (this)
			{
				this.m_currentOwner = newCurrentOwner;
				this.m_lockAcquiredTimeUtc = DateTime.UtcNow;
				this.m_depthCount = 1;
			}
		}

		private int Priority(LockOwner owner)
		{
			return (int)(owner / LockOwner.Component);
		}

		private LockOwner HighestPriorityPending
		{
			get
			{
				this.UpdateHighestPriorityPending(false);
				return this.m_highestPriorityPending;
			}
		}

		public string Identity
		{
			get
			{
				return this.m_identity;
			}
		}

		private string m_databaseName;

		private string m_identity;

		private LockOwner m_currentOwner;

		private DateTime m_lockAcquiredTimeUtc;

		private int m_depthCount;

		private List<LockOwner> m_pendingOwners = new List<LockOwner>();

		private LockOwner m_highestPriorityPending;
	}
}
