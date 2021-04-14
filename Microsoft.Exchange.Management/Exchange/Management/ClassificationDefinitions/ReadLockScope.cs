using System;
using System.Threading;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal class ReadLockScope : ReaderWriterLockScopeBase
	{
		private ReadLockScope(ReaderWriterLockSlim readerWriterLock) : base(readerWriterLock)
		{
		}

		protected override void Acquire()
		{
			base.ScopedReaderWriterLock.EnterReadLock();
		}

		protected override void Release()
		{
			base.ScopedReaderWriterLock.ExitReadLock();
		}

		internal static ReadLockScope Create(ReaderWriterLockSlim readerWriterLock)
		{
			if (readerWriterLock == null)
			{
				throw new ArgumentNullException("readerWriterLock");
			}
			ReadLockScope readLockScope = new ReadLockScope(readerWriterLock);
			readLockScope.Acquire();
			return readLockScope;
		}
	}
}
