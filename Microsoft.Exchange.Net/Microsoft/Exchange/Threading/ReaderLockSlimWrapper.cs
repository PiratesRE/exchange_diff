using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Threading
{
	public struct ReaderLockSlimWrapper : IDisposable
	{
		public ReaderLockSlimWrapper(ReaderWriterLockSlim rwLockSlim)
		{
			ArgumentValidator.ThrowIfNull("rwLockSlim", rwLockSlim);
			this.rwLockSlim = rwLockSlim;
		}

		public void AcquireLock()
		{
			this.rwLockSlim.EnterReadLock();
		}

		public void Dispose()
		{
			if (this.rwLockSlim.IsReadLockHeld)
			{
				this.rwLockSlim.ExitReadLock();
			}
		}

		private readonly ReaderWriterLockSlim rwLockSlim;
	}
}
