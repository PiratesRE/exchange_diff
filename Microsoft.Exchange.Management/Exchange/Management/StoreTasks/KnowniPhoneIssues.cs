using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	internal class KnowniPhoneIssues : AnalysisRule
	{
		internal KnowniPhoneIssues()
		{
			base.Name = "KnowniPhoneIssues";
			base.Message = Strings.Error_KnowniPhoneIssues;
			base.AlertLevel = AnalysisRule.RuleAlertLevel.Warning;
			base.RequiredProperties = new List<PropertyDefinition>
			{
				CalendarItemBaseSchema.ClientInfoString,
				CalendarItemBaseSchema.CalendarLogTriggerAction
			};
		}

		protected override void AnalyzeLog(LinkedListNode<CalendarLogAnalysis> logNode)
		{
			logNode.Value.GetPropertyValue(CalendarItemBaseSchema.CalendarLogTriggerAction);
			string propertyValue = logNode.Value.GetPropertyValue(CalendarItemBaseSchema.ClientInfoString);
			foreach (string value in this.problomaticAgents)
			{
				if (propertyValue.IndexOf(value, StringComparison.InvariantCultureIgnoreCase) != -1)
				{
					logNode.Value.AddAlert(this);
					return;
				}
			}
		}

		private string[] problomaticAgents = new string[]
		{
			"iphone",
			"ipad"
		};
	}
}
