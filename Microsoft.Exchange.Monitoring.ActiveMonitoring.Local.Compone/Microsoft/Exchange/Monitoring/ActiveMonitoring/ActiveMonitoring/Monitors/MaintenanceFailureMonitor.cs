using System;
using System.Reflection;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors
{
	public sealed class MaintenanceFailureMonitor : MaintenanceMonitor
	{
		internal static MonitorDefinition CreateDefinition(Component component)
		{
			return new MonitorDefinition
			{
				AssemblyPath = MaintenanceFailureMonitor.AssemblyPath,
				TypeName = MaintenanceFailureMonitor.TypeName,
				Name = string.Format("{0}.{1}", "MaintenanceFailureMonitor", component.Name),
				SampleMask = NotificationItem.GenerateResultName(component.Name, MonitoringNotificationEvent.MaintenanceFailure.ToString(), null),
				ServiceName = component.Name,
				Component = component,
				Enabled = true,
				TimeoutSeconds = 30,
				MonitoringIntervalSeconds = 1800,
				RecurrenceIntervalSeconds = 300,
				SecondaryMonitoringThreshold = 15.0
			};
		}

		internal const string MaintenanceFailureMonitorName = "MaintenanceFailureMonitor";

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(MaintenanceFailureMonitor).FullName;
	}
}
