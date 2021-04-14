using System;
using System.IO;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.JobQueues;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class TeamMailboxSyncJob : Job
	{
		public TeamMailboxSyncInfo SyncInfoEntry { get; private set; }

		public SyncOption SyncOption { get; private set; }

		public TeamMailboxSyncJob(JobQueue queue, Configuration config, TeamMailboxSyncInfo syncInfoEntry, string clientString, SyncOption syncOption) : base(queue, config, clientString)
		{
			this.SyncInfoEntry = syncInfoEntry;
			this.SyncOption = syncOption;
			if (((TeamMailboxSyncConfiguration)config).UseOAuth)
			{
				this.credentials = ((TeamMailboxSyncJobQueue)queue).OAuthCredentialFactory.Get(syncInfoEntry.MailboxPrincipal.MailboxInfo.OrganizationId);
			}
			else
			{
				this.credentials = CredentialCache.DefaultCredentials;
			}
			this.loggingContext = new LoggingContext(this.SyncInfoEntry.MailboxGuid, this.SyncInfoEntry.SiteUrl, base.ClientString, null);
		}

		protected override void End()
		{
			try
			{
				if (base.LastError != null)
				{
					ProtocolLog.LogCycleFailure(this.loggingComponent, this.loggingContext, "The sync cycle completed with error", base.LastError);
				}
				else
				{
					ProtocolLog.LogCycleSuccess(this.loggingComponent, this.loggingContext, "The sync cycle completed successfully");
				}
				this.SafeCloseLoggingStream();
			}
			finally
			{
				base.End();
			}
		}

		protected void SafeInitializeLoggingStream()
		{
			try
			{
				if (this.SyncInfoEntry.Logger != null)
				{
					this.syncCycleLogStream = this.SyncInfoEntry.Logger.GetStream();
				}
				else
				{
					ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "SafeInitializeLoggingStream: Logger is null and possibly caused by corruption of log stream configuration", new ArgumentException());
				}
			}
			catch (IOException exception)
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "SafeInitializeLoggingStream: Failed with IOException", exception);
			}
			catch (StorageTransientException exception2)
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "SafeInitializeLoggingStream: Failed with StorageTransientException", exception2);
			}
			catch (StoragePermanentException exception3)
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "SafeInitializeLoggingStream: Failed with StoragePermanentException", exception3);
			}
		}

		protected void SafeCloseLoggingStream()
		{
			if (this.syncCycleLogStream != null)
			{
				try
				{
					this.syncCycleLogStream.SetLength(this.syncCycleLogStream.Position);
					this.syncCycleLogStream.Close();
					this.syncCycleLogStream = null;
					this.SyncInfoEntry.Logger.Save();
				}
				catch (IOException exception)
				{
					ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "SafeCloseLoggingStream: Failed with IOException", exception);
				}
				catch (StorageTransientException exception2)
				{
					ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "SafeCloseLoggingStream: Failed with StorageTransientException", exception2);
				}
				catch (StoragePermanentException exception3)
				{
					ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "SafeCloseLoggingStream: Failed with StoragePermanentException", exception3);
				}
			}
		}

		private readonly LoggingContext loggingContext;

		protected ProtocolLog.Component loggingComponent;

		protected ICredentials credentials;

		protected Stream syncCycleLogStream;
	}
}
