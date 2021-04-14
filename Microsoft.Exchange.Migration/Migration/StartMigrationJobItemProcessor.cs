using System;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	internal class StartMigrationJobItemProcessor : MigrationJobItemProcessorBase
	{
		public StartMigrationJobItemProcessor(MigrationJobItem migrationObject, IMigrationDataProvider dataProvider) : base(migrationObject, dataProvider)
		{
		}

		protected override MigrationJobItemProcessorResponse InternalProcess()
		{
			if (this.MigrationObject.State != MigrationState.Failed)
			{
				MigrationStage stage = this.MigrationObject.WorkflowPosition.Stage;
				if (stage <= MigrationStage.Validation)
				{
					if (stage == MigrationStage.Discovery || stage == MigrationStage.Validation)
					{
						goto IL_72;
					}
				}
				else
				{
					if (stage == MigrationStage.Injection)
					{
						goto IL_72;
					}
					if (stage == MigrationStage.Processing)
					{
						base.StepHandler.Start(base.SnapshotId);
						goto IL_72;
					}
				}
				throw new ArgumentException("Don't know how to process stage: " + this.MigrationObject.WorkflowPosition.Stage);
			}
			IL_72:
			return MigrationJobItemProcessorResponse.Create(MigrationProcessorResult.Completed, null, null, null, null, null, false, null);
		}

		protected override MigrationJobItemProcessorResponse ApplyResponse(MigrationJobItemProcessorResponse response)
		{
			if (response.Result == MigrationProcessorResult.Completed)
			{
				MigrationUserStatus status = this.MigrationObject.Status;
				MigrationUserStatus initialStatus = this.MigrationObject.WorkflowPosition.GetInitialStatus();
				MigrationWorkflowPosition position = this.MigrationObject.WorkflowPosition;
				if (this.MigrationObject.State == MigrationState.Failed)
				{
					position = this.MigrationObject.MigrationJob.Workflow.GetRestartPosition(this.MigrationObject.WorkflowPosition);
				}
				this.MigrationObject.SetStatus(this.DataProvider, initialStatus, MigrationState.Active, new MigrationFlags?(this.MigrationObject.Flags & ~MigrationFlags.Start), position, response.DelayTime, null, null, null, false, null);
				return MigrationJobItemProcessorResponse.Create(MigrationProcessorResult.Working, null, null, null, null, null, false, MigrationCountCache.MigrationStatusChange.CreateStatusChange(status, initialStatus));
			}
			return base.ApplyResponse(response);
		}
	}
}
