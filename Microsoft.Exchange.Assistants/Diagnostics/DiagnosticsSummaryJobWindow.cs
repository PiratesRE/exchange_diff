using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Assistants.Diagnostics
{
	internal class DiagnosticsSummaryJobWindow
	{
		public int TotalOnDatabaseCount { get; private set; }

		public int InterestingCount { get; private set; }

		public int NotInterestingCount { get; private set; }

		public int FilteredMailboxCount { get; private set; }

		public int FailedFilteringCount { get; private set; }

		public int ProcessedSeparatelyCount { get; private set; }

		public DiagnosticsSummaryJob DiagnosticsSummaryJob { get; private set; }

		public DateTime StartTime { get; private set; }

		public DateTime EndTime { get; private set; }

		public DiagnosticsSummaryJobWindow() : this(0, 0, 0, 0, 0, 0, DateTime.MinValue, DateTime.MinValue, new DiagnosticsSummaryJob())
		{
		}

		public DiagnosticsSummaryJobWindow(int totalOnDatabase, int interesting, int notInteresting, int filtered, int failedFiltering, int processedSeparately, DateTime start, DateTime end, DiagnosticsSummaryJob summaryJob)
		{
			ArgumentValidator.ThrowIfNull("summaryJob", summaryJob);
			this.TotalOnDatabaseCount = totalOnDatabase;
			this.InterestingCount = interesting;
			this.NotInterestingCount = notInteresting;
			this.FilteredMailboxCount = filtered;
			this.FailedFilteringCount = failedFiltering;
			this.ProcessedSeparatelyCount = processedSeparately;
			this.StartTime = start;
			this.EndTime = end;
			this.DiagnosticsSummaryJob = summaryJob;
		}
	}
}
