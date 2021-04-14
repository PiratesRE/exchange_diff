using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Management.Migration
{
	[Cmdlet("Stop", "MigrationBatch", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class StopMigrationBatch : MigrationObjectTaskBase<MigrationBatchIdParameter>
	{
		public override string Action
		{
			get
			{
				return "StopMigrationBatch";
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.DataObject != null)
				{
					return Strings.ConfirmationMessageStopMigrationBatch(this.DataObject.Identity.ToString());
				}
				return Strings.ConfirmationMessageStopMigrationBatch(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			MigrationBatchDataProvider migrationBatchDataProvider = (MigrationBatchDataProvider)base.DataSession;
			migrationBatchDataProvider.MigrationJob = base.GetAndValidateMigrationJob(true);
			LocalizedString? localizedString;
			bool flag;
			if (migrationBatchDataProvider.MigrationJob.IsPAW)
			{
				flag = migrationBatchDataProvider.MigrationJob.SupportsFlag(MigrationFlags.Stop, out localizedString);
			}
			else
			{
				flag = migrationBatchDataProvider.MigrationJob.SupportsStopping(out localizedString);
			}
			if (!flag)
			{
				if (localizedString == null)
				{
					localizedString = new LocalizedString?(Strings.MigrationJobCannotBeStopped);
				}
				base.WriteError(new MigrationPermanentException(localizedString.Value));
				migrationBatchDataProvider.MigrationJob = null;
			}
			if (migrationBatchDataProvider.MigrationJob == null)
			{
				MigrationObjectTaskBase<MigrationBatchIdParameter>.WriteJobNotFoundError(this, this.Identity.RawIdentity);
			}
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			MigrationBatchDataProvider batchProvider = (MigrationBatchDataProvider)base.DataSession;
			if (batchProvider.MigrationJob.IsPAW)
			{
				batchProvider.MigrationJob.SetMigrationFlags(batchProvider.MailboxProvider, MigrationFlags.Stop);
			}
			else
			{
				MigrationHelper.RunUpdateOperation(delegate
				{
					batchProvider.MigrationJob.StopJob(batchProvider.MailboxProvider, batchProvider.MigrationSession.Config, JobCancellationStatus.CancelledByUserRequest);
				});
			}
			MigrationObjectTaskBase<MigrationBatchIdParameter>.RegisterMigrationBatch(this, batchProvider.MailboxSession, base.CurrentOrganizationId, false, false);
			base.InternalProcessRecord();
		}
	}
}
