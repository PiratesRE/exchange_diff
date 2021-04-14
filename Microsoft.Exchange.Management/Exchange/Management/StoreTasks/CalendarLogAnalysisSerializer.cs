using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CalendarLogAnalysisSerializer
	{
		internal static string Serialize(IEnumerable<CalendarLogAnalysis> logs, OutputType outputType, AnalysisDetailLevel detailLevel, bool showAll)
		{
			IEnumerable<PropertyDefinition> propertyMask = showAll ? new List<PropertyDefinition>() : CalendarLogAnalysisSerializer.FindUnchangedProperties(logs);
			LogSerializer logSerializer;
			switch (outputType)
			{
			case OutputType.HTML:
				logSerializer = new HtmlLogSerializer(propertyMask);
				goto IL_42;
			case OutputType.XML:
				logSerializer = new XmlLogSerializer(propertyMask);
				goto IL_42;
			}
			logSerializer = new CsvLogSerializer(propertyMask);
			IL_42:
			IEnumerable<PropertyDefinition> properties = AnalysisDetailLevels.GetDisplayProperties(detailLevel).Union(CalendarLogAnalysis.GetDisplayProperties(logs));
			logs.OrderBy((CalendarLogAnalysis f) => f, CalendarLogAnalysis.GetComparer());
			return logSerializer.Serialize(logs, properties, null);
		}

		private static IEnumerable<PropertyDefinition> FindUnchangedProperties(IEnumerable<CalendarLogAnalysis> logs)
		{
			List<PropertyDefinition> list = new List<PropertyDefinition>();
			Dictionary<PropertyDefinition, object>.KeyCollection keys = logs.First<CalendarLogAnalysis>().InternalProperties.Keys;
			foreach (PropertyDefinition propertyDefinition in keys)
			{
				if (!CalendarLogAnalysisSerializer.MinPropSet.Contains(propertyDefinition))
				{
					string b = logs.First<CalendarLogAnalysis>()[propertyDefinition];
					bool flag = false;
					foreach (CalendarLogAnalysis calendarLogAnalysis in logs)
					{
						if (calendarLogAnalysis[propertyDefinition] != b)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						list.Add(propertyDefinition);
					}
				}
			}
			return list;
		}

		private static IEnumerable<PropertyDefinition> MinPropSet = new List<PropertyDefinition>
		{
			CalendarItemBaseSchema.OriginalLastModifiedTime,
			CalendarItemBaseSchema.CalendarLogTriggerAction,
			CalendarItemBaseSchema.ClientInfoString
		};
	}
}
