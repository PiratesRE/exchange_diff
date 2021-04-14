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
	internal sealed class InstantMessagingDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				LocalEndpointManager instance = LocalEndpointManager.Instance;
				if (instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
				{
					MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition("OwaIMInitializationFailedMonitor", NotificationItem.GenerateResultName(ExchangeComponent.OwaDependency.Name, "InstantMessage", null), ExchangeComponent.OwaDependency.Name, ExchangeComponent.OwaDependency, (int)TimeSpan.FromMinutes(30.0).TotalSeconds, (int)TimeSpan.FromMinutes(30.0).TotalSeconds, 1, true);
					monitorDefinition.RecurrenceIntervalSeconds = 0;
					MonitorStateTransition[] monitorStateTransitions = new MonitorStateTransition[]
					{
						new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 0)
					};
					monitorDefinition.MonitorStateTransitions = monitorStateTransitions;
					monitorDefinition.ServicePriority = 1;
					monitorDefinition.ScenarioDescription = "Validate OWA health is not impacted by instant messaging issues";
					base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
					ResponderDefinition definition = EscalateResponder.CreateDefinition("OwaIMInitializationFailedEscalate", ExchangeComponent.OwaDependency.Name, "OwaIMInitializationFailedMonitor", string.Format("{0}/{1}", "OwaIMInitializationFailedMonitor", ExchangeComponent.OwaDependency.Name), Environment.MachineName, ServiceHealthStatus.Unrecoverable, ExchangeComponent.OwaDependency.EscalationTeam, Strings.OwaIMInitializationFailedSubject, Strings.OwaIMInitializationFailedMessage, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
					base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, string.Format("InstantMessagingDiscovery:: DoWork() threw an exception.  Exception:{0}", ex.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\InstantMessaging\\InstantMessagingDiscovery.cs", 87);
			}
		}

		internal const string IMInitializationFailedMonitorName = "OwaIMInitializationFailedMonitor";

		internal const string IMInitializationFailedResponderName = "OwaIMInitializationFailedEscalate";
	}
}
