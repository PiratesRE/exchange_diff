using System;
using System.Threading;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal class UpgradeableReadLockScope : ReaderWriterLockScopeBase
	{
		private UpgradeableReadLockScope(ReaderWriterLockSlim readerWriterLock) : base(readerWriterLock)
		{
		}

		protected override void Acquire()
		{
			base.ScopedReaderWriterLock.EnterUpgradeableReadLock();
		}

		protected override void Release()
		{
			if (!this.upgradeableReadLockExited)
			{
				base.ScopedReaderWriterLock.ExitUpgradeableReadLock();
				this.upgradeableReadLockExited = true;
			}
		}

		internal WriteLockScope Upgrade()
		{
			if (base.IsDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			if (!this.lockChanged && !this.upgradeableReadLockExited)
			{
				this.lockChanged = true;
				return WriteLockScope.Create(base.ScopedReaderWriterLock);
			}
			return null;
		}

		internal ReadLockScope Downgrade()
		{
			if (base.IsDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			if (!this.lockChanged && !this.upgradeableReadLockExited)
			{
				this.lockChanged = true;
				ReadLockScope result = ReadLockScope.Create(base.ScopedReaderWriterLock);
				this.Release();
				return result;
			}
			return null;
		}

		internal static UpgradeableReadLockScope Create(ReaderWriterLockSlim readerWriterLock)
		{
			if (readerWriterLock == null)
			{
				throw new ArgumentNullException("readerWriterLock");
			}
			UpgradeableReadLockScope upgradeableReadLockScope = new UpgradeableReadLockScope(readerWriterLock);
			upgradeableReadLockScope.Acquire();
			return upgradeableReadLockScope;
		}

		private bool upgradeableReadLockExited;

		private bool lockChanged;
	}
}
