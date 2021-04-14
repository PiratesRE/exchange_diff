using System;
using System.Web;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal sealed class ElapsedTimeWatcher
	{
		public static void Watch(RequestStatistics.RequestStatItem name, Action action)
		{
			DateTime utcNow = DateTime.UtcNow;
			action();
			if (HttpContext.Current != null && HttpContext.Current.Items != null)
			{
				RequestStatistics requestStatistics = HttpContext.Current.Items[RequestStatistics.RequestStatsKey] as RequestStatistics;
				if (requestStatistics != null)
				{
					requestStatistics.AddStatisticsDataPoint(name, utcNow, DateTime.UtcNow);
				}
			}
		}

		public static void WatchStartTime(string eventName)
		{
			DateTime utcNow = DateTime.UtcNow;
			if (HttpContext.Current != null && HttpContext.Current.Items != null)
			{
				RequestStatistics requestStatistics = HttpContext.Current.Items[RequestStatistics.RequestStatsKey] as RequestStatistics;
				if (requestStatistics != null)
				{
					requestStatistics.AddExtendedStatisticsDataPoint(eventName, utcNow.ToString("hh:mm:ss.ffftt"));
				}
			}
		}

		public static void WatchMessage(string messageName, string messageData)
		{
			if (HttpContext.Current != null && HttpContext.Current.Items != null)
			{
				RequestStatistics requestStatistics = HttpContext.Current.Items[RequestStatistics.RequestStatsKey] as RequestStatistics;
				if (requestStatistics != null)
				{
					requestStatistics.AddExtendedStatisticsDataPoint(messageName, messageData);
				}
			}
		}
	}
}
