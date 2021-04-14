using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Assistants.Diagnostics
{
	internal class DiagnosticsSummaryDatabase
	{
		public DiagnosticsSummaryJobWindow WindowJobStatistics { get; private set; }

		public DiagnosticsSummaryJob OnDemandJobsStatistics { get; private set; }

		public bool IsAssistantEnabled { get; private set; }

		public DateTime StartTime { get; private set; }

		public DiagnosticsSummaryDatabase() : this(true, DateTime.MinValue, new DiagnosticsSummaryJobWindow(), new DiagnosticsSummaryJob())
		{
		}

		public DiagnosticsSummaryDatabase(bool isAssistantEnabled, DateTime startTime, DiagnosticsSummaryJobWindow window, DiagnosticsSummaryJob demand)
		{
			ArgumentValidator.ThrowIfNull("window", window);
			ArgumentValidator.ThrowIfNull("demand", demand);
			this.IsAssistantEnabled = isAssistantEnabled;
			this.StartTime = startTime;
			this.WindowJobStatistics = window;
			this.OnDemandJobsStatistics = demand;
		}
	}
}
