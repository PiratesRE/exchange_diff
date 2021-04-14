using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.WorkloadManagement.EventLogs;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class MailboxDatabaseCopyStatusCache
	{
		protected MailboxDatabaseCopyStatusCache()
		{
		}

		public static Hookable<MailboxDatabaseCopyStatusCache> Instance
		{
			get
			{
				return MailboxDatabaseCopyStatusCache.instance;
			}
		}

		public virtual RpcDatabaseCopyStatus2 TryGetCopyStatus(string serverFqdn, Guid mdbGuid)
		{
			MailboxDatabaseCopyStatusCache.CopyStatusCacheEntry copyStatusCacheEntry = null;
			try
			{
				this.lockObject.EnterUpgradeableReadLock();
				if (!this.data.TryGetValue(serverFqdn, out copyStatusCacheEntry))
				{
					try
					{
						this.lockObject.EnterWriteLock();
						if (!this.data.TryGetValue(serverFqdn, out copyStatusCacheEntry))
						{
							copyStatusCacheEntry = new MailboxDatabaseCopyStatusCache.CopyStatusCacheEntry();
							this.data.Add(serverFqdn, copyStatusCacheEntry);
						}
					}
					finally
					{
						try
						{
							this.lockObject.ExitWriteLock();
						}
						catch (SynchronizationLockException)
						{
						}
					}
				}
			}
			finally
			{
				try
				{
					this.lockObject.ExitUpgradeableReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			RpcDatabaseCopyStatus2 result;
			try
			{
				copyStatusCacheEntry.LockObject.EnterUpgradeableReadLock();
				if (copyStatusCacheEntry.UpdateTime > ExDateTime.UtcNow - CiHealthMonitorConfiguration.RefreshInterval)
				{
					result = copyStatusCacheEntry.TryGetCopyStatus(mdbGuid);
				}
				else
				{
					try
					{
						copyStatusCacheEntry.LockObject.EnterWriteLock();
						if (copyStatusCacheEntry.UpdateTime > ExDateTime.UtcNow - CiHealthMonitorConfiguration.RefreshInterval)
						{
							return copyStatusCacheEntry.TryGetCopyStatus(mdbGuid);
						}
						RpcDatabaseCopyStatus2[] results = null;
						RpcErrorExceptionInfo errorInfo = null;
						try
						{
							AmRpcExceptionWrapper.Instance.ClientRetryableOperation(serverFqdn, delegate
							{
								using (ReplayRpcClient replayRpcClient = new ReplayRpcClient(serverFqdn))
								{
									replayRpcClient.SetTimeOut((int)CiHealthMonitorConfiguration.RpcTimeout.TotalMilliseconds);
									errorInfo = replayRpcClient.RpccGetCopyStatusEx4(RpcGetDatabaseCopyStatusFlags2.None, MailboxDatabaseCopyStatusCache.dbGuidsForCachedCopyStatus, ref results);
								}
							});
							if (!errorInfo.IsFailed())
							{
								copyStatusCacheEntry.Update(results);
							}
							else
							{
								ExTraceGlobals.ResourceHealthManagerTracer.TraceError<string>((long)this.GetHashCode(), "[MailboxDatabaseCopyStatusCache::TryGetCopyStatus] RPC call failed, error: {0}", errorInfo.ErrorMessage);
								WorkloadManagerEventLogger.LogEvent(WorkloadManagementEventLogConstants.Tuple_CiMdbCopyStatusFailure, string.Empty, new object[]
								{
									serverFqdn,
									errorInfo.ErrorMessage
								});
								copyStatusCacheEntry.Update();
							}
						}
						catch (AmServerException ex)
						{
							ExTraceGlobals.ResourceHealthManagerTracer.TraceError<AmServerException>((long)this.GetHashCode(), "[MailboxDatabaseCopyStatusCache::TryGetCopyStatus] Failed to execute RPC call, exception: {0}", ex);
							if (!(ex is AmReplayServiceDownException))
							{
								WorkloadManagerEventLogger.LogEvent(WorkloadManagementEventLogConstants.Tuple_CiMdbCopyStatusFailure, string.Empty, new object[]
								{
									serverFqdn,
									ex.ToString()
								});
							}
							copyStatusCacheEntry.Update();
						}
					}
					finally
					{
						try
						{
							copyStatusCacheEntry.LockObject.ExitWriteLock();
						}
						catch (SynchronizationLockException)
						{
						}
					}
					result = copyStatusCacheEntry.TryGetCopyStatus(mdbGuid);
				}
			}
			finally
			{
				try
				{
					copyStatusCacheEntry.LockObject.ExitUpgradeableReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			return result;
		}

		private static readonly Hookable<MailboxDatabaseCopyStatusCache> instance = Hookable<MailboxDatabaseCopyStatusCache>.Create(true, new MailboxDatabaseCopyStatusCache());

		private static readonly Guid[] dbGuidsForCachedCopyStatus = new Guid[]
		{
			Guid.Empty
		};

		private readonly Dictionary<string, MailboxDatabaseCopyStatusCache.CopyStatusCacheEntry> data = new Dictionary<string, MailboxDatabaseCopyStatusCache.CopyStatusCacheEntry>();

		private readonly ReaderWriterLockSlim lockObject = new ReaderWriterLockSlim();

		private class CopyStatusCacheEntry
		{
			public CopyStatusCacheEntry()
			{
				this.UpdateTime = ExDateTime.MinValue;
				this.LockObject = new ReaderWriterLockSlim();
			}

			public ExDateTime UpdateTime { get; private set; }

			public ReaderWriterLockSlim LockObject { get; private set; }

			public RpcDatabaseCopyStatus2 TryGetCopyStatus(Guid mdbGuid)
			{
				RpcDatabaseCopyStatus2 result = null;
				if (this.data.TryGetValue(mdbGuid, out result))
				{
					return result;
				}
				return null;
			}

			public void Update(IEnumerable<RpcDatabaseCopyStatus2> data)
			{
				this.UpdateTime = ExDateTime.UtcNow;
				this.data.Clear();
				foreach (RpcDatabaseCopyStatus2 rpcDatabaseCopyStatus in data)
				{
					this.data[rpcDatabaseCopyStatus.DBGuid] = rpcDatabaseCopyStatus;
				}
			}

			public void Update()
			{
				this.UpdateTime = ExDateTime.UtcNow;
				this.data.Clear();
			}

			private readonly Dictionary<Guid, RpcDatabaseCopyStatus2> data = new Dictionary<Guid, RpcDatabaseCopyStatus2>();
		}
	}
}
