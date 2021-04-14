using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	internal class MessageClassCheck : AnalysisRule
	{
		internal MessageClassCheck()
		{
			base.Name = "MessageClassFilter";
			base.Message = string.Empty;
			base.AlertLevel = AnalysisRule.RuleAlertLevel.Info;
			base.RequiredProperties = new List<PropertyDefinition>
			{
				StoreObjectSchema.ItemClass,
				CalendarItemBaseSchema.CalendarLogTriggerAction
			};
		}

		protected override void AnalyzeLog(LinkedListNode<CalendarLogAnalysis> logNode)
		{
			string propertyValue = logNode.Value.GetPropertyValue(StoreObjectSchema.ItemClass);
			string propertyValue2 = logNode.Value.GetPropertyValue(CalendarItemBaseSchema.CalendarLogTriggerAction);
			if (string.IsNullOrEmpty(propertyValue))
			{
				base.Message = Strings.Error_MessageClassFilter;
				base.AlertLevel = AnalysisRule.RuleAlertLevel.Error;
				logNode.Value.AddAlert(this);
				return;
			}
			if ("IPM.Appointment IPM.Schedule.Meeting.Request IPM.Schedule.Meeting.Canceled IPM.Schedule.Meeting.Resp.Pos IPM.Schedule.Meeting.Resp.Neg".IndexOf(propertyValue) != -1)
			{
				string a;
				if ((a = propertyValue2) != null)
				{
					if (a == "Create")
					{
						base.Message = Strings.Info_MessageItemHasBeenCreated;
						goto IL_B1;
					}
					if (a == "MoveToDeletedItems")
					{
						base.Message = Strings.Info_MessageItemHasBeenDeleted;
						goto IL_B1;
					}
				}
				base.Message = Strings.Info_MessageItemHasBeenUpdated;
				IL_B1:
				logNode.Value.AddAlert(this);
			}
		}

		private const string ValidMessageClasses = "IPM.Appointment IPM.Schedule.Meeting.Request IPM.Schedule.Meeting.Canceled IPM.Schedule.Meeting.Resp.Pos IPM.Schedule.Meeting.Resp.Neg";
	}
}
