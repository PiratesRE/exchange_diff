using System;
using Microsoft.Office.CompliancePolicy;
using Microsoft.Office.CompliancePolicy.Monitor;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Data.ApplicationLogic.UnifiedPolicy
{
	internal sealed class MonitoringItemErrorPublisher : IMonitoringNotification
	{
		public static MonitoringItemErrorPublisher Instance
		{
			get
			{
				return MonitoringItemErrorPublisher.instance;
			}
		}

		public void PublishEvent(string componentName, string organization, string context, Exception exception)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("componentName", componentName);
			ArgumentValidator.ThrowIfNullOrEmpty("organization", organization);
			string arg = (exception != null) ? exception.ToString() : "<>";
			EventNotificationItem.Publish(ExchangeComponent.UnifiedPolicy.Name, componentName, null, string.Format("Policy sync issues identified for Tenant {0}.\r\nContext: {1}.\r\nError: {2}.", organization, context, arg), ResultSeverityLevel.Error, false);
		}

		private static readonly MonitoringItemErrorPublisher instance = new MonitoringItemErrorPublisher();
	}
}
