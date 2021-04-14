using System;
using System.Threading;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class PendingNotifierLockTracker
	{
		public bool TryReleaseAllLocks(PendingNotifierLockTracker.ReleaseAllLocksCallback callback)
		{
			this.allLocksReleasedCallback = callback;
			int num = Interlocked.Exchange(ref this.pendingRequestCounter, int.MinValue);
			if (num == 0)
			{
				this.lockOwner = Thread.CurrentThread;
				this.allLocksReleasedCallback = null;
			}
			return num == 0;
		}

		public bool TryAcquireLock()
		{
			int num = Interlocked.Increment(ref this.pendingRequestCounter);
			if (num == 1)
			{
				this.lockOwner = Thread.CurrentThread;
			}
			return num == 1;
		}

		public bool TryAcquireLockOnlyIfSucceed()
		{
			int num = Interlocked.CompareExchange(ref this.pendingRequestCounter, 1, 0);
			if (num == 0)
			{
				this.lockOwner = Thread.CurrentThread;
			}
			return num == 0;
		}

		public bool TryReleaseLock()
		{
			return this.TryReleaseLock(false);
		}

		public bool TryReleaseLock(bool isCurrentRequestCompleted)
		{
			if (!isCurrentRequestCompleted && this.lockOwner != Thread.CurrentThread)
			{
				throw new OwaOperationNotSupportedException("This thread is not the owner of the lock!");
			}
			this.lockOwner = null;
			if (this.pendingRequestCounter < 0)
			{
				this.lockOwner = Thread.CurrentThread;
				if (this.allLocksReleasedCallback != null)
				{
					this.allLocksReleasedCallback();
				}
				return true;
			}
			int num = Interlocked.Decrement(ref this.pendingRequestCounter);
			if (num > 0)
			{
				this.lockOwner = Thread.CurrentThread;
			}
			return num == 0;
		}

		public bool SetPipeAvailable(bool releaseLock)
		{
			int num = Interlocked.CompareExchange(ref this.pendingRequestAvailable, 1, 0);
			if (num != 0)
			{
				throw new OwaExistentNotificationPipeException("There is already a pending pipe for the same user context");
			}
			this.lockOwner = Thread.CurrentThread;
			return releaseLock && !this.TryReleaseLock();
		}

		public void SetPipeUnavailable()
		{
			if (this.lockOwner != Thread.CurrentThread)
			{
				throw new OwaOperationNotSupportedException("This thread is not the owner of the lock!");
			}
			int num = Interlocked.CompareExchange(ref this.pendingRequestAvailable, 0, 1);
			if (num != 1)
			{
				throw new OwaNotificationPipeException("The pipe is already unavailable");
			}
			this.TryAcquireLock();
		}

		public bool IsLockOwner()
		{
			return this.lockOwner == Thread.CurrentThread;
		}

		private const int ReleaseAllLocksValue = -2147483648;

		private volatile PendingNotifierLockTracker.ReleaseAllLocksCallback allLocksReleasedCallback;

		private int pendingRequestCounter = 1;

		private int pendingRequestAvailable;

		private Thread lockOwner;

		public delegate void ReleaseAllLocksCallback();
	}
}
