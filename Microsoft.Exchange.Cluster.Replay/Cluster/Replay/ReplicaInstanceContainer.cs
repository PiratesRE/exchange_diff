using System;
using System.Threading;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReplicaInstanceContainer : DisposeTrackableBase
	{
		public ReplicaInstanceContainer(ReplicaInstance instance, ReplicaInstanceActionQueue queue, IPerfmonCounters perfCounters)
		{
			this.ReplicaInstance = instance;
			this.Queue = queue;
			this.PerformanceCounters = perfCounters;
		}

		internal void UpdateReplicaInstance(ReplicaInstance newInstance)
		{
			this.ReplicaInstance = newInstance;
			if (this.m_cachedStatus != null)
			{
				this.m_cachedStatus.ForceRefresh = true;
			}
		}

		internal ReplicaInstance ReplicaInstance { get; private set; }

		internal ReplicaInstanceActionQueue Queue { get; private set; }

		internal IPerfmonCounters PerformanceCounters { get; private set; }

		internal RpcDatabaseCopyStatus2 GetCopyStatus(RpcGetDatabaseCopyStatusFlags2 flags)
		{
			if ((flags & RpcGetDatabaseCopyStatusFlags2.ReadThrough) == RpcGetDatabaseCopyStatusFlags2.None)
			{
				return this.GetCachedCopyStatus(flags);
			}
			RpcDatabaseCopyStatus2 copyStatusWithTimeout = this.GetCopyStatusWithTimeout(flags);
			CopyStatusServerCachedEntry copyStatusServerCachedEntry = this.UpdateCachedCopyStatus(copyStatusWithTimeout);
			return copyStatusServerCachedEntry.CopyStatus;
		}

		private RpcDatabaseCopyStatus2 GetCachedCopyStatus(RpcGetDatabaseCopyStatusFlags2 flags)
		{
			CopyStatusServerCachedEntry copyStatusServerCachedEntry = this.m_cachedStatus;
			if (this.IsCopyStatusReadThroughNeeded(copyStatusServerCachedEntry))
			{
				TimeSpan copyStatusServerTimeout = ReplicaInstanceContainer.CopyStatusServerTimeout;
				bool flag = Monitor.TryEnter(this.m_statusCallLocker);
				try
				{
					if (!flag)
					{
						ManualOneShotEvent.Result result = this.m_firstCachedStatusCallCompleted.WaitOne(copyStatusServerTimeout);
						if (result != ManualOneShotEvent.Result.Success)
						{
							throw new ReplayServiceRpcCopyStatusTimeoutException(this.ReplicaInstance.Configuration.DisplayName, (int)copyStatusServerTimeout.TotalSeconds);
						}
						lock (this.m_statusCacheLocker)
						{
							copyStatusServerCachedEntry = this.m_cachedStatus;
							if (copyStatusServerCachedEntry == null)
							{
								throw new ReplayServiceRpcCopyStatusTimeoutException(this.ReplicaInstance.Configuration.DisplayName, (int)copyStatusServerTimeout.TotalSeconds);
							}
							if (copyStatusServerCachedEntry.CreateTimeUtc < DateTime.UtcNow - ReplicaInstanceContainer.CopyStatusStaleTimeout)
							{
								Exception ex = new ReplayServiceRpcCopyStatusTimeoutException(this.ReplicaInstance.Configuration.DisplayName, (int)ReplicaInstanceContainer.CopyStatusStaleTimeout.TotalSeconds);
								copyStatusServerCachedEntry.CopyStatus.CopyStatus = CopyStatusEnum.Failed;
								copyStatusServerCachedEntry.CopyStatus.ErrorMessage = ex.Message;
								copyStatusServerCachedEntry.CopyStatus.ExtendedErrorInfo = new ExtendedErrorInfo(ex);
							}
							return copyStatusServerCachedEntry.CopyStatus;
						}
					}
					copyStatusServerCachedEntry = this.m_cachedStatus;
					if (this.IsCopyStatusReadThroughNeeded(copyStatusServerCachedEntry))
					{
						try
						{
							ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "GetCachedCopyStatus() for DB '{0}' ({1}): Cache TTL expired or force refresh requested.", this.ReplicaInstance.Configuration.DisplayName, this.ReplicaInstance.Configuration.IdentityGuid);
							RpcDatabaseCopyStatus2 copyStatusWithTimeout = this.GetCopyStatusWithTimeout(flags);
							copyStatusServerCachedEntry = this.UpdateCachedCopyStatus(copyStatusWithTimeout);
						}
						catch (TimeoutException arg)
						{
							ExTraceGlobals.ReplayServiceRpcTracer.TraceError<string, TimeoutException>((long)this.GetHashCode(), "GetCachedCopyStatus() Timeout for DB '{0}': {1}", this.ReplicaInstance.Configuration.DisplayName, arg);
							throw new ReplayServiceRpcCopyStatusTimeoutException(this.ReplicaInstance.Configuration.DisplayName, (int)copyStatusServerTimeout.TotalSeconds);
						}
						finally
						{
							this.m_firstCachedStatusCallCompleted.Set();
						}
					}
				}
				finally
				{
					if (flag)
					{
						Monitor.Exit(this.m_statusCallLocker);
					}
				}
			}
			return copyStatusServerCachedEntry.CopyStatus;
		}

		private CopyStatusServerCachedEntry UpdateCachedCopyStatus(RpcDatabaseCopyStatus2 status)
		{
			CopyStatusServerCachedEntry copyStatusServerCachedEntry = new CopyStatusServerCachedEntry(status);
			CopyStatusServerCachedEntry cachedStatus;
			lock (this.m_statusCacheLocker)
			{
				if (CopyStatusHelper.CheckCopyStatusNewer(copyStatusServerCachedEntry, this.m_cachedStatus))
				{
					this.m_cachedStatus = copyStatusServerCachedEntry;
				}
				cachedStatus = this.m_cachedStatus;
			}
			return cachedStatus;
		}

		private RpcDatabaseCopyStatus2 GetCopyStatusWithTimeout(RpcGetDatabaseCopyStatusFlags2 flags)
		{
			RpcDatabaseCopyStatus2 status = null;
			TimeSpan invokeTimeout = InvokeWithTimeout.InfiniteTimeSpan;
			if (RegistryParameters.GetCopyStatusServerTimeoutEnabled)
			{
				invokeTimeout = ReplicaInstanceContainer.CopyStatusServerTimeout;
			}
			InvokeWithTimeout.Invoke(delegate()
			{
				status = this.ReplicaInstance.GetCopyStatus(flags);
			}, null, invokeTimeout, true, null);
			return status;
		}

		private bool IsCopyStatusReadThroughNeeded(CopyStatusServerCachedEntry cachedStatus)
		{
			return cachedStatus == null || cachedStatus.ForceRefresh || cachedStatus.CreateTimeUtc < DateTime.UtcNow - ReplicaInstanceContainer.CopyStatusCacheTTL;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.m_firstCachedStatusCallCompleted != null)
			{
				this.m_firstCachedStatusCallCompleted.Close();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ReplicaInstanceContainer>(this);
		}

		private const string FirstStatusCallCompletedEventName = "FirstStatusCallCompletedEvent";

		private static readonly TimeSpan CopyStatusCacheTTL = TimeSpan.FromSeconds((double)RegistryParameters.GetCopyStatusRpcCacheTTLInSec);

		private static readonly TimeSpan CopyStatusServerTimeout = TimeSpan.FromSeconds((double)RegistryParameters.GetCopyStatusServerTimeoutSec);

		private static readonly TimeSpan CopyStatusStaleTimeout = TimeSpan.FromSeconds((double)RegistryParameters.GetCopyStatusServerCachedEntryStaleTimeoutSec);

		private CopyStatusServerCachedEntry m_cachedStatus;

		private object m_statusCacheLocker = new object();

		private object m_statusCallLocker = new object();

		private ManualOneShotEvent m_firstCachedStatusCallCompleted = new ManualOneShotEvent("FirstStatusCallCompletedEvent");
	}
}
