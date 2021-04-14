using System;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	internal abstract class MigrationJobProcessorBase : MigrationHierarchyProcessorBase<MigrationJob, MigrationJobItem, StoreObjectId, MigrationJobProcessorResponse>
	{
		protected MigrationJobProcessorBase(MigrationJob migrationObject, IMigrationDataProvider dataProvider) : base(migrationObject, dataProvider)
		{
		}

		protected override MigrationProcessorResponse DefaultCorruptedChildResponse
		{
			get
			{
				return MigrationProcessorResponse.Create(MigrationProcessorResult.Failed, null, null);
			}
		}

		protected override int? MaxChildObjectsToProcessCount
		{
			get
			{
				return new int?(ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("ProcessingBatchSize"));
			}
		}

		protected override MigrationJobProcessorResponse PerformPoisonDetection()
		{
			if (this.MigrationObject.PoisonCount >= ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("MigrationPoisonedCountThreshold"))
			{
				return this.HandlePermanentException(new MigrationPoisonCountThresholdExceededException());
			}
			MigrationJobProcessorResponse result;
			try
			{
				this.MigrationObject.UpdatePoisonCount(this.DataProvider, this.MigrationObject.PoisonCount + 1);
				result = MigrationJobProcessorResponse.Create(MigrationProcessorResult.Working, null, null, null, null, null);
			}
			catch (SaveConflictException ex)
			{
				MigrationApplication.NotifyOfTransientException(ex, "MigrationApplication::RunJobOperation - UpdatePoisonCount: job " + this.MigrationObject);
				result = MigrationJobProcessorResponse.Create(MigrationProcessorResult.Waiting, new TimeSpan?(TimeSpan.Zero), null, null, null, null);
			}
			catch (LocalizedException ex2)
			{
				if (CommonUtils.IsTransientException(ex2))
				{
					MigrationApplication.NotifyOfTransientException(ex2, "MigrationApplication::RunJobOperation - UpdatePoisonCount: job " + this.MigrationObject);
					throw;
				}
				MigrationApplication.NotifyOfCriticalError(ex2, "MigrationApplication::RunJobOperation - UpdatePoisonCount => job = " + this.MigrationObject);
				throw new MigrationTransientException(ex2.LocalizedString, ex2);
			}
			return result;
		}

		protected override bool TryLoad(StoreObjectId childId, out MigrationJobItem child)
		{
			try
			{
				MigrationJobObjectCache migrationJobObjectCache = new MigrationJobObjectCache(this.DataProvider);
				migrationJobObjectCache.PreSeed(this.MigrationObject);
				child = MigrationJobItem.Load(this.DataProvider, childId, migrationJobObjectCache, false);
			}
			catch (ObjectNotFoundException ex)
			{
				MigrationApplication.NotifyOfIgnoredException(ex, "Couldn't find job-item: " + childId);
				child = null;
				return false;
			}
			return child != null;
		}

		protected override void SetContext()
		{
			this.originalCurrentCulture = Thread.CurrentThread.CurrentCulture;
			this.originalCurrentUICulture = Thread.CurrentThread.CurrentUICulture;
			if (this.MigrationObject.AdminCulture.IsNeutralCulture)
			{
				throw new UnsupportedAdminCultureException(this.MigrationObject.AdminCulture.ToString());
			}
			Thread.CurrentThread.CurrentCulture = this.MigrationObject.AdminCulture;
			Thread.CurrentThread.CurrentUICulture = this.MigrationObject.AdminCulture;
		}

		protected override void RestoreContext()
		{
			Thread.CurrentThread.CurrentCulture = this.originalCurrentCulture;
			Thread.CurrentThread.CurrentUICulture = this.originalCurrentUICulture;
			MigrationLogContext.Current.Job = null;
		}

		protected override MigrationJobProcessorResponse HandlePermanentException(LocalizedException ex)
		{
			if (ex is MigrationDataCorruptionException)
			{
				MigrationApplication.NotifyOfCorruptJob(ex, "MigrationJobProcessorBase::Process => job " + this.MigrationObject);
				MigrationHelper.SendFriendlyWatson(ex, true, this.MigrationObject.ToString());
			}
			else
			{
				MigrationApplication.NotifyOfCriticalError(ex, "MigrationJobProcessorBase::Process => job " + this.MigrationObject);
			}
			return MigrationJobProcessorResponse.Create(MigrationProcessorResult.Failed, null, ex, null, null, null);
		}

		protected override MigrationJobProcessorResponse HandleTransientException(LocalizedException ex)
		{
			if (MigrationApplication.HasTransientErrorReachedThreshold<MigrationJobStatus>(this.MigrationObject.StatusData))
			{
				MigrationLogger.Log(MigrationEventType.Warning, "Transient error reached threshold for job " + this.MigrationObject, new object[0]);
				return this.HandlePermanentException(new TooManyTransientFailuresException(this.MigrationObject.JobName, ex));
			}
			MigrationApplication.NotifyOfTransientException(ex, "MigrationJobItemProcessorBase::Process => job " + this.MigrationObject);
			return MigrationJobProcessorResponse.Create(MigrationProcessorResult.Waiting, new TimeSpan?(ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("SyncMigrationProcessorTransientErrorRunDelay")), ex, null, null, null);
		}

		protected override MigrationJobProcessorResponse ApplyResponse(MigrationJobProcessorResponse response)
		{
			switch (response.Result)
			{
			case MigrationProcessorResult.Working:
				this.MigrationObject.SetStatus(this.DataProvider, MigrationJobStatus.SyncStarting, MigrationState.Active, null, null, response.DelayTime, null, response.LastProcessedRow, null, response.ChildStatusChanges, response.ClearPoison, null, response.ProcessingDuration);
				return response;
			case MigrationProcessorResult.Waiting:
				this.MigrationObject.SetStatus(this.DataProvider, MigrationJobStatus.SyncStarting, MigrationState.Waiting, null, null, response.DelayTime, response.Error, response.LastProcessedRow, null, response.ChildStatusChanges, response.ClearPoison, null, response.ProcessingDuration);
				return response;
			case MigrationProcessorResult.Completed:
				throw new NotSupportedException("expected " + response.Result + " to be handled at a different level... ");
			case MigrationProcessorResult.Failed:
				this.MigrationObject.SetStatus(this.DataProvider, MigrationJobStatus.Failed, MigrationState.Failed, null, null, null, response.Error, response.LastProcessedRow, null, response.ChildStatusChanges, response.ClearPoison, null, response.ProcessingDuration);
				return response;
			}
			throw new NotSupportedException("Not expected to see " + response.Result + " result for a job processor...");
		}

		private CultureInfo originalCurrentCulture;

		private CultureInfo originalCurrentUICulture;
	}
}
