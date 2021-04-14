using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class HealthMonitoringEventsStrings
	{
		public static Dictionary<HealthMonitoringEvents, string> StringMap
		{
			get
			{
				if (HealthMonitoringEventsStrings.stringMap == null)
				{
					lock (HealthMonitoringEventsStrings.initLock)
					{
						if (HealthMonitoringEventsStrings.stringMap == null)
						{
							Type typeFromHandle = typeof(HealthMonitoringEvents);
							Array values = Enum.GetValues(typeFromHandle);
							HealthMonitoringEventsStrings.stringMap = new Dictionary<HealthMonitoringEvents, string>(values.Length);
							for (int i = 0; i < values.Length; i++)
							{
								HealthMonitoringEvents healthMonitoringEvents = (HealthMonitoringEvents)values.GetValue(i);
								string name = Enum.GetName(typeFromHandle, healthMonitoringEvents);
								HealthMonitoringEventsStrings.stringMap.Add(healthMonitoringEvents, name);
							}
						}
					}
				}
				return HealthMonitoringEventsStrings.stringMap;
			}
		}

		private static object initLock = new object();

		private static Dictionary<HealthMonitoringEvents, string> stringMap;
	}
}
