using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.JobQueues;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MaintenanceSyncJobQueue : TeamMailboxSyncJobQueue
	{
		public MaintenanceSyncJobQueue(TeamMailboxSyncConfiguration config, IResourceMonitorFactory resourceMonitorFactory, IOAuthCredentialFactory oauthCredentialFactory, bool createTeamMailboxSyncInfoCache = true) : base(QueueType.TeamMailboxMaintenanceSync, "TeamMailboxMaintenanceLastSyncCycleLog", config, resourceMonitorFactory, oauthCredentialFactory, createTeamMailboxSyncInfoCache)
		{
		}

		protected override Job InternalCreateJob(TeamMailboxSyncInfo info, string clientString, SyncOption syncOption)
		{
			return new MaintenanceSyncJob(this, this.config, info, clientString, syncOption);
		}
	}
}
