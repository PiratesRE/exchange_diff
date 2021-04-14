using System;
using System.Threading;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class OwaRWLockWrapper
	{
		public OwaRWLockWrapper()
		{
			this.rwLock = new ReaderWriterLock();
		}

		public void LockWriterElastic(int suggestedTimeout)
		{
			this.LockElastic(suggestedTimeout, true);
		}

		public void LockReaderElastic(int suggestedTimeout)
		{
			this.LockElastic(suggestedTimeout, false);
		}

		public void LockWriter(int exactTimeout)
		{
			this.Lock(exactTimeout, true);
		}

		public void LockReader(int exactTimeout)
		{
			this.Lock(exactTimeout, false);
		}

		public void ReleaseWriterLock()
		{
			if (this.rwLock.IsWriterLockHeld)
			{
				Interlocked.Decrement(ref this.numberOfWriterLocksHeld);
			}
			this.rwLock.ReleaseWriterLock();
		}

		public void ReleaseReaderLock()
		{
			this.rwLock.ReleaseReaderLock();
		}

		public void ReleaseLock()
		{
			if (this.rwLock.IsWriterLockHeld)
			{
				Interlocked.Exchange(ref this.numberOfWriterLocksHeld, 0);
			}
			this.rwLock.ReleaseLock();
		}

		public void DowngradeFromWriterLock(ref LockCookie cookieForDowngrade)
		{
			this.rwLock.DowngradeFromWriterLock(ref cookieForDowngrade);
		}

		private static int GetProcessorCount()
		{
			return 8;
		}

		private void LockElastic(int suggestedTimeout, bool lockWriter)
		{
			suggestedTimeout += (int)((double)(2 * suggestedTimeout) * Math.Max(0.0, 1.0 - Math.Pow((double)OwaRWLockWrapper.waitingOnLock / (10.0 * (double)OwaRWLockWrapper.procCount), 2.0)));
			this.Lock(suggestedTimeout, lockWriter);
		}

		private void Lock(int exactTimeout, bool lockWriter)
		{
			try
			{
				Interlocked.Increment(ref OwaRWLockWrapper.waitingOnLock);
				if (lockWriter)
				{
					this.rwLock.AcquireWriterLock(exactTimeout);
					Interlocked.Increment(ref this.numberOfWriterLocksHeld);
				}
				else
				{
					this.rwLock.AcquireReaderLock(exactTimeout);
				}
			}
			catch (ApplicationException innerException)
			{
				throw new OwaLockTimeoutException(string.Format("Attempt to acquire {0} lock on user context timed out", lockWriter ? "writer" : "reader"), innerException, this);
			}
			finally
			{
				Interlocked.Decrement(ref OwaRWLockWrapper.waitingOnLock);
			}
		}

		public bool IsWriterLockHeld
		{
			get
			{
				return this.rwLock.IsWriterLockHeld;
			}
		}

		public bool IsReaderLockHeld
		{
			get
			{
				return this.rwLock.IsReaderLockHeld;
			}
		}

		public int NumberOfWriterLocksHeld
		{
			get
			{
				return this.numberOfWriterLocksHeld;
			}
			private set
			{
				this.numberOfWriterLocksHeld = value;
			}
		}

		private const double ForceSuggestedTimeoutAtThreadsPerProc = 10.0;

		private static int waitingOnLock = 0;

		private static int procCount = OwaRWLockWrapper.GetProcessorCount();

		private ReaderWriterLock rwLock;

		private int numberOfWriterLocksHeld;
	}
}
