using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.JobQueues;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MembershipSyncJob : TeamMailboxSyncJob
	{
		public MembershipSyncJob(JobQueue queue, Configuration config, ITeamMailboxSecurityRefresher teamMailboxSecurityRefresher, TeamMailboxSyncInfo syncInfoEntry, string clientString, SyncOption syncOption) : base(queue, config, syncInfoEntry, clientString, syncOption)
		{
			if (teamMailboxSecurityRefresher == null)
			{
				throw new ArgumentNullException("teamMailboxSecurityRefresher");
			}
			this.teamMailboxSecurityRefresher = teamMailboxSecurityRefresher;
			this.loggingComponent = ProtocolLog.Component.MembershipSync;
		}

		public override void Begin(object state)
		{
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					base.SafeInitializeLoggingStream();
					MembershipSynchronizer membershipSynchronizer = new MembershipSynchronizer(this, base.SyncInfoEntry.MailboxSession, base.SyncInfoEntry.MailboxPrincipal.MailboxInfo.OrganizationId, this.teamMailboxSecurityRefresher, base.SyncInfoEntry.ResourceMonitor, base.SyncInfoEntry.SiteUrl, this.credentials, ((TeamMailboxSyncConfiguration)base.Config).UseOAuth, ((TeamMailboxSyncConfiguration)base.Config).HttpDebugEnabled, this.syncCycleLogStream);
					membershipSynchronizer.BeginExecute(new AsyncCallback(this.OnMembershipSynchronizationCompleted), membershipSynchronizer);
				});
			}
			catch (GrayException lastError)
			{
				base.LastError = lastError;
				this.End();
			}
		}

		private void OnMembershipSynchronizationCompleted(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new InvalidOperationException("TeamMailboxSyncJob.OnMembershipSynchronizationCompleted: asyncResult cannot be null here.");
			}
			MembershipSynchronizer membershipSynchronizer = asyncResult.AsyncState as MembershipSynchronizer;
			if (membershipSynchronizer == null)
			{
				throw new InvalidOperationException("TeamMailboxSyncJob.OnMembershipSynchronizationCompleted: asyncResult.AsyncState is not MembershipSynchronizer");
			}
			membershipSynchronizer.EndExecute(asyncResult);
			if (membershipSynchronizer.LastError != null)
			{
				base.LastError = membershipSynchronizer.LastError;
			}
			this.End();
		}

		private readonly ITeamMailboxSecurityRefresher teamMailboxSecurityRefresher;
	}
}
