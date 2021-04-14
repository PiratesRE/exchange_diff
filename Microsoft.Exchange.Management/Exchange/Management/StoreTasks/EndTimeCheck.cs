using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	internal class EndTimeCheck : AnalysisRule
	{
		internal EndTimeCheck()
		{
			base.Name = "EndDateCheck";
			base.Message = Strings.Error_EndDateCheck;
			base.AlertLevel = AnalysisRule.RuleAlertLevel.Warning;
			base.RequiredProperties = new List<PropertyDefinition>
			{
				CalendarItemInstanceSchema.EndTime
			};
		}

		protected override void AnalyzeLog(LinkedListNode<CalendarLogAnalysis> logNode)
		{
			object obj = null;
			if (logNode.Value.InternalProperties.TryGetValue(CalendarItemInstanceSchema.EndTime, out obj))
			{
				ExDateTime t = (ExDateTime)obj;
				if (t < EndTimeCheck.LatestValidDate && t > EndTimeCheck.EarliestValidDate)
				{
					return;
				}
			}
			logNode.Value.AddAlert(this);
		}

		private static ExDateTime EarliestValidDate = new ExDateTime(ExTimeZone.CurrentTimeZone, 1995, 1, 1);

		private static ExDateTime LatestValidDate = new ExDateTime(ExTimeZone.CurrentTimeZone, 2025, 1, 1);
	}
}
