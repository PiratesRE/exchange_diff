using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Net.JobQueues;
using Microsoft.Exchange.ServiceHost;
using Microsoft.Exchange.Servicelets.JobQueue.Messages;
using Microsoft.Exchange.Servicelets.JobQueue.PublicFolder;

namespace Microsoft.Exchange.Servicelets.JobQueue
{
	public class Servicelet : Servicelet
	{
		public override void Work()
		{
			bool flag = false;
			try
			{
				AppConfig appConfig;
				try
				{
					appConfig = new AppConfig();
				}
				catch (ConfigurationErrorsException ex)
				{
					this.eventLog.LogEvent(MSExchangeJobQueueEventLogConstants.Tuple_FailedToLoadAppConfig, null, new object[]
					{
						ex.Message
					});
					return;
				}
				TeamMailboxSyncConfiguration config = new TeamMailboxSyncConfiguration(appConfig.TMSyncCacheAbsoluteExpiry, appConfig.TMSyncCacheSlidingExpiry, appConfig.TMSyncCacheBucketCount, appConfig.TMSyncCacheBucketSize, appConfig.TMSyncMaxJobQueueLength, appConfig.TMSyncMaxPendingJobs, appConfig.TMSyncDispatcherWakeupInterval, appConfig.TMSyncMinSyncInterval, appConfig.TMSyncSharePointQueryPageSize, appConfig.TMSyncUseOAuth, appConfig.TMSyncHttpDebugEnabled);
				DocumentSyncJobQueue item = new DocumentSyncJobQueue(config, new TeamMailboxSyncResourceMonitorFactory(), new OAuthCredentialFactory(), true);
				MembershipSyncJobQueue item2 = new MembershipSyncJobQueue(config, new TeamMailboxSecurityRefresher(), new TeamMailboxSyncActiveDirectoryResourceMonitorFactory(), new OAuthCredentialFactory(), true);
				MaintenanceSyncJobQueue item3 = new MaintenanceSyncJobQueue(config, new TeamMailboxSyncActiveDirectoryResourceMonitorFactory(), new OAuthCredentialFactory(), true);
				PublicFolderSyncJobQueue item4 = new PublicFolderSyncJobQueue();
				JobQueueManager.Initialize(new List<JobQueue>
				{
					item,
					item2,
					item3,
					item4
				});
				Exception ex2;
				if (!RpcServerWrapper.Start(appConfig.EnqueueRequestTimeout, out ex2))
				{
					this.eventLog.LogEvent(MSExchangeJobQueueEventLogConstants.Tuple_FailedToRegisterRpc, null, new object[]
					{
						ex2.Message
					});
				}
				else
				{
					flag = true;
					base.StopEvent.WaitOne();
				}
			}
			finally
			{
				if (flag)
				{
					RpcServerWrapper.Stop();
					JobQueueManager.Shutdown();
				}
			}
		}

		private static readonly Guid ComponentGuid = new Guid("94c643bc-a01d-4994-a40f-2eae50c10414");

		private readonly ExEventLog eventLog = new ExEventLog(Servicelet.ComponentGuid, "MSExchange JobQueue");
	}
}
