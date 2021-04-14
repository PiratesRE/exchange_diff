using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.StoreTasks
{
	internal class HtmlLogSerializer : LogSerializer
	{
		public HtmlLogSerializer(IEnumerable<PropertyDefinition> propertyMask)
		{
			this.propertyMask = propertyMask;
		}

		protected override void SerializeHeader(IEnumerable<CalendarLogAnalysis> logs, IEnumerable<PropertyDefinition> properties, ref StringBuilder sb)
		{
			sb.AppendFormat("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\"  \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <title>Logs for Item: {0}</title>\r\n</head>\r\n<body>\r\n    <table border=\"0\">", logs.First<CalendarLogAnalysis>().Identity);
		}

		protected override void SerializeBody(IEnumerable<CalendarLogAnalysis> logs, IEnumerable<PropertyDefinition> properties, ref StringBuilder sb)
		{
			sb.AppendLine("\r\n        <tr>");
			foreach (PropertyDefinition propertyDefinition in properties)
			{
				if (!this.propertyMask.Contains(propertyDefinition))
				{
					sb.AppendFormat("           <th>{0}</th>", propertyDefinition.Name);
				}
			}
			sb.AppendLine("        </tr>");
			foreach (CalendarLogAnalysis calendarLogAnalysis in logs)
			{
				if (calendarLogAnalysis.HasAlerts)
				{
					sb.AppendLine("        <tr>");
					sb.AppendFormat("<td>{0}</td>", calendarLogAnalysis.LocalLogTime);
					foreach (PropertyDefinition propertyDefinition2 in properties)
					{
						if (!this.propertyMask.Contains(propertyDefinition2))
						{
							sb.AppendFormat("<td>{0}</td>", calendarLogAnalysis[propertyDefinition2]);
						}
					}
					sb.AppendLine("        </tr>");
				}
			}
		}

		protected override void SerializeFooter(IEnumerable<CalendarLogAnalysis> logs, IEnumerable<PropertyDefinition> properties, ref StringBuilder sb)
		{
			sb.Append("    </table>\r\n</body>\r\n</html>");
		}
	}
}
