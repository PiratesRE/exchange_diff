using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Management.Migration
{
	[Cmdlet("Start", "MigrationBatch", SupportsShouldProcess = true)]
	public sealed class StartMigrationBatch : MigrationObjectTaskBase<MigrationBatchIdParameter>
	{
		public override string Action
		{
			get
			{
				return "StartMigrationBatch";
			}
		}

		[Parameter(Mandatory = false)]
		public new SwitchParameter Validate
		{
			get
			{
				return (SwitchParameter)(base.Fields["Validate"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Validate"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.DataObject != null)
				{
					return Strings.ConfirmationMessageStartMigrationBatch(this.DataObject.Identity.ToString());
				}
				return Strings.ConfirmationMessageStartMigrationBatch(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			MigrationBatchDataProvider migrationBatchDataProvider = (MigrationBatchDataProvider)base.DataSession;
			migrationBatchDataProvider.MigrationJob = base.GetAndValidateMigrationJob(true);
			bool isPAW = migrationBatchDataProvider.MigrationJob.IsPAW;
			if (migrationBatchDataProvider.MigrationJob == null)
			{
				MigrationObjectTaskBase<MigrationBatchIdParameter>.WriteJobNotFoundError(this, this.Identity.RawIdentity);
			}
			LocalizedString? localizedString;
			bool flag;
			if (isPAW)
			{
				flag = migrationBatchDataProvider.MigrationJob.SupportsFlag(MigrationFlags.Start, out localizedString);
			}
			else
			{
				flag = migrationBatchDataProvider.MigrationJob.SupportsStarting(out localizedString);
			}
			if (!flag)
			{
				if (localizedString == null)
				{
					localizedString = new LocalizedString?(Strings.MigrationOperationFailed);
				}
				base.WriteError(new MigrationPermanentException(localizedString.Value));
				migrationBatchDataProvider.MigrationJob = null;
			}
			if (this.Validate && (isPAW || (migrationBatchDataProvider.MigrationJob.MigrationType != MigrationType.ExchangeLocalMove && migrationBatchDataProvider.MigrationJob.MigrationType != MigrationType.ExchangeRemoteMove)))
			{
				base.WriteError(new ValidateNotSupportedException());
			}
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			MigrationBatchDataProvider migrationBatchDataProvider = (MigrationBatchDataProvider)base.DataSession;
			migrationBatchDataProvider.MigrationJob.ReportData.Append(Strings.MigrationReportJobStarted(base.ExecutingUserIdentityName));
			if (migrationBatchDataProvider.MigrationJob.IsPAW)
			{
				migrationBatchDataProvider.MigrationJob.SetMigrationFlags(migrationBatchDataProvider.MailboxProvider, MigrationFlags.Start);
			}
			else
			{
				MigrationBatchFlags migrationBatchFlags = migrationBatchDataProvider.MigrationJob.BatchFlags;
				if (this.Validate)
				{
					migrationBatchFlags |= MigrationBatchFlags.UseAdvancedValidation;
				}
				else
				{
					migrationBatchFlags &= ~MigrationBatchFlags.UseAdvancedValidation;
				}
				MigrationObjectTaskBase<MigrationBatchIdParameter>.StartJob(this, migrationBatchDataProvider, migrationBatchDataProvider.MigrationJob, null, migrationBatchFlags);
			}
			MigrationObjectTaskBase<MigrationBatchIdParameter>.RegisterMigrationBatch(this, migrationBatchDataProvider.MailboxSession, base.CurrentOrganizationId, false, false);
			base.InternalProcessRecord();
		}
	}
}
