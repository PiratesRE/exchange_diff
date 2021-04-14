using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.Migration
{
	[Cmdlet("Complete", "MigrationBatch", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class CompleteMigrationBatch : MigrationObjectTaskBase<MigrationBatchIdParameter>
	{
		public override string Action
		{
			get
			{
				return "Complete-MigrationBatch";
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<SmtpAddress> NotificationEmails
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)base.Fields["NotificationEmails"];
			}
			set
			{
				base.Fields["NotificationEmails"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.DataObject != null && this.DataObject.PendingCount > 0)
				{
					return Strings.ConfirmationMessageCompleteMigrationBatchWithPendingItems(this.DataObject.Identity.ToString(), this.DataObject.PendingCount);
				}
				if (this.DataObject != null && this.DataObject.FailedInitialSyncCount > 0)
				{
					return Strings.ConfirmationMessageCompleteMigrationBatchWithFailedItems(this.DataObject.Identity.ToString(), this.DataObject.FailedInitialSyncCount);
				}
				if (this.DataObject != null)
				{
					return Strings.ConfirmationMessageCompleteMigrationBatch(this.DataObject.Identity.ToString());
				}
				return Strings.ConfirmationMessageCompleteMigrationBatch(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			MigrationBatchDataProvider migrationBatchDataProvider = (MigrationBatchDataProvider)base.DataSession;
			if (!migrationBatchDataProvider.MigrationSession.Config.IsSupported(MigrationFeature.MultiBatch))
			{
				base.WriteError(new MigrationPermanentException(Strings.CompleteMigrationBatchNotSupported));
			}
			migrationBatchDataProvider.MigrationJob = base.GetAndValidateMigrationJob(true);
			if (migrationBatchDataProvider.MigrationJob == null)
			{
				MigrationObjectTaskBase<MigrationBatchIdParameter>.WriteJobNotFoundError(this, this.Identity.RawIdentity);
			}
			LocalizedString? localizedString;
			if (migrationBatchDataProvider.MigrationJob.IsPAW)
			{
				if (!migrationBatchDataProvider.MigrationJob.SupportsMultiBatchFinalization)
				{
					base.WriteError(new MigrationBatchCannotBeCompletedException(Strings.CompleteMigrationBatchNotSupported));
				}
			}
			else if (!migrationBatchDataProvider.MigrationJob.SupportsCompleting(out localizedString))
			{
				if (localizedString == null)
				{
					localizedString = new LocalizedString?(Strings.MigrationJobCannotBeCompleted);
				}
				base.WriteError(new MigrationBatchCannotBeCompletedException(localizedString.Value));
				migrationBatchDataProvider.MigrationJob = null;
			}
			if (base.Fields.IsModified("NotificationEmails"))
			{
				this.resolvedNotificationEmails = base.GetUpdatedNotificationEmails(this.NotificationEmails);
			}
			else
			{
				this.resolvedNotificationEmails = null;
			}
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			MigrationBatchDataProvider batchProvider = (MigrationBatchDataProvider)base.DataSession;
			bool isPAW = batchProvider.MigrationJob.IsPAW;
			Exception ex = null;
			try
			{
				if (isPAW)
				{
					this.DataObject.CompleteAfter = new DateTime?(DateTime.MinValue);
					this.DataObject.CompleteAfterUTC = new DateTime?(DateTime.MinValue);
					this.DataObject.SubscriptionSettingsModified = (DateTime)ExDateTime.UtcNow;
					MigrationHelper.RunUpdateOperation(delegate
					{
						batchProvider.MigrationJob.UpdateJob(batchProvider.MailboxProvider, false, this.DataObject);
					});
				}
				else
				{
					if (this.DataObject.MigrationType == MigrationType.PublicFolder)
					{
						this.VerifyLegacyPublicFolderDatabaseLocked();
					}
					MigrationHelper.RunUpdateOperation(delegate
					{
						batchProvider.MigrationJob.FinalizeJob(batchProvider.MailboxProvider, this.resolvedNotificationEmails);
					});
				}
				batchProvider.MigrationJob.ReportData.Append(Strings.MigrationReportJobCompletedByUser(base.ExecutingUserIdentityName));
			}
			catch (InvalidOperationException ex2)
			{
				ex = ex2;
			}
			if (ex != null)
			{
				base.WriteError(new MigrationPermanentException(Strings.MigrationCannotBeCompleted, ex));
			}
			if (isPAW)
			{
				MigrationObjectTaskBase<MigrationBatchIdParameter>.RegisterMigrationBatch(this, batchProvider.MailboxSession, base.CurrentOrganizationId, false, false);
			}
			base.InternalProcessRecord();
		}

		private void VerifyLegacyPublicFolderDatabaseLocked()
		{
			PublicFolderEndpoint publicFolderEndpoint = (PublicFolderEndpoint)this.DataObject.SourceEndpoint;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				IMailbox mailbox;
				if (publicFolderEndpoint != null)
				{
					mailbox = disposeGuard.Add<IMailbox>(publicFolderEndpoint.ConnectToSourceDatabase());
				}
				else
				{
					DatabaseIdParameter id = DatabaseIdParameter.Parse(this.DataObject.SourcePublicFolderDatabase);
					PublicFolderDatabase publicFolderDatabase = (PublicFolderDatabase)base.GetDataObject<PublicFolderDatabase>(id, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.DataObject.SourcePublicFolderDatabase)), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.DataObject.SourcePublicFolderDatabase)));
					mailbox = disposeGuard.Add<IMailbox>(PublicFolderEndpoint.ConnectToLocalSourceDatabase(publicFolderDatabase.ExchangeObjectId));
				}
				try
				{
					bool flag;
					mailbox.SetInTransitStatus(InTransitStatus.MoveSource, out flag);
					mailbox.SetInTransitStatus(InTransitStatus.NotInTransit, out flag);
				}
				catch (SourceMailboxAlreadyBeingMovedTransientException)
				{
					base.WriteError(new MigrationBatchCannotBeCompletedException(Strings.CompletePublicFolderMigrationBatchRequiresSourceLockDown));
				}
			}
		}

		private const string ParameterNotificationEmails = "NotificationEmails";

		private MultiValuedProperty<SmtpAddress> resolvedNotificationEmails;
	}
}
