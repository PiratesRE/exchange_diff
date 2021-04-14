using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	internal class MissingOrganizerEmailAddressCheck : AnalysisRule
	{
		internal MissingOrganizerEmailAddressCheck()
		{
			base.Name = "MissingOrganizerEmail";
			base.Message = Strings.Error_MissingOrganizerEmail;
			base.AlertLevel = AnalysisRule.RuleAlertLevel.Error;
			base.RequiredProperties = new List<PropertyDefinition>
			{
				CalendarItemBaseSchema.OrganizerEmailAddress
			};
		}

		protected override void AnalyzeLog(LinkedListNode<CalendarLogAnalysis> logNode)
		{
			object obj = null;
			if (logNode.Value.InternalProperties.TryGetValue(CalendarItemBaseSchema.OrganizerEmailAddress, out obj) && !string.IsNullOrEmpty(obj as string))
			{
				return;
			}
			logNode.Value.AddAlert(this);
		}
	}
}
