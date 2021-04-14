using System;
using System.Collections.Generic;
using Microsoft.Exchange.Assistants.EventLog;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class TimeBasedDatabaseDemandJob : TimeBasedDatabaseJob
	{
		public TimeBasedDatabaseDemandJob(TimeBasedDatabaseDriver driver, MailboxData mailboxData, PoisonMailboxControl poisonControl, PerformanceCountersPerDatabaseInstance databaseCounters) : base(driver, new List<MailboxData>(new MailboxData[]
		{
			mailboxData
		}), poisonControl, databaseCounters)
		{
		}

		protected override void LogJobBegin(int initialPendingQueueCount)
		{
			base.StartTime = DateTime.UtcNow;
			base.LogEvent(AssistantsEventLogConstants.Tuple_TimeDemandJobBegin, null, new object[]
			{
				base.Assistant.Name,
				base.DatabaseInfo.DisplayName,
				initialPendingQueueCount
			});
		}

		protected override void LogJobEnd(int initialPendingQueueCount, int mailboxesProcessedSuccessfullyCount, int mailboxesProcessedFailureCount, int mailboxesFailedToOpenStoreSessionCount, int mailboxesProcessedSeparatelyCount, int mailboxesRetriedCount)
		{
			base.LogEvent(AssistantsEventLogConstants.Tuple_TimeDemandJobEnd, null, new object[]
			{
				base.Assistant.Name,
				base.DatabaseInfo.DisplayName,
				mailboxesProcessedSuccessfullyCount,
				initialPendingQueueCount,
				mailboxesProcessedFailureCount,
				mailboxesFailedToOpenStoreSessionCount,
				mailboxesRetriedCount
			});
		}
	}
}
