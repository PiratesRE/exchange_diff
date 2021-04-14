using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TimeBasedAssistants
{
	internal class TimeBasedAssistantsOutOfSlaProbeScheduled : TimeBasedAssistantsOutOfSlaProbe
	{
		protected override TimeBasedAssistantsOutOfSlaDecisionMaker CreateDecisionMakerInstance()
		{
			return new TbaOutOfSlaDecisionMakerScheduled();
		}
	}
}
