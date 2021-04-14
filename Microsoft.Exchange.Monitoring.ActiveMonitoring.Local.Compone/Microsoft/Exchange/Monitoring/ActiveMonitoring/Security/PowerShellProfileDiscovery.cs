using System;
using System.Configuration;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Security
{
	public class PowerShellProfileDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceFunction(ExTraceGlobals.SecurityTracer, base.TraceContext, "PowerShellProfileDiscovery.DoWork : Started", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Security\\PowerShellProfileDiscovery.cs", 45);
			this.CreatePowerShellProfileContext();
			WTFDiagnostics.TraceFunction(ExTraceGlobals.SecurityTracer, base.TraceContext, "PowerShellProfileDiscovery.DoWork : Ended", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Security\\PowerShellProfileDiscovery.cs", 47);
		}

		private void CreatePowerShellProfileContext()
		{
			bool enabled = bool.Parse(base.Definition.Attributes["PowerShellProfileEnabled"]);
			int recurrenceIntervalSeconds = int.Parse(base.Definition.Attributes["PowerShellProfileProbeRecurrenceIntervalSeconds"]);
			int maxRetryApptempts = int.Parse(base.Definition.Attributes["PowerShellProfileRetryAttempts"]);
			ProbeDefinition definition = PowerShellProfileProbe.CreateProbeDefinition("PowerShellProfileProbe", typeof(PowerShellProfileProbe), "ProfileCheck", recurrenceIntervalSeconds, maxRetryApptempts, enabled);
			base.Broker.AddWorkDefinition<ProbeDefinition>(definition, base.TraceContext);
			int recurrenceIntervalSeconds2 = int.Parse(base.Definition.Attributes["PowerShellProfileMonitorRecurrenceIntervalSeconds"]);
			int monitoringIntervalSeconds = int.Parse(base.Definition.Attributes["PowerShellProfileMonitorMonitoringIntervalSeconds"]);
			int monitoringThreshold = int.Parse(base.Definition.Attributes["PowerShellProfileMonitorMonitoringThreshold"]);
			MonitorDefinition monitorDefinition = this.CreateMonitorDefinition("PowerShellProfileMonitor", PowerShellProfileDiscovery.OverallXFailuresMonitorType, "PowerShellProfileProbe/ProfileCheck", ExchangeComponent.Security.Name, "ProfileCheck", recurrenceIntervalSeconds2, monitoringIntervalSeconds, monitoringThreshold, enabled);
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Check any PowerShell proflies are created at defualt locations";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition responderDefinition = PowerShellProfileResponder.CreateEscalateResponderDefinition("PowerShellProfileMonitor", ExchangeComponent.Security.Name, "PowerShellProfileProbe/ProfileCheck", "ProfileCheck", "Security", enabled, NotificationServiceClass.UrgentInTraining);
			responderDefinition.TargetHealthState = ServiceHealthStatus.Unhealthy;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private MonitorDefinition CreateMonitorDefinition(string monitorName, Type monitorType, string sampleMask, string serviceName, string targetResource, int recurrenceIntervalSeconds, int monitoringIntervalSeconds, int monitoringThreshold, bool enabled)
		{
			MonitorDefinition monitorDefinition = new MonitorDefinition();
			monitorDefinition.Name = monitorName;
			monitorDefinition.AssemblyPath = monitorType.Assembly.Location;
			monitorDefinition.TypeName = monitorType.FullName;
			monitorDefinition.SampleMask = sampleMask;
			monitorDefinition.ServiceName = serviceName;
			monitorDefinition.TargetResource = targetResource;
			monitorDefinition.Component = ExchangeComponent.Security;
			monitorDefinition.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			monitorDefinition.InsufficientSamplesIntervalSeconds = Math.Max(5 * monitorDefinition.RecurrenceIntervalSeconds, Convert.ToInt32(ConfigurationManager.AppSettings["InsufficientSamplesIntervalInSeconds"]));
			monitorDefinition.TimeoutSeconds = recurrenceIntervalSeconds;
			monitorDefinition.MonitoringIntervalSeconds = monitoringIntervalSeconds;
			monitorDefinition.MonitoringThreshold = (double)monitoringThreshold;
			monitorDefinition.Enabled = enabled;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.SecurityTracer, base.TraceContext, string.Format("PowerShellProfileDiscovery.CreateMonitorDefinition: Created MonitorDefinition '{0}' for '{1}'", monitorName, monitorType.FullName), null, "CreateMonitorDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Security\\PowerShellProfileDiscovery.cs", 148);
			return monitorDefinition;
		}

		internal const string PowerShellProfileProbeName = "PowerShellProfileProbe";

		private const string PowerShellProfileMonitorName = "PowerShellProfileMonitor";

		private const string PowerShellProfileResponderName = "PowerShellProfileResponder";

		private const string EscalationTeam = "Security";

		private const string Mask = "ProfileCheck";

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly Type OverallXFailuresMonitorType = typeof(OverallXFailuresMonitor);
	}
}
