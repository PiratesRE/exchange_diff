using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TimeBasedAssistants
{
	internal abstract class TimeBasedAssistantsLastNCriteria : TimeBasedAssistantsCriteria
	{
		protected TimeBasedAssistantsLastNCriteria(IEnumerable<TimeBasedAssistantsLastNCriteria.WorkcycleAlertClassification> alertThresholds)
		{
			ArgumentValidator.ThrowIfNull("alertThresholds", alertThresholds);
			this.alertThresholds = alertThresholds.ToList<TimeBasedAssistantsLastNCriteria.WorkcycleAlertClassification>();
			this.FailedCriteriaRecordList = new List<TimeBasedAssistantsLastNCriteria.FailedCriteriaRecord>();
		}

		public List<TimeBasedAssistantsLastNCriteria.FailedCriteriaRecord> FailedCriteriaRecordList { get; private set; }

		protected bool IsSatisfiedForLastN(AssistantInfo assistantInfo, MailboxDatabase database, WindowJob[] history, Func<WindowJob, bool> alertCondition)
		{
			ArgumentValidator.ThrowIfNull("assistantInfo", assistantInfo);
			ArgumentValidator.ThrowIfNull("database", database);
			ArgumentValidator.ThrowIfNull("history", history);
			ArgumentValidator.ThrowIfNull("alertCondition", alertCondition);
			int checkpointHistoryEvaluationThreshold = this.GetCheckpointHistoryEvaluationThreshold(assistantInfo);
			int num = history.Length;
			if (num == 0 || num < checkpointHistoryEvaluationThreshold)
			{
				return true;
			}
			int num2 = num - checkpointHistoryEvaluationThreshold;
			List<WindowJob> source = (num2 > 0) ? history.Skip(num2).ToList<WindowJob>() : history.ToList<WindowJob>();
			bool flag = source.Any((WindowJob windowJob) => !alertCondition(windowJob));
			if (!flag)
			{
				this.FailedCriteriaRecordList.Add(new TimeBasedAssistantsLastNCriteria.FailedCriteriaRecord
				{
					AssistantName = assistantInfo.AssistantName,
					DatabaseGuid = database.Guid
				});
			}
			return flag;
		}

		private int GetCheckpointHistoryEvaluationThreshold(AssistantInfo assistantInfo)
		{
			ArgumentValidator.ThrowIfNull("assistantInfo", assistantInfo);
			int workcycleMinutes = (int)assistantInfo.WorkcycleLength.TotalMinutes;
			int num = (int)assistantInfo.WorkcycleCheckpointLength.TotalMinutes;
			List<TimeBasedAssistantsLastNCriteria.WorkcycleAlertClassification> list = (from t in this.alertThresholds
			where t.WorkcycleMinutesMax >= workcycleMinutes
			select t).ToList<TimeBasedAssistantsLastNCriteria.WorkcycleAlertClassification>();
			if (!list.Any<TimeBasedAssistantsLastNCriteria.WorkcycleAlertClassification>() || num == 0)
			{
				return 1;
			}
			TimeBasedAssistantsLastNCriteria.WorkcycleAlertClassification workcycleAlertClassification = list.First<TimeBasedAssistantsLastNCriteria.WorkcycleAlertClassification>();
			int num2 = workcycleAlertClassification.WorkcycleMinutesMax - workcycleMinutes;
			foreach (TimeBasedAssistantsLastNCriteria.WorkcycleAlertClassification workcycleAlertClassification2 in list)
			{
				int num3 = workcycleAlertClassification2.WorkcycleMinutesMax - workcycleMinutes;
				if (num3 < num2)
				{
					num2 = num3;
					workcycleAlertClassification = workcycleAlertClassification2;
				}
			}
			return Math.Max(1, workcycleAlertClassification.AlertTimeThresholdMinutes / num);
		}

		private readonly List<TimeBasedAssistantsLastNCriteria.WorkcycleAlertClassification> alertThresholds;

		internal class FailedCriteriaRecord
		{
			public string AssistantName { get; set; }

			public Guid DatabaseGuid { get; set; }
		}

		internal struct WorkcycleAlertClassification
		{
			public int WorkcycleMinutesMax { get; set; }

			public int AlertTimeThresholdMinutes { get; set; }
		}
	}
}
