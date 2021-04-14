using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TimeBasedAssistants
{
	internal class AssistantInfo
	{
		public string AssistantName { get; set; }

		public TimeSpan WorkcycleLength { get; set; }

		public TimeSpan WorkcycleCheckpointLength { get; set; }
	}
}
