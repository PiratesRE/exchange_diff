using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Owa
{
	internal sealed class InstantMessagingLogAnalyzerDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				LocalEndpointManager instance = LocalEndpointManager.Instance;
				if (instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
				{
					MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("OwaIMLogAnalyzerMonitor", string.Format("{0}/{1}", ExchangeComponent.Eds.Name, "OwaLyncFailureAboveThreshold"), ExchangeComponent.Eds.Name, ExchangeComponent.Eds, 3, true, 300);
					monitorDefinition.RecurrenceIntervalSeconds = 0;
					MonitorStateTransition[] monitorStateTransitions = new MonitorStateTransition[]
					{
						new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 0)
					};
					monitorDefinition.MonitorStateTransitions = monitorStateTransitions;
					monitorDefinition.ServicePriority = 0;
					monitorDefinition.ScenarioDescription = "Validate OWA health is not impacted by instant messaging issues";
					base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
					ResponderDefinition definition = EscalateResponder.CreateDefinition("OwaIMLogAnalyzerEscalate", ExchangeComponent.OwaDependency.Name, "OwaIMLogAnalyzerMonitor", "OwaIMLogAnalyzerMonitor", Environment.MachineName, ServiceHealthStatus.Unrecoverable, ExchangeComponent.Owa.EscalationTeam, Strings.OwaIMLogAnalyzerSubject, Strings.OwaIMLogAnalyzerMessage, true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
					base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, string.Format("InstantMessagingLogAnalyzerDiscovery:: DoWork() threw an exception.  Exception:{0}", ex.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\InstantMessaging\\InstantMessagingLogAnalyzerDiscovery.cs", 91);
			}
		}

		internal const string IMLogAnalyzerMonitorName = "OwaIMLogAnalyzerMonitor";

		internal const string IMILogAnalyzerResponderName = "OwaIMLogAnalyzerEscalate";

		internal const string IMLogAnalyzerEventName = "OwaLyncFailureAboveThreshold";
	}
}
