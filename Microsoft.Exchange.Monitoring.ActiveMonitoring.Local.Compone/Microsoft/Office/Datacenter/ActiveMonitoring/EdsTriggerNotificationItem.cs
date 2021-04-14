using System;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class EdsTriggerNotificationItem : NotificationItem
	{
		internal EdsTriggerNotificationItem(string triggerName, ResultSeverityLevel severity, DateTime timeStamp) : base(EdsTriggerNotificationItem.edsNotificationServiceName, EdsTriggerNotificationItem.TriggerComponentName, triggerName, triggerName, severity)
		{
			base.TimeStamp = timeStamp;
		}

		internal EdsTriggerNotificationItem(string triggerName, double triggerValue, ResultSeverityLevel severity, DateTime timeStamp) : this(triggerName, severity, timeStamp)
		{
			base.SampleValue = triggerValue;
		}

		internal static string TriggerComponentName
		{
			get
			{
				return "Trigger";
			}
		}

		public static string GenerateResultName(string triggerName)
		{
			return NotificationItem.GenerateResultName(EdsTriggerNotificationItem.edsNotificationServiceName, EdsTriggerNotificationItem.TriggerComponentName, triggerName);
		}

		private static readonly string edsNotificationServiceName = ExchangeComponent.Eds.Name;
	}
}
