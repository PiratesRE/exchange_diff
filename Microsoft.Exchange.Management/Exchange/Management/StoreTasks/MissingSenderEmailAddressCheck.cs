using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	internal class MissingSenderEmailAddressCheck : AnalysisRule
	{
		internal MissingSenderEmailAddressCheck()
		{
			base.Name = "MissingSenderEmail";
			base.Message = Strings.Error_MissingSenderEmail;
			base.AlertLevel = AnalysisRule.RuleAlertLevel.Error;
			base.RequiredProperties = new List<PropertyDefinition>
			{
				CalendarItemBaseSchema.SenderEmailAddress
			};
		}

		protected override void AnalyzeLog(LinkedListNode<CalendarLogAnalysis> logNode)
		{
			object obj = null;
			if (logNode.Value.InternalProperties.TryGetValue(CalendarItemBaseSchema.SenderEmailAddress, out obj) && !string.IsNullOrEmpty(obj as string))
			{
				return;
			}
			logNode.Value.AddAlert(this);
		}
	}
}
