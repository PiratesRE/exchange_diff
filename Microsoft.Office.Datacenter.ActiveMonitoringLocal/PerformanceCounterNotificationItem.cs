using System;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal class PerformanceCounterNotificationItem : NotificationItem
	{
		internal PerformanceCounterNotificationItem(string counterName, double counterValue, DateTime timeStamp) : base(PerformanceCounterNotificationItem.edsNotificationServiceName, PerformanceCounterNotificationItem.PerformanceCounterComponentName, counterName, counterName, ResultSeverityLevel.Informational)
		{
			base.SampleValue = counterValue;
			base.CustomProperties.Add("CollectionTime", timeStamp.ToString());
		}

		internal static string PerformanceCounterComponentName
		{
			get
			{
				return "Performance Counter";
			}
		}

		internal static string PerformanceCounterAnalyzerName
		{
			get
			{
				return "PerfLogActiveMonitoringAnalyzer";
			}
		}

		public static string GenerateResultName(string perfCounterName)
		{
			return NotificationItem.GenerateResultName(PerformanceCounterNotificationItem.edsNotificationServiceName, PerformanceCounterNotificationItem.PerformanceCounterComponentName, perfCounterName);
		}

		private static readonly string edsNotificationServiceName = ExchangeComponent.Eds.Name;
	}
}
