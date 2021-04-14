using System;
using System.Collections.Generic;
using Microsoft.Exchange.Assistants.Diagnostics;
using Microsoft.Exchange.Assistants.EventLog;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class TimeBasedDatabaseWindowJob : TimeBasedDatabaseJob
	{
		public TimeBasedDatabaseWindowJob(TimeBasedDatabaseDriver driver, List<MailboxData> queue, int notInterestingCount, int filteredCount, int failedFilteringCount, int totalOnDatabaseCount, PoisonMailboxControl poisonControl, PerformanceCountersPerDatabaseInstance databaseCounters) : base(driver, queue, poisonControl, databaseCounters)
		{
			this.notInterestingMailboxCount = notInterestingCount;
			this.filteredMailboxCount = filteredCount;
			this.failedFilteringMailboxCount = failedFilteringCount;
			this.totalOnDatabaseCount = totalOnDatabaseCount;
		}

		protected override void LogJobBegin(int initialPendingQueueCount)
		{
			base.StartTime = DateTime.UtcNow;
			base.LogEvent(AssistantsEventLogConstants.Tuple_TimeWindowBegin, null, new object[]
			{
				base.Assistant.Name,
				base.DatabaseInfo.DisplayName,
				initialPendingQueueCount
			});
			AssistantsLog.LogBeginJob(base.Assistant.NonLocalizedName, base.DatabaseInfo.DisplayName, initialPendingQueueCount);
		}

		protected override void LogJobEnd(int initialPendingQueueCount, int mailboxesProcessedSuccessfullyCount, int mailboxesProcessedFailureCount, int mailboxesFailedToOpenStoreSessionCount, int mailboxesProcessedSeparatelyCount, int mailboxesRetriedCount)
		{
			int num = initialPendingQueueCount - mailboxesProcessedSuccessfullyCount - mailboxesProcessedFailureCount - mailboxesFailedToOpenStoreSessionCount - mailboxesProcessedSeparatelyCount + mailboxesRetriedCount;
			base.EndTime = DateTime.UtcNow;
			AssistantEndWorkCycleCheckpointStatistics assistantEndWorkCycleCheckpointStatistics = new AssistantEndWorkCycleCheckpointStatistics
			{
				DatabaseName = base.DatabaseInfo.DisplayName,
				StartTime = base.StartTime,
				EndTime = base.EndTime,
				TotalMailboxCount = initialPendingQueueCount,
				ProcessedMailboxCount = mailboxesProcessedSuccessfullyCount,
				MailboxErrorCount = mailboxesProcessedFailureCount,
				FailedToOpenStoreSessionCount = mailboxesFailedToOpenStoreSessionCount,
				RetriedMailboxCount = mailboxesRetriedCount,
				MailboxesProcessedSeparatelyCount = mailboxesProcessedSeparatelyCount,
				MailboxRemainingCount = num
			};
			AssistantsLog.LogEndJobEvent(base.Assistant.NonLocalizedName, assistantEndWorkCycleCheckpointStatistics.FormatCustomData());
			if (mailboxesProcessedSuccessfullyCount == 0 && initialPendingQueueCount != 0)
			{
				base.LogEvent(AssistantsEventLogConstants.Tuple_DatabaseNotProcessedInTimeWindow, null, new object[]
				{
					base.Assistant.Name,
					base.DatabaseInfo.DisplayName,
					mailboxesProcessedFailureCount,
					mailboxesFailedToOpenStoreSessionCount,
					mailboxesRetriedCount,
					num
				});
				return;
			}
			base.LogEvent(AssistantsEventLogConstants.Tuple_TimeWindowEnd, null, new object[]
			{
				base.Assistant.Name,
				base.DatabaseInfo.DisplayName,
				base.EndTime.Subtract(base.StartTime),
				mailboxesProcessedSuccessfullyCount,
				mailboxesProcessedFailureCount,
				mailboxesProcessedSeparatelyCount,
				mailboxesFailedToOpenStoreSessionCount,
				mailboxesRetriedCount,
				num
			});
		}

		public new DiagnosticsSummaryJobWindow GetJobDiagnosticsSummary()
		{
			return new DiagnosticsSummaryJobWindow(this.totalOnDatabaseCount, base.InterestingMailboxCount, this.notInterestingMailboxCount, this.filteredMailboxCount, this.failedFilteringMailboxCount, base.OnDemandMailboxCount, base.StartTime, base.EndTime, base.GetJobDiagnosticsSummary());
		}

		private readonly int totalOnDatabaseCount;

		private readonly int notInterestingMailboxCount;

		private readonly int filteredMailboxCount;

		private readonly int failedFilteringMailboxCount;
	}
}
