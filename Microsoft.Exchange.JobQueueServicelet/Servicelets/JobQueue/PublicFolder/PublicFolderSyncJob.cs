using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.PublicFolder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.JobQueues;

namespace Microsoft.Exchange.Servicelets.JobQueue.PublicFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PublicFolderSyncJob : Job
	{
		public PublicFolderSyncJob(JobQueue queue, OrganizationId orgId, Guid contentMailboxGuid, bool executeReconcileFolders) : base(queue, queue.Configuration, string.Empty)
		{
			this.OrganizationId = orgId;
			this.ContentMailboxGuid = contentMailboxGuid;
			this.ExecuteReconcileFolders = executeReconcileFolders;
		}

		public OrganizationId OrganizationId { get; private set; }

		public Guid ContentMailboxGuid { get; private set; }

		public bool ExecuteReconcileFolders { get; private set; }

		public override void Begin(object state)
		{
			if (this.ShouldSyncMailbox())
			{
				PublicFolderSynchronizer.Begin(this.OrganizationId, this.ContentMailboxGuid, this.ExecuteReconcileFolders, delegate(Exception exception)
				{
					base.LastError = exception;
					this.End();
				});
				return;
			}
			this.End();
		}

		private bool ShouldSyncMailbox()
		{
			bool result;
			try
			{
				ExchangePrincipal exchangePrincipal;
				if (!PublicFolderSession.TryGetPublicFolderMailboxPrincipal(this.OrganizationId, this.ContentMailboxGuid, false, out exchangePrincipal) || exchangePrincipal == null)
				{
					PublicFolderSynchronizerLogger.LogOnServer(string.Format("Sync Cancelled for Mailbox {0} in Organization {1}. Could not get ExchangePrincipal. Mailbox could have been deleted.", this.ContentMailboxGuid, this.OrganizationId), LogEventType.Warning, null);
					result = false;
				}
				else if (!exchangePrincipal.MailboxInfo.IsRemote && LocalServerCache.LocalServer != null && exchangePrincipal.MailboxInfo.Location.ServerFqdn != LocalServerCache.LocalServer.Fqdn)
				{
					PublicFolderSynchronizerLogger.LogOnServer(string.Format("Sync Cancelled for Mailbox {0} in Organization {1}. Mailbox was moved to Server {2}.", this.ContentMailboxGuid, this.OrganizationId, exchangePrincipal.MailboxInfo.Location.ServerFqdn), LogEventType.Warning, null);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			catch (LocalizedException e)
			{
				PublicFolderSynchronizerLogger.LogOnServer(string.Format("Sync Cancelled for Mailbox {0} in Organization {1}. Could not get ExchangePrincipal. Mailbox could have been deleted/relocated. Exception - {2}", this.ContentMailboxGuid, this.OrganizationId, PublicFolderMailboxLoggerBase.GetExceptionLogString(e, PublicFolderMailboxLoggerBase.ExceptionLogOption.All)), LogEventType.Warning, null);
				result = false;
			}
			return result;
		}
	}
}
