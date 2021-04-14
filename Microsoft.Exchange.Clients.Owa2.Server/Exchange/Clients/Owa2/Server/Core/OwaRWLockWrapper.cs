using System;
using System.Threading;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class OwaRWLockWrapper
	{
		public OwaRWLockWrapper()
		{
			this.rwLock = new ReaderWriterLockSlim();
		}

		public bool IsWriterLockHeld
		{
			get
			{
				return this.rwLock.IsWriteLockHeld;
			}
		}

		public bool IsReaderLockHeld
		{
			get
			{
				return this.rwLock.IsReadLockHeld;
			}
		}

		public bool LockWriterElastic(int suggestedTimeout)
		{
			return this.LockElastic(suggestedTimeout, true);
		}

		public bool LockReaderElastic(int suggestedTimeout)
		{
			return this.LockElastic(suggestedTimeout, false);
		}

		public bool LockWriter(int exactTimeout)
		{
			return this.Lock(exactTimeout, true);
		}

		public bool LockReader(int exactTimeout)
		{
			return this.Lock(exactTimeout, false);
		}

		public void ReleaseWriterLock()
		{
			this.rwLock.ExitWriteLock();
		}

		public void ReleaseReaderLock()
		{
			this.rwLock.ExitReadLock();
		}

		private static int GetProcessorCount()
		{
			return 8;
		}

		private bool LockElastic(int suggestedTimeout, bool lockWriter)
		{
			suggestedTimeout += (int)((double)(2 * suggestedTimeout) * Math.Max(0.0, 1.0 - Math.Pow((double)OwaRWLockWrapper.waitingOnLock / (10.0 * (double)OwaRWLockWrapper.procCount), 2.0)));
			return this.Lock(suggestedTimeout, lockWriter);
		}

		private bool Lock(int exactTimeout, bool lockWriter)
		{
			bool result;
			try
			{
				Interlocked.Increment(ref OwaRWLockWrapper.waitingOnLock);
				if (lockWriter)
				{
					result = this.rwLock.TryEnterWriteLock(exactTimeout);
				}
				else
				{
					result = this.rwLock.TryEnterReadLock(exactTimeout);
				}
			}
			catch (LockRecursionException innerException)
			{
				throw new OwaLockRecursionException(string.Format("Attempt to acquire {0} lock threw an exception", lockWriter ? "writer" : "reader"), innerException, this);
			}
			finally
			{
				Interlocked.Decrement(ref OwaRWLockWrapper.waitingOnLock);
			}
			return result;
		}

		private const double ForceSuggestedTimeoutAtThreadsPerProc = 10.0;

		private static int waitingOnLock = 0;

		private static int procCount = OwaRWLockWrapper.GetProcessorCount();

		private ReaderWriterLockSlim rwLock;
	}
}
