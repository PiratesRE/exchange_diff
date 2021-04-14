using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Management.Migration
{
	[Cmdlet("Remove", "MigrationUser", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveMigrationUser : RemoveMigrationObjectTaskBase<MigrationUserIdParameter, MigrationUser>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		protected override string Action
		{
			get
			{
				return "Remove-MigrationUser";
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveMigrationUser(this.DataObject.Identity.ToString());
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			MigrationLogger.Initialize();
			MigrationLogContext.Current.Source = "Remove-MigrationUser";
			MigrationLogContext.Current.Organization = base.CurrentOrganizationId.OrganizationalUnit;
			return MigrationUserDataProvider.CreateDataProvider(this.Action, base.TenantGlobalCatalogSession, this.partitionMailbox, base.ExecutingUserIdentityName);
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			MigrationUserDataProvider migrationUserDataProvider = (MigrationUserDataProvider)base.DataSession;
			migrationUserDataProvider.ForceRemoval = this.Force;
			MigrationJob job = migrationUserDataProvider.GetJob(this.DataObject.BatchId);
			if (job != null && job.MigrationType == MigrationType.PublicFolder)
			{
				base.WriteError(new CannotRemoveMigrationUserFromPublicFolderBatchException());
			}
			if (this.Force)
			{
				return;
			}
			if (job == null)
			{
				base.WriteError(new CannotRemoveUserWithoutBatchException(this.DataObject.Identity.ToString()));
			}
			LocalizedString? localizedString;
			if (!job.IsPAW && !job.SupportsRemovingUsers(out localizedString))
			{
				base.WriteError(new CannotRemoveMigrationUserOnCurrentStateException(this.Identity.ToString(), job.JobName));
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			MigrationUserDataProvider migrationUserDataProvider = (MigrationUserDataProvider)base.DataSession;
			MigrationObjectTaskBase<MigrationBatchIdParameter>.RegisterMigrationBatch(this, migrationUserDataProvider.MailboxSession, base.CurrentOrganizationId, false, false);
		}
	}
}
