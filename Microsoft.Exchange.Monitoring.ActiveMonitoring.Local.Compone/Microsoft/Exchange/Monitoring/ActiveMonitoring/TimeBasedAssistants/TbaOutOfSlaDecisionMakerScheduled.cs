using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TimeBasedAssistants
{
	internal sealed class TbaOutOfSlaDecisionMakerScheduled : TimeBasedAssistantsOutOfSlaDecisionMaker
	{
		public TbaOutOfSlaDecisionMakerScheduled() : base(TbaOutOfSlaDecisionMakerScheduled.maxWorkcycleLengthMinutesToLookAt, TbaOutOfSlaAlertType.Scheduled, TbaOutOfSlaDecisionMakerScheduled.workcycleAlertClassifications)
		{
		}

		private static readonly int maxWorkcycleLengthMinutesToLookAt = (int)TimeSpan.FromHours(24.0).TotalMinutes;

		private static readonly TimeBasedAssistantsLastNCriteria.WorkcycleAlertClassification[] workcycleAlertClassifications = new TimeBasedAssistantsLastNCriteria.WorkcycleAlertClassification[]
		{
			new TimeBasedAssistantsLastNCriteria.WorkcycleAlertClassification
			{
				WorkcycleMinutesMax = (int)TimeSpan.FromHours(24.0).TotalMinutes,
				AlertTimeThresholdMinutes = (int)TimeSpan.FromHours(12.0).TotalMinutes
			}
		};
	}
}
