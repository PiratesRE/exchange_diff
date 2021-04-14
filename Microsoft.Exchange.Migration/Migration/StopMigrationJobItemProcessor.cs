using System;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	internal class StopMigrationJobItemProcessor : SnapshotMigrationJobItemProcessorBase
	{
		public StopMigrationJobItemProcessor(MigrationJobItem migrationObject, IMigrationDataProvider dataProvider) : base(migrationObject, dataProvider)
		{
		}

		protected override MigrationState StateToSetWhenStageCompleted
		{
			get
			{
				return MigrationState.Stopped;
			}
		}

		protected override MigrationProcessorResult ResultForProcessorActionCompleted
		{
			get
			{
				return MigrationProcessorResult.Suspended;
			}
		}

		protected override MigrationFlags FlagsToClearOnSteadyState
		{
			get
			{
				return MigrationFlags.Stop;
			}
		}

		protected override MigrationJobItemProcessorResponse InternalProcess()
		{
			IStepSnapshot snapshot = null;
			ISnapshotId snapshotId = base.SnapshotId;
			MigrationStage stage = this.MigrationObject.WorkflowPosition.Stage;
			if (stage <= MigrationStage.Validation)
			{
				if (stage != MigrationStage.Discovery && stage != MigrationStage.Validation)
				{
					goto IL_58;
				}
			}
			else if (stage != MigrationStage.Injection)
			{
				if (stage != MigrationStage.Processing)
				{
					goto IL_58;
				}
				snapshot = base.StepHandler.Stop(snapshotId);
				goto IL_7D;
			}
			if (snapshotId != null)
			{
				snapshot = base.StepHandler.Stop(snapshotId);
				goto IL_7D;
			}
			goto IL_7D;
			IL_58:
			throw new ArgumentException("Don't know how to process stage: " + this.MigrationObject.WorkflowPosition.Stage);
			IL_7D:
			return base.ResponseFromSnapshot(snapshot, false);
		}
	}
}
