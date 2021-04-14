using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.OfficeGraph.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Search;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Search.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.OfficeGraph
{
	public sealed class OfficeGraphDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				if (!LocalEndpointManager.IsDataCenter && !LocalEndpointManager.IsDataCenterDedicated)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.OfficeGraphTracer, base.TraceContext, "OfficeGraphDiscovery.DoWork: Not in datacenter, thus no need to create OfficeGraph related work items", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OfficeGraph\\OfficeGraphDiscovery.cs", 72);
				}
				else
				{
					this.attributeHelper = new AttributeHelper(base.Definition);
					LocalEndpointManager instance = LocalEndpointManager.Instance;
					if (instance.ExchangeServerRoleEndpoint == null)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.OfficeGraphTracer, base.TraceContext, "OfficeGraphDiscovery.DoWork: Could not find ExchangeServerRoleEndpoint, thus no need to create OfficeGraph related work items", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OfficeGraph\\OfficeGraphDiscovery.cs", 83);
					}
					else if (this.IsInMaintenance())
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.OfficeGraphTracer, base.TraceContext, "OfficeGraphDiscovery.DoWork: Server is in maintenance mode, thus skip monitoring", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OfficeGraph\\OfficeGraphDiscovery.cs", 90);
					}
					else if (instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.OfficeGraphTracer, base.TraceContext, "OfficeGraphDiscovery.DoWork: Creating monitoring contexts for mailbox", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OfficeGraph\\OfficeGraphDiscovery.cs", 97);
						this.CreateMonitoringContextsForMailbox();
						WTFDiagnostics.TraceInformation(ExTraceGlobals.OfficeGraphTracer, base.TraceContext, "OfficeGraphDiscovery.DoWork: Created monitoring contexts for mailbox", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OfficeGraph\\OfficeGraphDiscovery.cs", 99);
					}
					else if (instance.ExchangeServerRoleEndpoint.IsFrontendTransportRoleInstalled || instance.ExchangeServerRoleEndpoint.IsBridgeheadRoleInstalled)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.OfficeGraphTracer, base.TraceContext, "OfficeGraphDiscovery.DoWork: Creating monitoring contexts for transport", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OfficeGraph\\OfficeGraphDiscovery.cs", 106);
						this.CreateMonitoringContextsForTransport();
						WTFDiagnostics.TraceInformation(ExTraceGlobals.OfficeGraphTracer, base.TraceContext, "OfficeGraphDiscovery.DoWork: Created monitoring contexts for transport", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OfficeGraph\\OfficeGraphDiscovery.cs", 108);
					}
				}
			}
			catch (EndpointManagerEndpointUninitializedException)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OfficeGraphTracer, base.TraceContext, "OfficeGraphDiscovery.DoWork: EndpointManagerEndpointUninitializedException is caught. Endpoint is not available to do monitoring", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OfficeGraph\\OfficeGraphDiscovery.cs", 114);
			}
		}

		private bool IsInMaintenance()
		{
			return DirectoryAccessor.Instance.IsRecoveryActionsEnabledOffline(LocalServer.GetServer().Name);
		}

		private void CreateMonitoringContextsForMailbox()
		{
			this.CreateTransportDeliveryAgentContext(false);
			this.CreateMessageTracingPluginProcessingTimeContext(false);
			this.CreateMessageTracingPluginLogDirectorySizeContext(false);
		}

		private void CreateMonitoringContextsForTransport()
		{
		}

		private void CreateTransportDeliveryAgentContext(bool isEscalateModeUrgent)
		{
			bool @bool = this.attributeHelper.GetBool("OfficeGraphTransportDeliveryAgentEnabled", true, true);
			int @int = this.attributeHelper.GetInt("OfficeGraphTransportDeliveryAgentProbeRecurrenceIntervalSeconds", true, 0, null, null);
			int int2 = this.attributeHelper.GetInt("OfficeGraphTransportDeliveryAgentMonitorMonitoringThreshold", true, 0, null, null);
			int int3 = this.attributeHelper.GetInt("OfficeGraphTransportDeliveryAgentMonitorMonitoringIntervalSeconds", true, 0, null, null);
			int int4 = this.attributeHelper.GetInt("OfficeGraphTransportDeliveryAgentMonitorUnhealthyStateSeconds", true, 0, null, null);
			ProbeDefinition probeDefinition = this.CreateProbeDefinition("OfficeGraphTransportDeliveryAgentProcessingTimeProbe", typeof(OfficeGraphTransportDeliveryAgentProcessingTimeProbe), string.Empty, @int, @bool);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = this.CreateMonitorDefinition("OfficeGraphTransportDeliveryAgentProcessingTimeMonitor", typeof(OverallXFailuresMonitor), probeDefinition.ConstructWorkItemResultName(), string.Empty, @int, int3, int2, @bool);
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Monitor Office Graph transport delivery agent's processing time";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			List<MonitorStateTransition> list = new List<MonitorStateTransition>();
			list.Add(new MonitorStateTransition(ServiceHealthStatus.Unhealthy, int4));
			ResponderDefinition definition = SearchMonitoringHelper.CreateEscalateResponderDefinition(monitorDefinition, "OfficeGraphTransportDeliveryAgentProcessingTimeProbe escalation", @bool, ServiceHealthStatus.Unhealthy, SearchEscalateResponder.EscalateModes.Scheduled, true);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
			monitorDefinition.MonitorStateTransitions = list.ToArray();
		}

		private void CreateMessageTracingPluginProcessingTimeContext(bool isEscalateModeUrgent)
		{
			bool @bool = this.attributeHelper.GetBool("OfficeGraphMessageTracingPluginEnabled", true, true);
			int @int = this.attributeHelper.GetInt("OfficeGraphMessageTracingPluginProbeRecurrenceIntervalSeconds", true, 0, null, null);
			int int2 = this.attributeHelper.GetInt("OfficeGraphMessageTracingPluginMonitorMonitoringThreshold", true, 0, null, null);
			int int3 = this.attributeHelper.GetInt("OfficeGraphMessageTracingPluginMonitorMonitoringIntervalSeconds", true, 0, null, null);
			int int4 = this.attributeHelper.GetInt("OfficeGraphMessageTracingPluginMonitorUnhealthyStateSeconds", true, 0, null, null);
			ProbeDefinition probeDefinition = this.CreateProbeDefinition("OfficeGraphMessageTracingPluginProcessingTimeProbe", typeof(OfficeGraphMessageTracingPluginProcessingTimeProbe), string.Empty, @int, @bool);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = this.CreateMonitorDefinition("OfficeGraphMessageTracingPluginProcessingTimeMonitor", typeof(OverallXFailuresMonitor), probeDefinition.ConstructWorkItemResultName(), string.Empty, @int, int3, int2, @bool);
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Monitor Office Graph message tracing plugin's processing time";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			List<MonitorStateTransition> list = new List<MonitorStateTransition>();
			list.Add(new MonitorStateTransition(ServiceHealthStatus.Unhealthy, int4));
			ResponderDefinition definition = SearchMonitoringHelper.CreateEscalateResponderDefinition(monitorDefinition, "OfficeGraphMessageTracingPluginProcessingTimeProbe escalation", @bool, ServiceHealthStatus.Unhealthy, SearchEscalateResponder.EscalateModes.Scheduled, true);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
			monitorDefinition.MonitorStateTransitions = list.ToArray();
		}

		private void CreateMessageTracingPluginLogDirectorySizeContext(bool isEscalateModeUrgent)
		{
			bool @bool = this.attributeHelper.GetBool("OfficeGraphMessageTracingPluginEnabled", true, true);
			int @int = this.attributeHelper.GetInt("OfficeGraphMessageTracingPluginProbeRecurrenceIntervalSeconds", true, 0, null, null);
			int int2 = this.attributeHelper.GetInt("OfficeGraphMessageTracingPluginMonitorMonitoringThreshold", true, 0, null, null);
			int int3 = this.attributeHelper.GetInt("OfficeGraphMessageTracingPluginMonitorMonitoringIntervalSeconds", true, 0, null, null);
			int int4 = this.attributeHelper.GetInt("OfficeGraphMessageTracingPluginMonitorUnhealthyStateSeconds", true, 0, null, null);
			ProbeDefinition probeDefinition = this.CreateProbeDefinition("OfficeGraphMessageTracingPluginLogSizeProbe", typeof(OfficeGraphMessageTracingPluginLogSizeProbe), string.Empty, @int, @bool);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = this.CreateMonitorDefinition("OfficeGraphMessageTracingPluginLogSizeMonitor", typeof(OverallXFailuresMonitor), probeDefinition.ConstructWorkItemResultName(), string.Empty, @int, int3, int2, @bool);
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Monitor Office Graph message tracing plugin's log directory size";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			List<MonitorStateTransition> list = new List<MonitorStateTransition>();
			list.Add(new MonitorStateTransition(ServiceHealthStatus.Unhealthy, int4));
			ResponderDefinition definition = SearchMonitoringHelper.CreateEscalateResponderDefinition(monitorDefinition, "OfficeGraphMessageTracingPluginLogSizeProbe escalation", @bool, ServiceHealthStatus.Unhealthy, SearchEscalateResponder.EscalateModes.Scheduled, true);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
			monitorDefinition.MonitorStateTransitions = list.ToArray();
		}

		private ProbeDefinition CreateProbeDefinition(string probeName, Type probeType, string targetResource, int recurrenceIntervalSeconds, bool enabled)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = probeType.Assembly.Location;
			probeDefinition.TypeName = probeType.FullName;
			probeDefinition.Name = probeName;
			probeDefinition.ServiceName = ExchangeComponent.OfficeGraph.Name;
			probeDefinition.TargetResource = targetResource;
			probeDefinition.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			probeDefinition.TimeoutSeconds = 2 * recurrenceIntervalSeconds;
			probeDefinition.MaxRetryAttempts = 0;
			probeDefinition.Enabled = enabled;
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.OfficeGraphTracer, base.TraceContext, "OfficeGraphDiscovery.CreateProbeDefinition: Created ProbeDefinition '{0}' for '{1}'.", probeName, targetResource, null, "CreateProbeDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OfficeGraph\\OfficeGraphDiscovery.cs", 321);
			return probeDefinition;
		}

		private MonitorDefinition CreateMonitorDefinition(string monitorName, Type monitorType, string sampleMask, string targetResource, int recurrenceIntervalSeconds, int monitoringIntervalSeconds, int monitoringThreshold, bool enabled)
		{
			MonitorDefinition monitorDefinition = new MonitorDefinition();
			monitorDefinition.AssemblyPath = monitorType.Assembly.Location;
			monitorDefinition.TypeName = monitorType.FullName;
			monitorDefinition.Name = monitorName;
			monitorDefinition.ServiceName = ExchangeComponent.OfficeGraph.Name;
			monitorDefinition.SampleMask = sampleMask;
			monitorDefinition.TargetResource = targetResource;
			monitorDefinition.Component = ExchangeComponent.OfficeGraph;
			monitorDefinition.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			monitorDefinition.InsufficientSamplesIntervalSeconds = Math.Max(5 * monitorDefinition.RecurrenceIntervalSeconds, Convert.ToInt32(ConfigurationManager.AppSettings["InsufficientSamplesIntervalInSeconds"]));
			monitorDefinition.TimeoutSeconds = 2 * recurrenceIntervalSeconds;
			monitorDefinition.MonitoringIntervalSeconds = monitoringIntervalSeconds;
			monitorDefinition.MonitoringThreshold = (double)monitoringThreshold;
			monitorDefinition.Enabled = enabled;
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.OfficeGraphTracer, base.TraceContext, "OfficeGraphDiscovery.CreateMonitorDefinition: Created MonitorDefinition '{0}' for '{1}'.", monitorName, targetResource, null, "CreateMonitorDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OfficeGraph\\OfficeGraphDiscovery.cs", 367);
			return monitorDefinition;
		}

		internal const int MaxRetryAttempt = 0;

		private static readonly Type OverallXFailuresMonitorType = typeof(OverallXFailuresMonitor);

		private static readonly Type OverallConsecutiveProbeFailuresMonitorType = typeof(OverallConsecutiveProbeFailuresMonitor);

		private static readonly Type RestartServiceResponderType = typeof(RestartServiceResponder);

		private AttributeHelper attributeHelper;
	}
}
