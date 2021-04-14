using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TimeBasedAssistants
{
	internal class TimeBasedAssistantsOutOfSlaProbeUrgent : TimeBasedAssistantsOutOfSlaProbe
	{
		protected override TimeBasedAssistantsOutOfSlaDecisionMaker CreateDecisionMakerInstance()
		{
			return new TbaOutOfSlaDecisionMakerUrgent();
		}
	}
}
