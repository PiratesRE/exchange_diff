using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class SuspendLockTable
	{
		internal static void AddSuspendLock(StateLock suspendLock)
		{
			SuspendLockTable.s_suspendLocks.AddInstance(suspendLock);
		}

		internal static void RemoveInstance(StateLock suspendLock)
		{
			SuspendLockTable.s_suspendLocks.RemoveInstance(suspendLock);
		}

		internal static bool TryGetSuspendLock(string identity, out StateLock suspendLock)
		{
			return SuspendLockTable.s_suspendLocks.TryGetInstance(identity, out suspendLock);
		}

		internal static StateLock GetNewOrExistingStateLock(string dbName, string identity)
		{
			return SuspendLockTable.s_suspendLocks.GetNewOrExistingStateLock(dbName, identity);
		}

		private static SuspendLockTable.SuspendLocks s_suspendLocks = new SuspendLockTable.SuspendLocks();

		[ClassAccessLevel(AccessLevel.Implementation)]
		private class SuspendLocks : SafeInstanceTable<StateLock>
		{
			internal StateLock GetNewOrExistingStateLock(string dbName, string identity)
			{
				this.m_rwLock.AcquireWriterLock(-1);
				StateLock result;
				try
				{
					StateLock stateLock;
					if (this.m_instances.ContainsKey(identity))
					{
						stateLock = this.m_instances[identity];
					}
					else
					{
						stateLock = StateLock.ConstructStateLock(dbName, identity);
						this.m_instances.Add(identity, stateLock);
					}
					result = stateLock;
				}
				finally
				{
					this.m_rwLock.ReleaseWriterLock();
				}
				return result;
			}
		}
	}
}
