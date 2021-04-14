using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Owa
{
	public sealed class OwaEdsDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			try
			{
				if (instance.ExchangeServerRoleEndpoint == null)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaEdsDiscovery:: DoWork(): Could not find ExchangeServerRoleEndpoint", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\Eds\\OwaEdsDiscovery.cs", 61);
					return;
				}
			}
			catch (EndpointManagerEndpointUninitializedException ex)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, string.Format("OwaEdsDiscovery:: DoWork(): ExchangeServerRoleEndpoint object threw exception.  Exception:{0}", ex.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\Eds\\OwaEdsDiscovery.cs", 67);
				return;
			}
			if (instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				OwaEdsAlertDefinitions item = new OwaEdsAlertDefinitions("OwaTooManyHttpErrorResponsesEncountered", Strings.OwaTooManyHttpErrorResponsesEncounteredSubject, Strings.OwaTooManyHttpErrorResponsesEncounteredBody, NotificationServiceClass.Urgent, false);
				OwaEdsAlertDefinitions item2 = new OwaEdsAlertDefinitions("OwaStartPageFailures", Strings.OwaTooManyStartPageFailuresSubject, Strings.OwaTooManyStartPageFailuresBody, NotificationServiceClass.Scheduled, false);
				OwaEdsAlertDefinitions item3 = new OwaEdsAlertDefinitions("OwaLogoffFailures", Strings.OwaTooManyLogoffFailuresSubject, Strings.OwaTooManyLogoffFailuresBody, NotificationServiceClass.Scheduled, false);
				OwaEdsAlertDefinitions item4 = new OwaEdsAlertDefinitions("OwaOutsideInDatabaseAvailability", Strings.OwaOutsideInDatabaseAvailabilityFailuresSubject, Strings.OwaOutsideInDatabaseAvailabilityFailuresBody, NotificationServiceClass.UrgentInTraining, true);
				foreach (OwaEdsAlertDefinitions owaEdsAlertDefinitions in new List<OwaEdsAlertDefinitions>
				{
					item,
					item2,
					item3,
					item4
				})
				{
					MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(owaEdsAlertDefinitions.MonitorName, string.Format("{0}/{1}", ExchangeComponent.Eds.Name, owaEdsAlertDefinitions.RedEvent.ToString()), ExchangeComponent.Eds.Name, ExchangeComponent.Eds, 1, true, 300);
					monitorDefinition.RecurrenceIntervalSeconds = 0;
					if (owaEdsAlertDefinitions.RecycleAppPool)
					{
						monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
						{
							new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
							new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, TimeSpan.FromMinutes(15.0))
						};
					}
					else
					{
						monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
						{
							new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 0)
						};
					}
					monitorDefinition.ServicePriority = 0;
					monitorDefinition.ScenarioDescription = "Validate OWA health is not impacted by EDS monitoring reported isssues";
					base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
					if (owaEdsAlertDefinitions.RecycleAppPool)
					{
						ResponderDefinition responderDefinition = ResetIISAppPoolResponder.CreateDefinition(owaEdsAlertDefinitions.RecycleResponderName, owaEdsAlertDefinitions.MonitorName, "MSExchangeOWAAppPool", ServiceHealthStatus.Unhealthy, DumpMode.FullDump, null, 25.0, 90, "Exchange", true, "Dag");
						responderDefinition.ServiceName = ExchangeComponent.OwaProtocol.Name;
						responderDefinition.RecurrenceIntervalSeconds = 0;
						base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
					}
					ResponderDefinition definition = EscalateResponder.CreateDefinition(owaEdsAlertDefinitions.EscalateResponderName, ExchangeComponent.Owa.Name, owaEdsAlertDefinitions.MonitorName, owaEdsAlertDefinitions.MonitorName, Environment.MachineName, ServiceHealthStatus.Unrecoverable, ExchangeComponent.Owa.EscalationTeam, owaEdsAlertDefinitions.MessageSubject, owaEdsAlertDefinitions.MessageBody, true, owaEdsAlertDefinitions.NotificationClass, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
					base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
				}
				return;
			}
		}

		private const string OwaTooManyHttpErrorResponsesEncounteredString = "OwaTooManyHttpErrorResponsesEncountered";

		private const string OwaStartPageFailuresString = "OwaStartPageFailures";

		private const string OwaLogoffFailuresString = "OwaLogoffFailures";

		private const string OwaOutsideInDatabaseAvailabilityFailuresString = "OwaOutsideInDatabaseAvailability";
	}
}
