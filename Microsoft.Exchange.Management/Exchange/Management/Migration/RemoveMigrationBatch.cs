using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Management.Migration
{
	[Cmdlet("Remove", "MigrationBatch", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveMigrationBatch : MigrationObjectTaskBase<MigrationBatchIdParameter>
	{
		public override string Action
		{
			get
			{
				return "RemoveMigrationBatch";
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (!this.Force && this.DataObject != null && this.DataObject.PendingCount > 0)
				{
					return Strings.ConfirmationMessageRemoveMigrationBatchWithPendingItems(this.DataObject.Identity.ToString(), this.DataObject.PendingCount);
				}
				if (this.DataObject != null)
				{
					return Strings.ConfirmationMessageRemoveMigrationBatch(this.DataObject.Identity.ToString());
				}
				return Strings.ConfirmationMessageRemoveMigrationBatch(this.Identity.ToString());
			}
		}

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

		protected override void InternalValidate()
		{
			MigrationBatchDataProvider migrationBatchDataProvider = (MigrationBatchDataProvider)base.DataSession;
			migrationBatchDataProvider.MigrationJob = base.GetAndValidateMigrationJob(false);
			if (!this.Force)
			{
				LocalizedString? localizedString;
				bool flag;
				if (migrationBatchDataProvider.MigrationJob.IsPAW)
				{
					flag = migrationBatchDataProvider.MigrationJob.SupportsFlag(MigrationFlags.Remove, out localizedString);
				}
				else
				{
					flag = migrationBatchDataProvider.MigrationJob.SupportsRemoving(out localizedString);
				}
				if (!flag)
				{
					if (localizedString == null)
					{
						localizedString = new LocalizedString?(Strings.MigrationJobCannotBeRemoved);
					}
					base.WriteError(new LocalizedException(localizedString.Value));
					migrationBatchDataProvider.MigrationJob = null;
				}
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
			try
			{
				MigrationHelper.RunUpdateOperation(delegate
				{
					if (this.Force)
					{
						batchProvider.MigrationJob.Delete(batchProvider.MailboxProvider, true);
						return;
					}
					if (batchProvider.MigrationJob.IsPAW)
					{
						batchProvider.MigrationJob.SetMigrationFlags(batchProvider.MailboxProvider, MigrationFlags.Remove);
						return;
					}
					batchProvider.MigrationJob.RemoveJob(batchProvider.MailboxProvider);
				});
			}
			catch (ObjectNotFoundException ex)
			{
				MigrationObjectTaskBase<MigrationBatchIdParameter>.WriteJobNotFoundError(this, this.Identity.RawIdentity, ex);
			}
			batchProvider.MigrationJob.ReportData.Append(Strings.MigrationReportJobRemoved(base.ExecutingUserIdentityName));
			MigrationObjectTaskBase<MigrationBatchIdParameter>.RegisterMigrationBatch(this, batchProvider.MailboxSession, base.CurrentOrganizationId, false, false);
			base.InternalProcessRecord();
		}
	}
}
