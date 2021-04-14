using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Owa.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Owa
{
	public sealed class OwaErrorDetectionDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			try
			{
				if (instance.ExchangeServerRoleEndpoint == null)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaErrorDetectionDiscovery:: DoWork(): Could not find ExchangeServerRoleEndpoint", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\Eds\\OwaErrorDetectionDiscovery.cs", 48);
					return;
				}
			}
			catch (EndpointManagerEndpointUninitializedException ex)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, string.Format("OwaErrorDetectionDiscovery:: DoWork(): ExchangeServerRoleEndpoint object threw exception.  Exception:{0}", ex.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\Eds\\OwaErrorDetectionDiscovery.cs", 54);
				return;
			}
			if (instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				this.CreateAlertsRoutingConfig();
				int recurrenceIntervalSeconds = (int)this.ReadAttribute("ResponderRecurrenceInterval", TimeSpan.FromMinutes(5.0)).TotalSeconds;
				int recurrenceIntervalSeconds2 = (int)this.ReadAttribute("MonitorRecurrenceInterval", TimeSpan.FromMinutes(0.0)).TotalSeconds;
				int monitoringIntervalSeconds = (int)this.ReadAttribute("MonitoringInterval", TimeSpan.FromMinutes(5.0)).TotalSeconds;
				foreach (OwaErrorDetectionAlertDefinition owaErrorDetectionAlertDefinition in this.alertDefinitions)
				{
					if (owaErrorDetectionAlertDefinition.MonitorEnabled)
					{
						MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(owaErrorDetectionAlertDefinition.MonitorName, string.Format("{0}/{1}", ExchangeComponent.Eds.Name, owaErrorDetectionAlertDefinition.RedEvent.ToString()), owaErrorDetectionAlertDefinition.Component.Service, owaErrorDetectionAlertDefinition.Component, 1, true, 300);
						monitorDefinition.RecurrenceIntervalSeconds = recurrenceIntervalSeconds2;
						monitorDefinition.MonitoringIntervalSeconds = monitoringIntervalSeconds;
						MonitorStateTransition[] monitorStateTransitions = new MonitorStateTransition[]
						{
							new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 0)
						};
						monitorDefinition.MonitorStateTransitions = monitorStateTransitions;
						monitorDefinition.ServicePriority = 0;
						monitorDefinition.ScenarioDescription = "Validate OWA health is not impacted by any issues";
						base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
						ResponderDefinition responderDefinition = OwaErrorDetectionResponder.CreateResponderDefinition(owaErrorDetectionAlertDefinition.EscalateResponderName, owaErrorDetectionAlertDefinition.Component.Service, owaErrorDetectionAlertDefinition.MonitorName, owaErrorDetectionAlertDefinition.MonitorName, Environment.MachineName, ServiceHealthStatus.Unrecoverable, owaErrorDetectionAlertDefinition.Component.EscalationTeam, owaErrorDetectionAlertDefinition.MessageSubject, owaErrorDetectionAlertDefinition.MessageBody, owaErrorDetectionAlertDefinition.WhiteListedExceptions, owaErrorDetectionAlertDefinition.ResponderEnabled, owaErrorDetectionAlertDefinition.NotificationClass, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
						responderDefinition.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
						base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
					}
				}
				return;
			}
		}

		private void CreateAlertsRoutingConfig()
		{
			this.alertDefinitions = new List<OwaErrorDetectionAlertDefinition>();
			foreach (string text in base.Definition.Attributes.Keys)
			{
				string text2 = text;
				Guid guid = Guid.NewGuid();
				string[] array = this.ReadAttribute(text2, string.Empty).Split(new char[]
				{
					','
				});
				if (array.Length == 7 && !Guid.TryParse(text2, out guid))
				{
					string text3 = array[6];
					if (base.Definition.Attributes.ContainsKey(text3))
					{
						array = this.ReadAttribute(text3, string.Empty).Split(new char[]
						{
							','
						});
					}
					if (ExchangeComponent.WellKnownComponents.ContainsKey(array[0]))
					{
						Component component = ExchangeComponent.WellKnownComponents[array[0]];
						NotificationServiceClass responderType = NotificationServiceClass.Scheduled;
						bool monitorEnabled;
						bool responderEnabled;
						if (Enum.TryParse<NotificationServiceClass>(array[1], out responderType) && bool.TryParse(array[2], out monitorEnabled) && bool.TryParse(array[3], out responderEnabled))
						{
							this.alertDefinitions.Add(new OwaErrorDetectionAlertDefinition(text, component, responderType, monitorEnabled, responderEnabled, array[4], array[5]));
						}
					}
				}
			}
		}

		private List<OwaErrorDetectionAlertDefinition> alertDefinitions;
	}
}
