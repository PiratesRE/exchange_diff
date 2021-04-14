using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net.JobQueues;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class TeamMailboxSyncJobQueue : JobQueue, IDisposeTrackable, IDisposable
	{
		public IOAuthCredentialFactory OAuthCredentialFactory { get; private set; }

		public TeamMailboxSyncJobQueue(QueueType queueType, string syncLogConfigurationName, TeamMailboxSyncConfiguration config, IResourceMonitorFactory resourceMonitorFactory, IOAuthCredentialFactory oauthCredentialFactory, bool createTeamMailboxSyncInfoCache = true) : base(queueType, config)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			if (resourceMonitorFactory == null)
			{
				throw new ArgumentNullException("resourceMonitorFactory");
			}
			if (oauthCredentialFactory == null)
			{
				throw new ArgumentNullException("oauthCredentialFactory");
			}
			this.OAuthCredentialFactory = oauthCredentialFactory;
			if (createTeamMailboxSyncInfoCache)
			{
				this.teamMailboxSyncInfoCache = new TeamMailboxSyncInfoCache(resourceMonitorFactory, config.CacheSlidingExpiry, config.CacheBucketCount, config.CacheBucketSize, syncLogConfigurationName);
			}
			this.disposeTracker = this.GetDisposeTracker();
		}

		public override void Cleanup()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
				this.disposeTracker = null;
			}
			if (this.teamMailboxSyncInfoCache != null)
			{
				this.teamMailboxSyncInfoCache.Dispose();
			}
			GC.SuppressFinalize(this);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<TeamMailboxSyncJobQueue>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		protected override bool TryCreateJob(byte[] data, out Job job, out EnqueueResult result)
		{
			result = EnqueueResult.Success;
			job = null;
			string server = null;
			TeamMailboxSyncRpcInParameters teamMailboxSyncRpcInParameters = null;
			TeamMailboxSyncInfo teamMailboxSyncInfo = null;
			try
			{
				teamMailboxSyncRpcInParameters = new TeamMailboxSyncRpcInParameters(data);
			}
			catch (SerializationException ex)
			{
				result = new EnqueueResult(EnqueueResultType.InvalidData, ServerStrings.RpcServerParameterSerializationError(ex.Message));
				return false;
			}
			catch (ArgumentNullException ex2)
			{
				result = new EnqueueResult(EnqueueResultType.InvalidData, ServerStrings.RpcServerParameterInvalidError(ex2.Message));
				return false;
			}
			OrganizationId orgId = teamMailboxSyncRpcInParameters.OrgId.Equals(OrganizationId.ForestWideOrgId) ? OrganizationId.ForestWideOrgId : teamMailboxSyncRpcInParameters.OrgId;
			TeamMailboxSyncId teamMailboxSyncId = new TeamMailboxSyncId(teamMailboxSyncRpcInParameters.MailboxGuid, orgId, teamMailboxSyncRpcInParameters.DomainController);
			bool flag = (teamMailboxSyncRpcInParameters.SyncOption & SyncOption.RefreshTeamMailboxCacheEntry) == SyncOption.RefreshTeamMailboxCacheEntry || !string.IsNullOrEmpty(teamMailboxSyncRpcInParameters.DomainController);
			try
			{
				bool flag2 = this.teamMailboxSyncInfoCache.Contains(teamMailboxSyncId);
				teamMailboxSyncInfo = this.teamMailboxSyncInfoCache.Get(teamMailboxSyncId);
				if (this.ShouldSkipSync(teamMailboxSyncId, teamMailboxSyncInfo, out result))
				{
					return false;
				}
				if ((teamMailboxSyncInfo.MailboxSession != null && (teamMailboxSyncInfo.MailboxSession.IsDisposed || teamMailboxSyncInfo.MailboxSession.IsDead)) || ExDateTime.UtcNow - teamMailboxSyncInfo.WhenCreatedUtcTime > ((TeamMailboxSyncConfiguration)base.Configuration).CacheAbsoluteExpiry || (flag && flag2))
				{
					TeamMailboxSyncInfo info = this.teamMailboxSyncInfoCache.Remove(teamMailboxSyncId);
					this.CleanupSyncInfo(info);
					teamMailboxSyncInfo = this.teamMailboxSyncInfoCache.Get(teamMailboxSyncId);
					if (this.ShouldSkipSync(teamMailboxSyncId, teamMailboxSyncInfo, out result))
					{
						return false;
					}
				}
				server = TeamMailboxSyncInfoCache.LocalServerFqdn;
			}
			catch (ADTransientException ex3)
			{
				result = new EnqueueResult(EnqueueResultType.DirectoryError, ServerStrings.RpcServerDirectoryError(teamMailboxSyncId.MailboxGuid.ToString(), ex3.Message));
				return false;
			}
			catch (DataSourceOperationException ex4)
			{
				result = new EnqueueResult(EnqueueResultType.DirectoryError, ServerStrings.RpcServerDirectoryError(teamMailboxSyncId.MailboxGuid.ToString(), ex4.Message));
				return false;
			}
			catch (DataValidationException ex5)
			{
				result = new EnqueueResult(EnqueueResultType.DirectoryError, ServerStrings.RpcServerDirectoryError(teamMailboxSyncId.MailboxGuid.ToString(), ex5.Message));
				return false;
			}
			catch (StorageTransientException ex6)
			{
				result = new EnqueueResult(EnqueueResultType.StorageError, ServerStrings.RpcServerStorageError(teamMailboxSyncId.MailboxGuid.ToString(), ex6.Message));
				return false;
			}
			catch (WrongServerException ex7)
			{
				result = new EnqueueResult(EnqueueResultType.WrongServer, ServerStrings.RpcServerWrongRequestServer(teamMailboxSyncId.MailboxGuid.ToString(), ex7.Message));
				return false;
			}
			catch (StoragePermanentException ex8)
			{
				result = new EnqueueResult(EnqueueResultType.StorageError, ServerStrings.RpcServerStorageError(teamMailboxSyncId.MailboxGuid.ToString(), ex8.Message));
				return false;
			}
			if ((teamMailboxSyncRpcInParameters.SyncOption & SyncOption.IgnoreNextAllowedSyncTimeRestriction) == SyncOption.Default && teamMailboxSyncInfo.NextAllowedSyncUtcTime > ExDateTime.UtcNow)
			{
				result = new EnqueueResult(EnqueueResultType.RequestThrottled, ServerStrings.RpcServerRequestThrottled(teamMailboxSyncId.MailboxGuid.ToString(), teamMailboxSyncInfo.NextAllowedSyncUtcTime.ToString()));
				return false;
			}
			teamMailboxSyncInfo.IsPending = true;
			teamMailboxSyncInfo.PendingClientString = teamMailboxSyncRpcInParameters.ClientString;
			teamMailboxSyncInfo.PendingClientRequestTime = ExDateTime.UtcNow;
			job = this.InternalCreateJob(teamMailboxSyncInfo, teamMailboxSyncRpcInParameters.ClientString, teamMailboxSyncRpcInParameters.SyncOption);
			result.ResultDetail = ServerStrings.RpcServerRequestSuccess(server);
			return true;
		}

		public override void OnJobCompletion(Job job)
		{
			TeamMailboxSyncJob teamMailboxSyncJob = job as TeamMailboxSyncJob;
			lock (this.syncObject)
			{
				teamMailboxSyncJob.SyncInfoEntry.IsPending = false;
				teamMailboxSyncJob.SyncInfoEntry.PendingClientString = null;
				teamMailboxSyncJob.SyncInfoEntry.PendingClientRequestTime = ExDateTime.MinValue;
				teamMailboxSyncJob.SyncInfoEntry.LastSyncUtcTime = ExDateTime.UtcNow;
				teamMailboxSyncJob.SyncInfoEntry.NextAllowedSyncUtcTime = ExDateTime.UtcNow + ((TeamMailboxSyncConfiguration)this.config).MinSyncInterval;
				if (teamMailboxSyncJob.LastError != null)
				{
					teamMailboxSyncJob.SyncInfoEntry.SyncErrors[teamMailboxSyncJob.SyncInfoEntry.LastSyncUtcTime] = teamMailboxSyncJob.LastError;
				}
				base.OnJobCompletion(job);
			}
		}

		protected abstract Job InternalCreateJob(TeamMailboxSyncInfo info, string clientString, SyncOption syncOption);

		private void CleanupSyncInfo(TeamMailboxSyncInfo info)
		{
			if (info != null)
			{
				if (info.Logger != null)
				{
					info.Logger.Dispose();
				}
				if (info.MailboxSession != null && !info.MailboxSession.IsDisposed)
				{
					info.MailboxSession.Dispose();
				}
			}
		}

		private bool ShouldSkipSync(TeamMailboxSyncId id, TeamMailboxSyncInfo info, out EnqueueResult result)
		{
			result = EnqueueResult.Success;
			if (info == null)
			{
				result = new EnqueueResult(EnqueueResultType.NonexistentTeamMailbox, ServerStrings.RpcServerIgnoreNotFoundTeamMailbox(id.MailboxGuid.ToString()));
				return true;
			}
			if (info.IsPending)
			{
				result = new EnqueueResult(EnqueueResultType.AlreadyPending, ServerStrings.RpcServerRequestAlreadyPending(info.MailboxGuid.ToString(), info.PendingClientString, info.PendingClientRequestTime.ToString()));
				return true;
			}
			if (info.LifeCycleState == TeamMailboxLifecycleState.PendingDelete)
			{
				result = new EnqueueResult(EnqueueResultType.PendingDeleteTeamMailbox, ServerStrings.RpcServerIgnorePendingDeleteTeamMailbox(id.MailboxGuid.ToString()));
				return true;
			}
			if (info.LifeCycleState == TeamMailboxLifecycleState.Unlinked)
			{
				result = new EnqueueResult(EnqueueResultType.UnlinkedTeamMailbox, ServerStrings.RpcServerIgnoreUnlinkedTeamMailbox(id.MailboxGuid.ToString()));
				return true;
			}
			return false;
		}

		protected TeamMailboxSyncInfoCache teamMailboxSyncInfoCache;

		private DisposeTracker disposeTracker;
	}
}
