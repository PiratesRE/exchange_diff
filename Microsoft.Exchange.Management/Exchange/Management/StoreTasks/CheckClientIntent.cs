using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	internal class CheckClientIntent : AnalysisRule
	{
		internal CheckClientIntent()
		{
			base.Name = "CheckClientIntent";
			base.Message = Strings.Error_CheckClientIntent;
			base.AlertLevel = AnalysisRule.RuleAlertLevel.Warning;
			base.RequiredProperties = new List<PropertyDefinition>
			{
				CalendarItemBaseSchema.ClientInfoString,
				CalendarItemBaseSchema.ClientIntent
			};
		}

		protected override void AnalyzeLog(LinkedListNode<CalendarLogAnalysis> logNode)
		{
			object obj = null;
			object obj2 = null;
			if (logNode.Value.InternalProperties.TryGetValue(CalendarItemBaseSchema.ClientInfoString, out obj2))
			{
				if (string.IsNullOrEmpty(obj2 as string))
				{
					base.Message = Strings.Error_CheckClientInfo;
				}
				if (logNode.Value.InternalProperties.TryGetValue(CalendarItemBaseSchema.ClientIntent, out obj))
				{
					return;
				}
			}
			logNode.Value.AddAlert(this);
		}
	}
}
