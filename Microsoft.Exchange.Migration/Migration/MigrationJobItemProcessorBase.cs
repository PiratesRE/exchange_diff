using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	internal abstract class MigrationJobItemProcessorBase : MigrationProcessorBase<MigrationJobItem, MigrationJobItemProcessorResponse>
	{
		protected MigrationJobItemProcessorBase(MigrationJobItem migrationObject, IMigrationDataProvider dataProvider) : base(migrationObject, dataProvider)
		{
			this.StepHandler = MigrationServiceFactory.Instance.CreateStepHandler(this.MigrationObject.WorkflowPosition, dataProvider, this.MigrationObject.MigrationJob);
			this.SnapshotId = MigrationServiceFactory.Instance.GetStepSnapshotId(this.MigrationObject.WorkflowPosition, this.MigrationObject);
		}

		private protected IStepHandler StepHandler { protected get; private set; }

		private protected ISnapshotId SnapshotId { protected get; private set; }

		protected override MigrationJobItemProcessorResponse HandlePermanentException(LocalizedException ex)
		{
			if (ex is MigrationDataCorruptionException)
			{
				MigrationApplication.NotifyOfCorruptJob(ex, "MigrationJobItemProcessorBase::Process => job-item " + this.MigrationObject);
				MigrationHelper.SendFriendlyWatson(ex, true, this.MigrationObject.ToString());
			}
			else
			{
				MigrationApplication.NotifyOfCriticalError(ex, "MigrationJobItemProcessorBase::Process => job-item " + this.MigrationObject);
			}
			return MigrationJobItemProcessorResponse.Create(MigrationProcessorResult.Failed, null, ex, null, null, null, false, null);
		}

		protected override MigrationJobItemProcessorResponse HandleTransientException(LocalizedException ex)
		{
			if (MigrationApplication.HasTransientErrorReachedThreshold<MigrationUserStatus>(this.MigrationObject.StatusData))
			{
				MigrationLogger.Log(MigrationEventType.Warning, "Transient error reached threshold for job-item " + this.MigrationObject, new object[0]);
				return this.HandlePermanentException(ex);
			}
			MigrationApplication.NotifyOfTransientException(ex, "MigrationJobItemProcessorBase::Process => job-item " + this.MigrationObject);
			return MigrationJobItemProcessorResponse.Create(MigrationProcessorResult.Waiting, null, ex, null, null, null, false, null);
		}

		protected override MigrationJobItemProcessorResponse PerformPoisonDetection()
		{
			return MigrationJobItemProcessorResponse.Create(MigrationProcessorResult.Working, null, null, null, null, null, false, null);
		}

		protected override void SetContext()
		{
		}

		protected override void RestoreContext()
		{
		}

		protected override MigrationJobItemProcessorResponse ApplyResponse(MigrationJobItemProcessorResponse response)
		{
			MigrationUserStatus status = this.MigrationObject.Status;
			MigrationUserStatus migrationUserStatus;
			switch (response.Result)
			{
			case MigrationProcessorResult.Working:
				migrationUserStatus = this.StepHandler.ResolvePresentationStatus(this.MigrationObject.Flags, null);
				this.MigrationObject.SetStatus(this.DataProvider, migrationUserStatus, MigrationState.Active, null, null, response.DelayTime, null, null, null, false, null);
				goto IL_135;
			case MigrationProcessorResult.Waiting:
				migrationUserStatus = this.StepHandler.ResolvePresentationStatus(this.MigrationObject.Flags, null);
				this.MigrationObject.SetStatus(this.DataProvider, migrationUserStatus, MigrationState.Waiting, null, null, response.DelayTime, null, null, null, false, response.Error);
				goto IL_135;
			case MigrationProcessorResult.Failed:
				migrationUserStatus = MigrationUserStatus.Failed;
				this.MigrationObject.SetStatus(this.DataProvider, migrationUserStatus, MigrationState.Failed, new MigrationFlags?(MigrationFlags.None), null, null, null, null, null, false, response.Error);
				goto IL_135;
			case MigrationProcessorResult.Deleted:
			case MigrationProcessorResult.Suspended:
				throw new NotSupportedException("Not expected to see " + response.Result + " result for a job-item processor at this level...");
			}
			throw new NotSupportedException("expected " + response.Result + " to be handled at a different level... ");
			IL_135:
			response.StatusChange = MigrationCountCache.MigrationStatusChange.CreateStatusChange(status, migrationUserStatus);
			return response;
		}

		internal static MigrationJobItemProcessorResponse SetFlags(IMigrationDataProvider dataProvider, MigrationJobItem jobItem, MigrationFlags flags, MigrationProcessorResult result)
		{
			MigrationUserStatus status = jobItem.Status;
			jobItem.SetMigrationFlags(dataProvider, flags);
			return MigrationJobItemProcessorResponse.Create(result, null, null, null, null, null, false, MigrationCountCache.MigrationStatusChange.CreateStatusChange(status, jobItem.Status));
		}
	}
}
