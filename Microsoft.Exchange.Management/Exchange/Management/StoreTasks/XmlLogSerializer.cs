using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.StoreTasks
{
	internal class XmlLogSerializer : LogSerializer
	{
		public XmlLogSerializer(IEnumerable<PropertyDefinition> propertyMask)
		{
			this.propertyMask = propertyMask;
		}

		protected override void SerializeHeader(IEnumerable<CalendarLogAnalysis> logs, IEnumerable<PropertyDefinition> properties, ref StringBuilder sb)
		{
			sb.AppendLine("<?xml version=\"1.0\"?>\r\n<CalendarLogAnalysisItems>");
		}

		protected override void SerializeBody(IEnumerable<CalendarLogAnalysis> logs, IEnumerable<PropertyDefinition> properties, ref StringBuilder sb)
		{
			foreach (CalendarLogAnalysis calendarLogAnalysis in logs)
			{
				if (calendarLogAnalysis.HasAlerts)
				{
					sb.AppendFormat("    <CalendarLogAnalysis logtime=\"{0}\">", calendarLogAnalysis.LocalLogTime);
					sb.AppendLine("        <Properties>");
					foreach (PropertyDefinition propertyDefinition in properties)
					{
						if (!this.propertyMask.Contains(propertyDefinition) && calendarLogAnalysis.InternalProperties.ContainsKey(propertyDefinition))
						{
							sb.AppendFormat("          <{0} Type=\"{1}\">{2}</{0}>{3}", new object[]
							{
								propertyDefinition.Name,
								propertyDefinition.Type,
								calendarLogAnalysis[propertyDefinition],
								Environment.NewLine
							});
						}
					}
					sb.AppendLine("        </Properties>");
					if (calendarLogAnalysis.Alerts.Count<AnalysisRule>() == 0)
					{
						sb.AppendLine("        <Alerts />");
					}
					else
					{
						sb.AppendLine("        <Alerts>");
						foreach (AnalysisRule analysisRule in calendarLogAnalysis.Alerts)
						{
							sb.AppendFormat("          <{0} Severity=\"{1}\">{2}</{0}>{3}", new object[]
							{
								analysisRule.Name,
								analysisRule.AlertLevel,
								analysisRule.Message,
								Environment.NewLine
							});
						}
						sb.AppendLine("        </Alerts>");
					}
					sb.AppendLine("    </CalendarLogAnalysis>");
				}
			}
		}

		protected override void SerializeFooter(IEnumerable<CalendarLogAnalysis> logs, IEnumerable<PropertyDefinition> properties, ref StringBuilder sb)
		{
			sb.AppendLine("</CalendarLogAnalysisItems>");
		}
	}
}
