using System;
using System.Threading;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal class WriteLockScope : ReaderWriterLockScopeBase
	{
		private WriteLockScope(ReaderWriterLockSlim readerWriterLock) : base(readerWriterLock)
		{
		}

		protected override void Acquire()
		{
			base.ScopedReaderWriterLock.EnterWriteLock();
		}

		protected override void Release()
		{
			base.ScopedReaderWriterLock.ExitWriteLock();
		}

		internal static WriteLockScope Create(ReaderWriterLockSlim readerWriterLock)
		{
			if (readerWriterLock == null)
			{
				throw new ArgumentNullException("readerWriterLock");
			}
			WriteLockScope writeLockScope = new WriteLockScope(readerWriterLock);
			writeLockScope.Acquire();
			return writeLockScope;
		}
	}
}
