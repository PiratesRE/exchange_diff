using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.JobQueues;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MembershipSyncJobQueue : TeamMailboxSyncJobQueue
	{
		public MembershipSyncJobQueue(TeamMailboxSyncConfiguration config, ITeamMailboxSecurityRefresher teamMailboxSecurityRefresher, IResourceMonitorFactory resourceMonitorFactory, IOAuthCredentialFactory oauthCredentialFactory, bool createTeamMailboxSyncInfoCache = true) : base(QueueType.TeamMailboxMembershipSync, "TeamMailboxMembershipLastSyncCycleLog", config, resourceMonitorFactory, oauthCredentialFactory, createTeamMailboxSyncInfoCache)
		{
			if (teamMailboxSecurityRefresher == null)
			{
				throw new ArgumentNullException("teamMailboxSecurityRefresher");
			}
			this.teamMailboxSecurityRefresher = teamMailboxSecurityRefresher;
		}

		protected override Job InternalCreateJob(TeamMailboxSyncInfo info, string clientString, SyncOption syncOption)
		{
			return new MembershipSyncJob(this, this.config, this.teamMailboxSecurityRefresher, info, clientString, syncOption);
		}

		private ITeamMailboxSecurityRefresher teamMailboxSecurityRefresher;
	}
}
