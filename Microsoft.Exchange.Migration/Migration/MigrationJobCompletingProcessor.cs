using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	internal class MigrationJobCompletingProcessor : JobSyncCompletingProcessor
	{
		protected override bool IsBatchSyncedReport
		{
			get
			{
				return false;
			}
		}

		protected override MigrationUserStatus[] JobItemStatesForSuccess
		{
			get
			{
				return MigrationJobCompletingProcessor.JobItemStatusForSuccess;
			}
		}

		protected override MigrationUserStatus[] JobItemStatesForFailure
		{
			get
			{
				return MigrationJobCompletingProcessor.JobItemsStatusForCompletionErrors;
			}
		}

		internal static MigrationJobCompletingProcessor CreateProcessor(MigrationType type, bool isStaged)
		{
			if (type <= MigrationType.BulkProvisioning)
			{
				if (type == MigrationType.IMAP || type == MigrationType.ExchangeOutlookAnywhere)
				{
					throw new NotSupportedException("IMAP/Exchange not supported in Completing state");
				}
				if (type == MigrationType.BulkProvisioning)
				{
					throw new NotSupportedException("Bulk Provisioning not supported in Completing state");
				}
			}
			else if (type == MigrationType.ExchangeRemoteMove || type == MigrationType.ExchangeLocalMove || type == MigrationType.PublicFolder)
			{
				return new MigrationJobCompletingProcessor();
			}
			throw new ArgumentException("Invalid MigrationType " + type);
		}

		internal override bool Validate()
		{
			return base.Job != null && base.Job.Status == MigrationJobStatus.Completing;
		}

		internal override MigrationJobStatus GetNextStageStatus()
		{
			MigrationUtil.AssertOrThrow(this.hasErrors != null, "GetNextStageStatus should only be called after processing is completed for the processor.", new object[0]);
			if (!base.Job.TryAutoRetryCompletedJob(base.DataProvider))
			{
				return MigrationJobStatus.Completed;
			}
			if (base.Job.AutoComplete)
			{
				return MigrationJobStatus.SyncInitializing;
			}
			if (base.Job.MigrationType == MigrationType.PublicFolder)
			{
				return MigrationJobStatus.CompletionStarting;
			}
			return MigrationJobStatus.CompletionInitializing;
		}

		protected override LegacyMigrationJobProcessorResponse Process(bool scheduleNewWork)
		{
			MigrationJobReportingCursor migrationJobReportingCursor;
			LegacyMigrationJobProcessorResponse result = base.Process(out migrationJobReportingCursor);
			this.hasErrors = new bool?(migrationJobReportingCursor.HasErrors);
			return result;
		}

		protected override string GetTemplateName(bool areErrorsPresent)
		{
			return "MigrationCompletedReport.htm";
		}

		protected override string GetEmailSubject(bool areErrorsPresent)
		{
			if (!areErrorsPresent)
			{
				return Strings.MigrationFinalizationReportMailHeader(base.Job.JobName);
			}
			return Strings.MigrationFinalizationReportMailErrorHeader(base.Job.JobName);
		}

		protected override IDictionary<string, string> GetTemplateData(MigrationJobReportingCursor migrationReportData, string successReportLink, string failureReportLink)
		{
			MigrationObjectsCount migrationErrorCount = migrationReportData.MigrationErrorCount;
			MigrationObjectsCount migrationSuccessCount = migrationReportData.MigrationSuccessCount;
			bool flag = migrationReportData.HasErrors;
			Dictionary<string, string> dictionary = new Dictionary<string, string>(15);
			string text = JobSyncCompletingProcessor.GetStatisticsSummaryMessage(migrationReportData);
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
			if (!flag)
			{
				dictionary.Add("{ReportHeader}", Strings.MigrationFinalizationReportMailHeader(base.Job.JobName));
				dictionary.Add("{imghtml}", string.Empty);
				dictionary.Add("{ErrorSummaryMessage}", string.Empty);
			}
			else
			{
				dictionary.Add("{ReportHeader}", Strings.MigrationFinalizationReportMailErrorHeader(base.Job.JobName));
				dictionary.Add("{imghtml}", "<img width=\"16\" height=\"16\" src=\"cid:ErrorImage\" />");
				dictionary.Add("{ErrorSummaryMessage}", migrationErrorCount.ToString());
			}
			if (base.Job.ShouldAutoRetryCompletedJob)
			{
				dictionary.Add("{ExtraNotes}", Strings.LabelAutoRetry(base.Job.MaxAutoRunCount.Value - base.Job.AutoRunCount));
			}
			else
			{
				dictionary.Add("{ExtraNotes}", string.Empty);
			}
			dictionary.Add("{RetryFinalizationMessage}", (!flag) ? string.Empty : Strings.FinalizationErrorSummaryRetryMessage);
			MigrationObjectsCount migrationObjectsCount = migrationSuccessCount;
			dictionary.Add("{LabelCompletedMailboxes}", Strings.LabelSynced);
			dictionary.Add("{CompletedMailboxes}", migrationObjectsCount.ToString());
			MigrationObjectsCount migrationObjectsCount2 = migrationObjectsCount + migrationErrorCount;
			dictionary.Add("{LabelTotalMailboxes}", Strings.LabelTotalMailboxes);
			dictionary.Add("{TotalMailboxes}", migrationObjectsCount2.ToString());
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
			return DisposeTracker.Get<MigrationJobCompletingProcessor>(this);
		}

		private const string ReportTemplate = "MigrationCompletedReport.htm";

		internal static readonly MigrationUserStatus[] JobItemsStatusForCompletionErrors = new MigrationUserStatus[]
		{
			MigrationUserStatus.CompletionFailed,
			MigrationUserStatus.IncrementalFailed,
			MigrationUserStatus.CompletedWithWarnings,
			MigrationUserStatus.Corrupted,
			MigrationUserStatus.Stopped,
			MigrationUserStatus.IncrementalStopped,
			MigrationUserStatus.Failed
		};

		private static readonly MigrationUserStatus[] JobItemStatusForSuccess = new MigrationUserStatus[]
		{
			MigrationUserStatus.Completed
		};

		private bool? hasErrors;
	}
}
