using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Transport
{
	internal class TestMailflow : IWorkItem
	{
		public void Initialize(MaintenanceDefinition discoveryDefinition, IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			this.InitializeProbe(discoveryDefinition, broker, traceContext);
			this.InitializeMonitor(discoveryDefinition, broker, traceContext);
			this.InitializeResponder(discoveryDefinition, broker, traceContext);
		}

		private void InitializeProbe(MaintenanceDefinition discoveryDefinition, IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			ProbeDefinition definition = WorkDefinitionHelper.CreateProbeDefinition("XPremiseMailflowProbe", typeof(CrossPremiseMailFlowProbe), null, TestMailflow.Component.Name, TimeSpan.FromSeconds(600.0), false);
			broker.AddWorkDefinition<ProbeDefinition>(definition, traceContext);
		}

		private void InitializeMonitor(MaintenanceDefinition discoveryDefinition, IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			MonitorDefinition definition = OverallXFailuresMonitor.CreateDefinition("XPremiseMailflowMonitor", "XPremiseMailflowProbe", TestMailflow.Component.Name, TestMailflow.Component, 1230, 0, 1, true);
			broker.AddWorkDefinition<MonitorDefinition>(definition, traceContext);
		}

		private void InitializeResponder(MaintenanceDefinition discoveryDefinition, IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			ResponderDefinition definition = EscalateResponder.CreateDefinition("XPremiseMailflowEscalateResponder", TestMailflow.Component.Name, "XPremiseMailflowMonitor", "XPremiseMailflowMonitor", null, ServiceHealthStatus.None, TestMailflow.Component.EscalationTeam, Strings.EscalationSubjectUnhealthy, Strings.CrossPremiseMailflowEscalationMessage, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			broker.AddWorkDefinition<ResponderDefinition>(definition, traceContext);
		}

		private const int ProbeIntervalSeconds = 600;

		private const int AlertingThreshold = 1;

		private const string ProbeName = "XPremiseMailflowProbe";

		private const string MonitorName = "XPremiseMailflowMonitor";

		private const string ResponderName = "XPremiseMailflowEscalateResponder";

		private static Component Component = ExchangeComponent.Transport;
	}
}
