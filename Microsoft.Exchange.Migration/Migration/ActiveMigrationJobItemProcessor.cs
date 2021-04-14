using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	internal class ActiveMigrationJobItemProcessor : SnapshotMigrationJobItemProcessorBase
	{
		public ActiveMigrationJobItemProcessor(MigrationJobItem migrationObject, IMigrationDataProvider dataProvider) : base(migrationObject, dataProvider)
		{
		}

		protected override MigrationState StateToSetWhenStageCompleted
		{
			get
			{
				return MigrationState.Active;
			}
		}

		protected override MigrationProcessorResult ResultForProcessorActionCompleted
		{
			get
			{
				return MigrationProcessorResult.Completed;
			}
		}

		protected override MigrationJobItemProcessorResponse InternalProcess()
		{
			if (this.MigrationObject.MigrationJob.Workflow.ShouldDelay(this.MigrationObject.WorkflowPosition, this.MigrationObject.MigrationJob))
			{
				return MigrationJobItemProcessorResponse.Create(MigrationProcessorResult.Waiting, new TimeSpan?(ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("SyncMigrationProcessorMinWaitingJobDelay")), null, null, null, null, false, null);
			}
			MigrationStage stage = this.MigrationObject.WorkflowPosition.Stage;
			if (stage <= MigrationStage.Validation)
			{
				if (stage == MigrationStage.Discovery)
				{
					return this.Discovery();
				}
				if (stage == MigrationStage.Validation)
				{
					return this.Validation();
				}
			}
			else
			{
				if (stage == MigrationStage.Injection)
				{
					return this.Injection();
				}
				if (stage == MigrationStage.Processing)
				{
					return this.Processing();
				}
			}
			throw new ArgumentException("Don't know how to process stage: " + this.MigrationObject.WorkflowPosition.Stage);
		}

		private MigrationJobItemProcessorResponse Discovery()
		{
			MailboxData mailboxDataFromSmtpAddress = this.DataProvider.ADProvider.GetMailboxDataFromSmtpAddress(this.MigrationObject.LocalMailboxIdentifier, false, base.StepHandler.ExpectMailboxData);
			IStepSettings stepSettings = base.StepHandler.Discover(this.MigrationObject, mailboxDataFromSmtpAddress);
			return MigrationJobItemProcessorResponse.Create(MigrationProcessorResult.Completed, null, null, mailboxDataFromSmtpAddress, null, stepSettings, false, null);
		}

		private MigrationJobItemProcessorResponse Validation()
		{
			base.StepHandler.Validate(this.MigrationObject);
			return MigrationJobItemProcessorResponse.Create(MigrationProcessorResult.Completed, null, null, null, null, null, false, null);
		}

		private MigrationJobItemProcessorResponse Injection()
		{
			if (!base.StepHandler.CanProcess(this.MigrationObject))
			{
				return MigrationJobItemProcessorResponse.Create(MigrationProcessorResult.Waiting, null, null, null, null, null, false, null);
			}
			IStepSnapshot stepSnapshot = base.StepHandler.Inject(this.MigrationObject);
			return MigrationJobItemProcessorResponse.Create(MigrationProcessorResult.Completed, null, null, null, stepSnapshot, null, stepSnapshot != null, null);
		}

		private MigrationJobItemProcessorResponse Processing()
		{
			if (!base.StepHandler.CanProcess(this.MigrationObject))
			{
				return MigrationJobItemProcessorResponse.Create(MigrationProcessorResult.Waiting, null, null, null, null, null, false, null);
			}
			bool updated;
			IStepSnapshot snapshot = base.StepHandler.Process(base.SnapshotId, this.MigrationObject, out updated);
			return base.ResponseFromSnapshot(snapshot, updated);
		}
	}
}
