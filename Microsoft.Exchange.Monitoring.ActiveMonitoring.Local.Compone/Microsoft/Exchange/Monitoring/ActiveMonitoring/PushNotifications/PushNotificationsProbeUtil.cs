using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PushNotifications
{
	public static class PushNotificationsProbeUtil
	{
		internal static List<EventRecord> GetOperationalChannelEvents(DateTime startTime, DateTime endTime, int numberOfErrors, string appId = null)
		{
			string additionalConstraints = null;
			if (!string.IsNullOrEmpty(appId))
			{
				additionalConstraints = string.Format(" and UserData[EventXML[(AppId='{0}')]]", appId);
			}
			string xPathQuery = CrimsonChannelReader.GenerateXPathFilterForFailedEventsInTimeRange("Microsoft-Exchange-PushNotifications", CrimsonChannelType.Operational.ToString(), startTime, endTime, additionalConstraints);
			List<EventRecord> result;
			using (CrimsonChannelReader crimsonChannelReader = new CrimsonChannelReader("Microsoft-Exchange-PushNotifications", CrimsonChannelType.Operational.ToString(), xPathQuery))
			{
				List<EventRecord> list = crimsonChannelReader.ReadFirstNEvents(numberOfErrors).ToList<EventRecord>();
				result = list;
			}
			return result;
		}

		private const string PushNotificationChannelName = "Microsoft-Exchange-PushNotifications";
	}
}
