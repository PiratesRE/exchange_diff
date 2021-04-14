using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TimeBasedAssistants
{
	internal sealed class TbaOutOfSlaDecisionMakerUrgent : TimeBasedAssistantsOutOfSlaDecisionMaker
	{
		public TbaOutOfSlaDecisionMakerUrgent() : base(TbaOutOfSlaDecisionMakerUrgent.maxWorkcycleLengthMinutesToLookAt, TbaOutOfSlaAlertType.Urgent, TbaOutOfSlaDecisionMakerUrgent.workcycleAlertClassifications)
		{
		}

		private static readonly int maxWorkcycleLengthMinutesToLookAt = (int)TimeSpan.FromHours(8.0).TotalMinutes;

		private static readonly TimeBasedAssistantsLastNCriteria.WorkcycleAlertClassification[] workcycleAlertClassifications = new TimeBasedAssistantsLastNCriteria.WorkcycleAlertClassification[]
		{
			new TimeBasedAssistantsLastNCriteria.WorkcycleAlertClassification
			{
				WorkcycleMinutesMax = (int)TimeSpan.FromHours(8.0).TotalMinutes,
				AlertTimeThresholdMinutes = (int)TimeSpan.FromHours(8.0).TotalMinutes
			}
		};
	}
}
