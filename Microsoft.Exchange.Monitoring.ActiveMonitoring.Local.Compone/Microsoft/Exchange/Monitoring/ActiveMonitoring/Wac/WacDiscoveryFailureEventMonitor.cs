using System;
using System.Reflection;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Wac
{
	public class WacDiscoveryFailureEventMonitor : OverallXFailuresMonitor
	{
		public new static MonitorDefinition CreateDefinition(string name, string sampleMask, string serviceName, Component component, int monitoringInterval, int recurrenceInterval, int numberOfFailures, bool enabled = true)
		{
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition(name, sampleMask, serviceName, component, monitoringInterval, recurrenceInterval, numberOfFailures, enabled);
			monitorDefinition.AssemblyPath = WacDiscoveryFailureEventMonitor.AssemblyPath;
			monitorDefinition.TypeName = WacDiscoveryFailureEventMonitor.TypeName;
			return monitorDefinition;
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(WacDiscoveryFailureEventMonitor).FullName;
	}
}
