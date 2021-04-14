using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Wac
{
	public sealed class WacDiscoveryFailureEventDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (!LocalEndpointManager.IsDataCenter)
			{
				return;
			}
			try
			{
				if (instance.ExchangeServerRoleEndpoint == null)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "WacDiscoveryFailureEventDiscovery:: DoWork(): Could not find ExchangeServerRoleEndpoint", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Wac\\WacDiscoveryFailureEventDiscovery.cs", 64);
					return;
				}
			}
			catch (EndpointManagerEndpointUninitializedException ex)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, string.Format("WacDiscoveryFailureEventDiscovery:: DoWork(): ExchangeServerRoleEndpoint object threw exception.  Exception:{0}", ex.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Wac\\WacDiscoveryFailureEventDiscovery.cs", 70);
				return;
			}
			if (instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				MonitorDefinition monitorDefinition = WacDiscoveryFailureEventMonitor.CreateDefinition("WacDiscoveryFailureEventMonitor", NotificationItem.GenerateResultName(ExchangeComponent.OwaDependency.Name, "DocCollab", null), ExchangeComponent.OwaDependency.Name, ExchangeComponent.OwaDependency, WacDiscoveryFailureEventDiscovery.MonitorIntervalSeconds, WacDiscoveryFailureEventDiscovery.MonitorIntervalSeconds, WacDiscoveryFailureEventDiscovery.EventCount, true);
				MonitorStateTransition[] monitorStateTransitions = new MonitorStateTransition[]
				{
					new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 0)
				};
				monitorDefinition.MonitorStateTransitions = monitorStateTransitions;
				monitorDefinition.ServicePriority = 1;
				monitorDefinition.ScenarioDescription = "Validate Wac Discovery is successful and we return valid set of wac viewable file types to the user";
				base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
				ResponderDefinition definition = EscalateResponder.CreateDefinition("WacDiscoveryFailureEventResponder", ExchangeComponent.OwaDependency.Name, "WacDiscoveryFailureEventMonitor", string.Format("{0}/{1}", "WacDiscoveryFailureEventMonitor", ExchangeComponent.OwaDependency.Name), Environment.MachineName, ServiceHealthStatus.Unrecoverable, ExchangeComponent.Owa.EscalationTeam, Strings.WacDiscoveryFailureSubject, Strings.WacDiscoveryFailureBody(Environment.MachineName), true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
				base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
				return;
			}
		}

		private const string WacDiscoveryFailureEventMonitorName = "WacDiscoveryFailureEventMonitor";

		private const string WacDiscoveryFailureEventResponderName = "WacDiscoveryFailureEventResponder";

		private static readonly int MonitorIntervalSeconds = 3600;

		private static readonly int EventCount = 1;
	}
}
