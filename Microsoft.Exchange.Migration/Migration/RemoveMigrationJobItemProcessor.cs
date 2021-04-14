using System;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	internal class RemoveMigrationJobItemProcessor : MigrationJobItemProcessorBase
	{
		public RemoveMigrationJobItemProcessor(MigrationJobItem migrationObject, IMigrationDataProvider dataProvider) : base(migrationObject, dataProvider)
		{
		}

		protected override MigrationJobItemProcessorResponse InternalProcess()
		{
			if (this.MigrationObject.State == MigrationState.Disabled)
			{
				return MigrationJobItemProcessorResponse.Create(MigrationProcessorResult.Completed, null, null, null, null, null, false, null);
			}
			ISnapshotId snapshotId = base.SnapshotId;
			MigrationStage stage = this.MigrationObject.WorkflowPosition.Stage;
			if (stage <= MigrationStage.Validation)
			{
				if (stage != MigrationStage.Discovery && stage != MigrationStage.Validation)
				{
					goto IL_EE;
				}
			}
			else if (stage != MigrationStage.Injection)
			{
				if (stage != MigrationStage.Processing)
				{
					goto IL_EE;
				}
				if (snapshotId != null)
				{
					base.StepHandler.Delete(snapshotId);
				}
				return MigrationJobItemProcessorResponse.Create(MigrationProcessorResult.Completed, null, null, null, null, null, false, null);
			}
			if (snapshotId != null)
			{
				CommonUtils.CatchKnownExceptions(delegate
				{
					this.StepHandler.Delete(snapshotId);
				}, delegate(Exception ex)
				{
					MigrationApplication.NotifyOfIgnoredException(ex, "Error clearing unexpected subscription for job-item: " + this.MigrationObject);
				});
			}
			return MigrationJobItemProcessorResponse.Create(MigrationProcessorResult.Completed, null, null, null, null, null, false, null);
			IL_EE:
			throw new ArgumentException("Don't know how to process stage: " + this.MigrationObject.WorkflowPosition.Stage);
		}

		protected override MigrationJobItemProcessorResponse ApplyResponse(MigrationJobItemProcessorResponse response)
		{
			if (response.Result == MigrationProcessorResult.Completed)
			{
				MigrationUserStatus status = this.MigrationObject.Status;
				this.MigrationObject.Delete(this.DataProvider);
				return MigrationJobItemProcessorResponse.Create(MigrationProcessorResult.Deleted, null, null, null, null, null, false, MigrationCountCache.MigrationStatusChange.CreateRemoval(status));
			}
			return base.ApplyResponse(response);
		}
	}
}
