using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Assistants.Diagnostics
{
	internal class DiagnosticsSummaryJob
	{
		public int ProcessingCount { get; private set; }

		public int ProcessedSuccessfullyCount { get; private set; }

		public int ProcessedFailureCount { get; private set; }

		public int FailedToOpenStoreSessionCount { get; private set; }

		public int RetriedCount { get; private set; }

		public int QueuedCount { get; private set; }

		public DiagnosticsSummaryJob()
		{
		}

		public DiagnosticsSummaryJob(int processing, int processedSuccessfully, int processedFailure, int failedToOpenStoreSession, int retriedCount, int queued)
		{
			this.ProcessingCount = processing;
			this.ProcessedSuccessfullyCount = processedSuccessfully;
			this.ProcessedFailureCount = processedFailure;
			this.FailedToOpenStoreSessionCount = failedToOpenStoreSession;
			this.RetriedCount = retriedCount;
			this.QueuedCount = queued;
		}

		public void AddMoreSummary(DiagnosticsSummaryJob mbxDiagnosticsSummary)
		{
			ArgumentValidator.ThrowIfNull("mbxDiagnosticsSummary", mbxDiagnosticsSummary);
			this.ProcessingCount += mbxDiagnosticsSummary.ProcessingCount;
			this.ProcessedSuccessfullyCount += mbxDiagnosticsSummary.ProcessedSuccessfullyCount;
			this.ProcessedFailureCount += mbxDiagnosticsSummary.ProcessedFailureCount;
			this.FailedToOpenStoreSessionCount += mbxDiagnosticsSummary.FailedToOpenStoreSessionCount;
			this.RetriedCount += mbxDiagnosticsSummary.RetriedCount;
			this.QueuedCount += mbxDiagnosticsSummary.QueuedCount;
		}
	}
}
