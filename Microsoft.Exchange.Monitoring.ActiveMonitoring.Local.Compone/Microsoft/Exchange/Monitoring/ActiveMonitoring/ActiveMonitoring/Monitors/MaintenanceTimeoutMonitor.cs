using System;
using System.Reflection;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors
{
	public sealed class MaintenanceTimeoutMonitor : MaintenanceMonitor
	{
		internal static MonitorDefinition CreateDefinition(Component component)
		{
			return new MonitorDefinition
			{
				AssemblyPath = MaintenanceTimeoutMonitor.AssemblyPath,
				TypeName = MaintenanceTimeoutMonitor.TypeName,
				Name = string.Format("{0}.{1}", "MaintenanceTimeoutMonitor", component.Name),
				SampleMask = NotificationItem.GenerateResultName(component.Name, MonitoringNotificationEvent.MaintenanceTimeout.ToString(), null),
				ServiceName = component.Name,
				Component = component,
				Enabled = true,
				TimeoutSeconds = 30,
				MonitoringIntervalSeconds = 1800,
				RecurrenceIntervalSeconds = 300,
				SecondaryMonitoringThreshold = 15.0
			};
		}

		internal const string MaintenanceTimeoutMonitorName = "MaintenanceTimeoutMonitor";

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(MaintenanceTimeoutMonitor).FullName;
	}
}
