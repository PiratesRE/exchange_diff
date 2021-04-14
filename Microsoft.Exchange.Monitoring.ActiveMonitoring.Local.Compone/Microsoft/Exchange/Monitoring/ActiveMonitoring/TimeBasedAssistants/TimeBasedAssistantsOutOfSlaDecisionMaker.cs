using System;
using System.Collections.Generic;
using Microsoft.Exchange.Assistants;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TimeBasedAssistants
{
	internal abstract class TimeBasedAssistantsOutOfSlaDecisionMaker : TimeBasedAssistantsLastNCriteria
	{
		protected TimeBasedAssistantsOutOfSlaDecisionMaker(int maxWorkcycleLengthMinutesToLookAt, TbaOutOfSlaAlertType alertType, IEnumerable<TimeBasedAssistantsLastNCriteria.WorkcycleAlertClassification> alertThresholds) : base(alertThresholds)
		{
			ArgumentValidator.ThrowIfZeroOrNegative("maxWorkcycleLengthMinutesToLookAt", maxWorkcycleLengthMinutesToLookAt);
			this.maxWorkcycleLengthMinutesToLookAt = maxWorkcycleLengthMinutesToLookAt;
			this.alertType = alertType;
		}

		protected override bool IsSatisfied(AssistantInfo assistantInfo, MailboxDatabase database, WindowJob[] history)
		{
			ArgumentValidator.ThrowIfNull("assistantInfo", assistantInfo);
			ArgumentValidator.ThrowIfNull("database", database);
			ArgumentValidator.ThrowIfNull("history", history);
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
			IMailboxAssistantSettings @object = snapshot.MailboxAssistants.GetObject<IMailboxAssistantSettings>(assistantInfo.AssistantName);
			bool slaMonitoringEnabled = @object.SlaMonitoringEnabled;
			if (!slaMonitoringEnabled || assistantInfo.WorkcycleLength.TotalMinutes > (double)this.maxWorkcycleLengthMinutesToLookAt)
			{
				return true;
			}
			float goalPercent = 0f;
			switch (this.alertType)
			{
			case TbaOutOfSlaAlertType.Urgent:
				goalPercent = @object.SlaUrgentThreshold;
				break;
			case TbaOutOfSlaAlertType.Scheduled:
				goalPercent = @object.SlaNonUrgentThreshold;
				break;
			}
			ArgumentValidator.ThrowIfOutOfRange<float>("goalPercent", goalPercent, 0f, 1f);
			Array.Sort<WindowJob>(history, (WindowJob x1, WindowJob x2) => x1.StartTime.CompareTo(x2.StartTime));
			Func<WindowJob, bool> alertCondition = delegate(WindowJob wc)
			{
				int interestingMailboxCount = wc.InterestingMailboxCount;
				int completedMailboxCount = wc.CompletedMailboxCount;
				int failedMailboxCount = wc.FailedMailboxCount;
				int movedToOnDemandMailboxCount = wc.MovedToOnDemandMailboxCount;
				int retriedMailboxCount = wc.RetriedMailboxCount;
				int num = completedMailboxCount + failedMailboxCount + movedToOnDemandMailboxCount - retriedMailboxCount;
				int num2 = interestingMailboxCount - num;
				double num3 = (num == 0 && num2 == 0) ? 1.0 : ((double)num / (double)interestingMailboxCount);
				return num3 < (double)goalPercent;
			};
			return base.IsSatisfiedForLastN(assistantInfo, database, history, alertCondition);
		}

		private readonly TbaOutOfSlaAlertType alertType;

		private readonly int maxWorkcycleLengthMinutesToLookAt = int.MaxValue;
	}
}
