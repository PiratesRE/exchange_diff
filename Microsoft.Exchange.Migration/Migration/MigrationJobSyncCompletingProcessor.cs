using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	internal class MigrationJobSyncCompletingProcessor : JobSyncCompletingProcessor
	{
		protected override bool IsBatchSyncedReport
		{
			get
			{
				return true;
			}
		}

		protected override MigrationUserStatus[] JobItemStatesForSuccess
		{
			get
			{
				return MigrationJobSyncCompletingProcessor.JobItemStatusForSuccess;
			}
		}

		protected override MigrationUserStatus[] JobItemStatesForFailure
		{
			get
			{
				return MigrationJob.JobItemStatusForBatchCompletionErrors;
			}
		}

		internal static MigrationJobSyncCompletingProcessor CreateProcessor(MigrationType type)
		{
			if (type <= MigrationType.ExchangeOutlookAnywhere)
			{
				if (type == MigrationType.IMAP)
				{
					return new IMAPJobSyncCompletingProcessor();
				}
				if (type != MigrationType.ExchangeOutlookAnywhere)
				{
					goto IL_30;
				}
			}
			else if (type != MigrationType.ExchangeRemoteMove && type != MigrationType.ExchangeLocalMove && type != MigrationType.PublicFolder)
			{
				goto IL_30;
			}
			return new MigrationJobSyncCompletingProcessor();
			IL_30:
			throw new ArgumentException("Invalid MigrationType " + type);
		}

		internal override bool Validate()
		{
			return base.Job != null && base.Job.Status == MigrationJobStatus.SyncCompleting;
		}

		internal override MigrationJobStatus GetNextStageStatus()
		{
			if (base.Job.TryAutoRetryStartedJob(base.DataProvider))
			{
				return MigrationJobStatus.SyncInitializing;
			}
			return MigrationJobStatus.SyncCompleted;
		}

		protected override LegacyMigrationJobProcessorResponse Process(bool scheduleNewWork)
		{
			MigrationJobReportingCursor migrationJobReportingCursor;
			return base.Process(out migrationJobReportingCursor);
		}

		protected override string GetTemplateName(bool areErrorsPresent)
		{
			return "BatchCompletedReport.htm";
		}

		protected override string GetEmailSubject(bool errorsPresent)
		{
			if (base.Job.IsCancelled)
			{
				switch (base.Job.JobCancellationStatus)
				{
				case JobCancellationStatus.NotCancelled:
					break;
				case JobCancellationStatus.CancelledByUserRequest:
					return Strings.MigrationBatchCancelledByUser(base.Job.JobName);
				case JobCancellationStatus.CancelledDueToHighFailureCount:
					return Strings.MigrationBatchCancelledBySystem(base.Job.JobName);
				default:
					throw new InvalidOperationException("Unsupported job cancellation status " + base.Job.JobCancellationStatus);
				}
			}
			if (errorsPresent)
			{
				return Strings.MigrationBatchCompletionReportMailErrorHeader(base.Job.JobName);
			}
			return Strings.MigrationBatchCompletionReportMailHeader(base.Job.JobName);
		}

		protected override IDictionary<string, string> GetTemplateData(MigrationJobReportingCursor migrationReportData, string successReportLink, string failureReportLink)
		{
			MigrationObjectsCount migrationErrorCount = migrationReportData.MigrationErrorCount;
			MigrationObjectsCount migrationSuccessCount = migrationReportData.MigrationSuccessCount;
			bool hasErrors = migrationReportData.HasErrors;
			string text = JobSyncCompletingProcessor.GetStatisticsSummaryMessage(migrationReportData);
			Dictionary<string, string> dictionary = new Dictionary<string, string>(30);
			dictionary.Add("{StatisticsReportLink}", successReportLink);
			if (!string.IsNullOrEmpty(text))
			{
				text = string.Format(CultureInfo.InvariantCulture, "<br />{0}", new object[]
				{
					text
				});
			}
			dictionary.Add("{StatisticsSummaryMessage}", text);
			dictionary.Add("{ErrorReportLink}", failureReportLink);
			string licensingWarningSection = base.GetLicensingWarningSection();
			dictionary.Add("{MoacWarningSection}", licensingWarningSection);
			if (!hasErrors)
			{
				dictionary.Add("{imghtml}", string.Empty);
				dictionary.Add("{LabelCouldntMigrate}", string.Empty);
				dictionary.Add("{ErrorSummaryMessage}", string.Empty);
			}
			else
			{
				dictionary.Add("{imghtml}", "<img width=\"16\" height=\"16\" src=\"cid:ErrorImage\" />");
				dictionary.Add("{LabelCouldntMigrate}", Strings.LabelCouldntMigrate);
				dictionary.Add("{ErrorSummaryMessage}", migrationErrorCount.ToString());
			}
			if (base.Job.ShouldAutoRetryStartedJob)
			{
				dictionary.Add("{ExtraNotes}", Strings.LabelAutoRetry(base.Job.MaxAutoRunCount.Value - base.Job.AutoRunCount));
			}
			else
			{
				dictionary.Add("{ExtraNotes}", string.Empty);
			}
			dictionary.Add("{ReportHeader}", this.GetEmailSubject(hasErrors));
			dictionary.Add("{LabelStartedByUser}", Strings.LabelSubmittedByUser);
			dictionary.Add("{StartedByUser}", base.Job.SubmittedByUser);
			dictionary.Add("{LabelStartDateTime}", Strings.LabelStartDateTime);
			dictionary.Add("{LabelRunTime}", Strings.LabelRunTime);
			ExTimeZone userTimeZone = base.Job.UserTimeZone;
			if (base.Job.StartTime != null)
			{
				dictionary.Add("{StartDateTime}", userTimeZone.ConvertDateTime(base.Job.StartTime.Value).ToString("D", CultureInfo.CurrentCulture));
				TimeSpan timeSpan = ExDateTime.UtcNow - base.Job.StartTime.Value;
				dictionary.Add("{RunTime}", ItemStateTransitionHelper.LocalizeTimeSpan(timeSpan));
			}
			else
			{
				dictionary.Add("{StartDateTime}", string.Empty);
				dictionary.Add("{RunTime}", string.Empty);
			}
			MigrationObjectsCount migrationObjectsCount = migrationSuccessCount;
			dictionary.Add("{LabelSynced}", Strings.LabelSynced);
			dictionary.Add("{CompletedData}", migrationObjectsCount.ToString());
			MigrationObjectsCount migrationObjectsCount2 = migrationErrorCount + migrationObjectsCount;
			dictionary.Add("{LabelTotalRows}", Strings.LabelTotalRows);
			dictionary.Add("{TotalRows}", migrationObjectsCount2.ToString());
			dictionary.Add("{LabelLogMailFooter}", Strings.LabelLogMailFooter);
			return dictionary;
		}

		protected override IEnumerable<MigrationJobItem> GetJobItemsToProcess(string startingIndex, int maxCount)
		{
			IEnumerable<MigrationJobItem> result;
			if (string.IsNullOrEmpty(startingIndex))
			{
				result = MigrationJobItem.GetAllSortedByIdentifier(base.DataProvider, base.Job, maxCount);
			}
			else
			{
				result = MigrationJobItem.GetNextJobItems(base.DataProvider, base.Job, startingIndex, maxCount);
			}
			return result;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MigrationJobSyncCompletingProcessor>(this);
		}

		private const string ReportTemplate = "BatchCompletedReport.htm";

		private static readonly MigrationUserStatus[] JobItemStatusForSuccess = new MigrationUserStatus[]
		{
			MigrationUserStatus.Synced,
			MigrationUserStatus.IncrementalSyncing
		};
	}
}
