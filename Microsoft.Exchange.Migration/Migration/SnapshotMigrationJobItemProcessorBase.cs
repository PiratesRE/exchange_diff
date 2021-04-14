using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	internal abstract class SnapshotMigrationJobItemProcessorBase : MigrationJobItemProcessorBase
	{
		protected SnapshotMigrationJobItemProcessorBase(MigrationJobItem migrationObject, IMigrationDataProvider dataProvider) : base(migrationObject, dataProvider)
		{
		}

		protected abstract MigrationState StateToSetWhenStageCompleted { get; }

		protected abstract MigrationProcessorResult ResultForProcessorActionCompleted { get; }

		protected virtual MigrationFlags FlagsToClearOnSteadyState
		{
			get
			{
				return MigrationFlags.None;
			}
		}

		protected override MigrationJobItemProcessorResponse ApplyResponse(MigrationJobItemProcessorResponse response)
		{
			MigrationUserStatus status = this.MigrationObject.Status;
			MigrationFlags value = this.MigrationObject.Flags & ~this.FlagsToClearOnSteadyState;
			MigrationUserStatus migrationUserStatus;
			if (response.Result == MigrationProcessorResult.Completed)
			{
				MigrationWorkflowPosition nextPosition = this.MigrationObject.MigrationJob.Workflow.GetNextPosition(this.MigrationObject.WorkflowPosition, this.MigrationObject.SupportedSteps);
				if (nextPosition != null)
				{
					if (this.StateToSetWhenStageCompleted == MigrationState.Stopped)
					{
						migrationUserStatus = MigrationUserStatus.Stopped;
					}
					else
					{
						migrationUserStatus = nextPosition.GetInitialStatus();
					}
					this.MigrationObject.SetStatus(this.DataProvider, migrationUserStatus, this.StateToSetWhenStageCompleted, new MigrationFlags?(value), nextPosition, response.DelayTime, response.MailboxData, response.Settings, response.Snapshot, response.Updated, null);
					response = MigrationJobItemProcessorResponse.Create(MigrationProcessorResult.Working, null, null, null, null, null, false, null);
				}
				else
				{
					migrationUserStatus = ((response.Error == null) ? MigrationUserStatus.Completed : MigrationUserStatus.CompletedWithWarnings);
					this.MigrationObject.SetStatus(this.DataProvider, migrationUserStatus, MigrationState.Completed, new MigrationFlags?(MigrationFlags.None), null, response.DelayTime, response.MailboxData, response.Settings, response.Snapshot, response.Updated, response.Error);
				}
			}
			else if (response.Result == MigrationProcessorResult.Suspended)
			{
				migrationUserStatus = MigrationUserStatus.Stopped;
				this.MigrationObject.SetStatus(this.DataProvider, migrationUserStatus, MigrationState.Stopped, new MigrationFlags?(value), null, response.DelayTime, response.MailboxData, response.Settings, response.Snapshot, response.Updated, null);
			}
			else if (response.Result == MigrationProcessorResult.Failed)
			{
				migrationUserStatus = MigrationUserStatus.Failed;
				this.MigrationObject.SetStatus(this.DataProvider, migrationUserStatus, MigrationState.Failed, new MigrationFlags?(value), null, response.DelayTime, response.MailboxData, response.Settings, response.Snapshot, response.Updated, response.Error);
			}
			else if (response.Result == MigrationProcessorResult.Working)
			{
				migrationUserStatus = base.StepHandler.ResolvePresentationStatus(this.MigrationObject.Flags, response.Snapshot);
				this.MigrationObject.SetStatus(this.DataProvider, migrationUserStatus, MigrationState.Active, null, null, response.DelayTime, response.MailboxData, response.Settings, response.Snapshot, response.Updated, null);
			}
			else
			{
				if (response.Result != MigrationProcessorResult.Waiting)
				{
					return base.ApplyResponse(response);
				}
				migrationUserStatus = base.StepHandler.ResolvePresentationStatus(this.MigrationObject.Flags, response.Snapshot);
				this.MigrationObject.SetStatus(this.DataProvider, migrationUserStatus, MigrationState.Waiting, null, null, response.DelayTime, response.MailboxData, response.Settings, response.Snapshot, response.Updated, response.Error);
			}
			response.StatusChange = MigrationCountCache.MigrationStatusChange.CreateStatusChange(status, migrationUserStatus);
			return response;
		}

		protected MigrationJobItemProcessorResponse ResponseFromSnapshot(IStepSnapshot snapshot, bool updated = false)
		{
			if (snapshot == null)
			{
				return MigrationJobItemProcessorResponse.Create(this.ResultForProcessorActionCompleted, null, null, null, null, null, updated, null);
			}
			LocalizedString? errorMessage = snapshot.ErrorMessage;
			if (errorMessage == null || errorMessage.Value.IsEmpty)
			{
				errorMessage = new LocalizedString?(Strings.UnknownMigrationError);
			}
			LocalizedException error = new MigrationPermanentException(errorMessage.Value);
			switch (snapshot.Status)
			{
			case SnapshotStatus.InProgress:
				return MigrationJobItemProcessorResponse.Create(MigrationProcessorResult.Working, null, null, null, snapshot, null, updated, null);
			case SnapshotStatus.Failed:
				return MigrationJobItemProcessorResponse.Create(MigrationProcessorResult.Failed, null, error, null, snapshot, null, updated, null);
			case SnapshotStatus.AutoSuspended:
			case SnapshotStatus.Suspended:
				return MigrationJobItemProcessorResponse.Create(MigrationProcessorResult.Suspended, null, null, null, snapshot, null, updated, null);
			case SnapshotStatus.CompletedWithWarning:
				return MigrationJobItemProcessorResponse.Create(MigrationProcessorResult.Completed, null, error, null, snapshot, null, updated, null);
			case SnapshotStatus.Finalized:
				return MigrationJobItemProcessorResponse.Create(MigrationProcessorResult.Completed, null, null, null, snapshot, null, updated, null);
			case SnapshotStatus.Synced:
				return MigrationJobItemProcessorResponse.Create(MigrationProcessorResult.Waiting, new TimeSpan?(ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("SyncMigrationProcessorSyncedJobItemDelay")), null, null, snapshot, null, updated, null);
			}
			throw new NotSupportedException("Status " + snapshot.Status + " not expected...");
		}
	}
}
