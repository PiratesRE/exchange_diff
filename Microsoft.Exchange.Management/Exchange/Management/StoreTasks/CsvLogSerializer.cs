using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.StoreTasks
{
	internal class CsvLogSerializer : LogSerializer
	{
		public CsvLogSerializer(IEnumerable<PropertyDefinition> propertyMask)
		{
			this.propertyMask = propertyMask;
		}

		protected override void SerializeHeader(IEnumerable<CalendarLogAnalysis> logs, IEnumerable<PropertyDefinition> properties, ref StringBuilder sb)
		{
			sb.AppendFormat("Local Log Time{0}", ',');
			foreach (PropertyDefinition propertyDefinition in properties)
			{
				if (!this.propertyMask.Contains(propertyDefinition))
				{
					sb.AppendFormat("{0}{1}", this.CsvEscapeString(propertyDefinition.Name), ',');
				}
			}
			sb.Length--;
			sb.AppendLine();
		}

		protected override void SerializeBody(IEnumerable<CalendarLogAnalysis> logs, IEnumerable<PropertyDefinition> properties, ref StringBuilder sb)
		{
			foreach (CalendarLogAnalysis calendarLogAnalysis in logs)
			{
				if (calendarLogAnalysis.HasAlerts)
				{
					sb.AppendFormat("{0}{1}", this.CsvEscapeString(calendarLogAnalysis.LocalLogTime), ',');
					foreach (PropertyDefinition propertyDefinition in properties)
					{
						if (!this.propertyMask.Contains(propertyDefinition))
						{
							sb.AppendFormat("{0}{1}", this.CsvEscapeString(calendarLogAnalysis[propertyDefinition]), ',');
						}
					}
					if (calendarLogAnalysis.Alerts.Count<AnalysisRule>() > 0)
					{
						sb.AppendFormat("{0} ALERTS:", calendarLogAnalysis.Alerts.Count<AnalysisRule>());
						foreach (AnalysisRule analysisRule in calendarLogAnalysis.Alerts)
						{
							sb.AppendFormat("[{0}]", analysisRule.ToString());
						}
					}
					sb.AppendLine();
				}
			}
		}

		private string CsvEscapeString(string str)
		{
			if (str.Contains(','))
			{
				return string.Format("\"{0}\"", str);
			}
			return str;
		}

		public const char Delimiter = ',';
	}
}
