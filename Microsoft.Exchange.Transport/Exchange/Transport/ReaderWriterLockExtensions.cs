using System;
using System.Threading;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Transport
{
	internal static class ReaderWriterLockExtensions
	{
		public static IDisposable AcquireReadLock(this ReaderWriterLockSlim slimLock)
		{
			ReaderLockSlimWrapper readerLockSlimWrapper = new ReaderLockSlimWrapper(slimLock);
			readerLockSlimWrapper.AcquireLock();
			return readerLockSlimWrapper;
		}

		public static IDisposable AcquireWriteLock(this ReaderWriterLockSlim slimLock)
		{
			WriterLockSlimWrapper writerLockSlimWrapper = new WriterLockSlimWrapper(slimLock);
			writerLockSlimWrapper.AcquireLock();
			return writerLockSlimWrapper;
		}

		public static IDisposable AcquireUpgradeableLock(this ReaderWriterLockSlim slimLock)
		{
			UpgradeableLockSlimWrapper upgradeableLockSlimWrapper = new UpgradeableLockSlimWrapper(slimLock);
			upgradeableLockSlimWrapper.AcquireLock();
			return upgradeableLockSlimWrapper;
		}
	}
}
