using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Threading
{
	public struct WriterLockSlimWrapper : IDisposable
	{
		public WriterLockSlimWrapper(ReaderWriterLockSlim rwLockSlim)
		{
			ArgumentValidator.ThrowIfNull("rwLockSlim", rwLockSlim);
			this.rwLockSlim = rwLockSlim;
		}

		public void AcquireLock()
		{
			this.rwLockSlim.EnterWriteLock();
		}

		public void Dispose()
		{
			if (this.rwLockSlim.IsWriteLockHeld)
			{
				this.rwLockSlim.ExitWriteLock();
			}
		}

		private readonly ReaderWriterLockSlim rwLockSlim;
	}
}
