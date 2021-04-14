using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Shared
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ReaderWriterLockedBase
	{
		protected void ReaderLockedOperation(Action operation)
		{
			try
			{
				this.m_rwLock.EnterReadLock();
				operation();
			}
			finally
			{
				try
				{
					this.m_rwLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
		}

		protected void WriterLockedOperation(Action operation)
		{
			try
			{
				this.m_rwLock.EnterWriteLock();
				operation();
			}
			finally
			{
				try
				{
					this.m_rwLock.ExitWriteLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
		}

		private ReaderWriterLockSlim m_rwLock = new ReaderWriterLockSlim();
	}
}
