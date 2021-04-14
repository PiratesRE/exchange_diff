using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Threading
{
	public struct UpgradeableLockSlimWrapper : IDisposable
	{
		public UpgradeableLockSlimWrapper(ReaderWriterLockSlim rwLockSlim)
		{
			ArgumentValidator.ThrowIfNull("rwLockSlim", rwLockSlim);
			this.rwLockSlim = rwLockSlim;
		}

		public void AcquireLock()
		{
			this.rwLockSlim.EnterUpgradeableReadLock();
		}

		public void Dispose()
		{
			if (this.rwLockSlim.IsUpgradeableReadLockHeld)
			{
				this.rwLockSlim.ExitUpgradeableReadLock();
			}
		}

		private readonly ReaderWriterLockSlim rwLockSlim;
	}
}
