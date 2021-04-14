using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	internal class SentRepresentingChangeCheck : AnalysisRule
	{
		internal SentRepresentingChangeCheck()
		{
			base.Name = "SentRepresentingChangeCheck";
			base.AlertLevel = AnalysisRule.RuleAlertLevel.Error;
			base.RequiredProperties = new List<PropertyDefinition>
			{
				CalendarItemBaseSchema.OrganizerEmailAddress,
				ItemSchema.SentRepresentingDisplayName,
				StoreObjectSchema.ItemClass
			};
		}

		protected override void AnalyzeLog(LinkedListNode<CalendarLogAnalysis> logNode)
		{
			string propertyValue = logNode.Value.GetPropertyValue(StoreObjectSchema.ItemClass);
			if (propertyValue == "IPM.Schedule.Meeting.Request")
			{
				if (this.lastRequest != null)
				{
					string propertyValue2 = this.lastRequest.GetPropertyValue(CalendarItemBaseSchema.OrganizerEmailAddress);
					string propertyValue3 = logNode.Value.GetPropertyValue(CalendarItemBaseSchema.OrganizerEmailAddress);
					if (string.Compare(propertyValue2, propertyValue3, StringComparison.InvariantCultureIgnoreCase) != 0)
					{
						if (string.IsNullOrEmpty(propertyValue3))
						{
							base.Message = Strings.Error_SentRepresentingRemoved;
						}
						else
						{
							string propertyValue4 = this.lastRequest.GetPropertyValue(ItemSchema.SentRepresentingDisplayName);
							string propertyValue5 = logNode.Value.GetPropertyValue(ItemSchema.SentRepresentingDisplayName);
							base.Message = Strings.Error_SentRepresentingChanged(propertyValue4, propertyValue5);
						}
						logNode.Value.AddAlert(this);
					}
				}
				this.lastRequest = logNode.Value;
			}
		}

		private CalendarLogAnalysis lastRequest;
	}
}
