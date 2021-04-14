using System;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Places
{
	public sealed class FindPlacesEdsDiscovery : MaintenanceWorkItem
	{
		public static string MonitorName(string eventName)
		{
			return eventName + "Monitor";
		}

		public static string ResponderName(string eventName)
		{
			return eventName + "Escalate";
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.Places.Enabled || !LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				return;
			}
			this.CreateFailureMonitorAndResponder("FindPlacesFailureAboveThreshold", ExchangeComponent.Places, 1, Strings.FindPlacesRequestsError(Environment.MachineName));
			this.CreateFailureMonitorAndResponder("BingServicesLatencyAboveThreshold", ExchangeComponent.Places, 1, Strings.FindPlacesRequestsError(Environment.MachineName));
		}

		private void CreateFailureMonitorAndResponder(string eventName, Component exchangeComponent, int consecutiveFailures, string escalationMessage)
		{
			string text = FindPlacesEdsDiscovery.MonitorName(eventName);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(text, NotificationItem.GenerateResultName(exchangeComponent.Name, eventName, null), exchangeComponent.Name, exchangeComponent, consecutiveFailures, true, 300);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0)
			};
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Validate Places health is not impacted by any issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition definition = EscalateResponder.CreateDefinition(FindPlacesEdsDiscovery.ResponderName(eventName), exchangeComponent.Name, text, text, string.Empty, ServiceHealthStatus.Unhealthy, exchangeComponent.EscalationTeam, Strings.EscalationSubjectUnhealthy, escalationMessage, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
		}

		internal const int DefaultMonitorThreshold = 1;

		internal const string MonitorNameSuffix = "Monitor";

		internal const string ResponderNameSuffix = "Escalate";

		internal const string PlacesFailuresEventName = "FindPlacesFailureAboveThreshold";

		internal const string BingHighLatencyEventName = "BingServicesLatencyAboveThreshold";
	}
}
