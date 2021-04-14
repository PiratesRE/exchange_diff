using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.JobQueues;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MaintenanceSyncJob : TeamMailboxSyncJob
	{
		public MaintenanceSyncJob(JobQueue queue, Configuration config, TeamMailboxSyncInfo syncInfoEntry, string clientString, SyncOption syncOption) : base(queue, config, syncInfoEntry, clientString, syncOption)
		{
		}

		public override void Begin(object state)
		{
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					base.SafeInitializeLoggingStream();
					MaintenanceSynchronizer maintenanceSynchronizer = new MaintenanceSynchronizer(this, base.SyncInfoEntry.MailboxSession, base.SyncInfoEntry.MailboxPrincipal.MailboxInfo.OrganizationId, base.SyncInfoEntry.WebCollectionUrl, base.SyncInfoEntry.WebId, base.SyncInfoEntry.SiteUrl, base.SyncInfoEntry.DisplayName, base.SyncInfoEntry.ResourceMonitor, this.credentials, ((TeamMailboxSyncConfiguration)base.Config).UseOAuth, ((TeamMailboxSyncConfiguration)base.Config).HttpDebugEnabled, this.syncCycleLogStream);
					maintenanceSynchronizer.BeginExecute(new AsyncCallback(this.OnMaintenanceSynchronizationCompleted), maintenanceSynchronizer);
				});
			}
			catch (GrayException lastError)
			{
				base.LastError = lastError;
				this.End();
			}
		}

		private void OnMaintenanceSynchronizationCompleted(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new InvalidOperationException("TeamMailboxSyncJob.OnMaintenanceSynchronizationCompleted: asyncResult cannot be null here.");
			}
			MaintenanceSynchronizer maintenanceSynchronizer = asyncResult.AsyncState as MaintenanceSynchronizer;
			if (maintenanceSynchronizer == null)
			{
				throw new InvalidOperationException("TeamMailboxSyncJob.OnMaintenanceSynchronizationCompleted: asyncResult.AsyncState is not MaintenanceSynchronizer");
			}
			maintenanceSynchronizer.EndExecute(asyncResult);
			if (maintenanceSynchronizer.LastError != null)
			{
				base.LastError = maintenanceSynchronizer.LastError;
			}
			this.End();
		}
	}
}
