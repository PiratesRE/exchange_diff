using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TransportSync
{
	public sealed class TransportSyncExceptionAnalyzerMaintenance : MaintenanceWorkItem
	{
		public TimeSpan ExceptionRequestsMonitorRecurrenceInterval { get; private set; }

		public TimeSpan ExceptionRequestsMonitoringInterval { get; private set; }

		public TimeSpan ResponderRecurrenceInterval { get; private set; }

		public bool AlertResponderEnabled { get; private set; }

		public bool ExceptionRequestsMonitorEnabled { get; private set; }

		protected override void DoWork(CancellationToken cancellationToken)
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			string message;
			if (!instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				message = "TransportSyncExceptionAnalyzerMaintenance.DoWork: Mailbox role is not present on this server.";
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RBATracer, base.TraceContext, message, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\TransportSync\\Discovery\\TransportSyncExceptionAnalyzerMaintenance.cs", 97);
				return;
			}
			message = "TransportSyncExceptionAnalyzerMaintenance.DoWork: Mailbox role is present on this server, so creating monitor and responder definitions";
			WTFDiagnostics.TraceInformation(ExTraceGlobals.RBATracer, base.TraceContext, message, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\TransportSync\\Discovery\\TransportSyncExceptionAnalyzerMaintenance.cs", 102);
			this.Configure(base.TraceContext);
			foreach (TransportSyncExceptionAnalyzerAlertDefinition transportSyncExceptionAnalyzerAlertDefinition in new List<TransportSyncExceptionAnalyzerAlertDefinition>
			{
				this.CreateAlertDefinition(AggregationSubscriptionType.Pop, ExchangeComponent.MailboxMigration),
				this.CreateAlertDefinition(AggregationSubscriptionType.IMAP, ExchangeComponent.MailboxMigration),
				this.CreateAlertDefinition(AggregationSubscriptionType.DeltaSyncMail, ExchangeComponent.MailboxMigration),
				this.CreateAlertDefinition(AggregationSubscriptionType.Facebook, ExchangeComponent.PeopleConnect),
				this.CreateAlertDefinition(AggregationSubscriptionType.LinkedIn, ExchangeComponent.PeopleConnect)
			})
			{
				if (transportSyncExceptionAnalyzerAlertDefinition.IsEnabled)
				{
					MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(transportSyncExceptionAnalyzerAlertDefinition.MonitorName, string.Format("{0}/{1}", transportSyncExceptionAnalyzerAlertDefinition.Component.Name, transportSyncExceptionAnalyzerAlertDefinition.MonitorName), transportSyncExceptionAnalyzerAlertDefinition.Component.Name, transportSyncExceptionAnalyzerAlertDefinition.Component, 1, true, 300);
					monitorDefinition.MonitoringIntervalSeconds = (int)transportSyncExceptionAnalyzerAlertDefinition.MonitoringInterval.TotalSeconds;
					monitorDefinition.RecurrenceIntervalSeconds = (int)transportSyncExceptionAnalyzerAlertDefinition.RecurrenceInterval.TotalSeconds;
					monitorDefinition.ServicePriority = 2;
					monitorDefinition.ScenarioDescription = "Validate TransportSync is not impacted by any issues";
					base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
					if (this.AlertResponderEnabled)
					{
						ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(transportSyncExceptionAnalyzerAlertDefinition.MonitorName, transportSyncExceptionAnalyzerAlertDefinition.Component.Name, transportSyncExceptionAnalyzerAlertDefinition.MonitorName, transportSyncExceptionAnalyzerAlertDefinition.MonitorName, Environment.MachineName, ServiceHealthStatus.None, transportSyncExceptionAnalyzerAlertDefinition.Component.EscalationTeam, transportSyncExceptionAnalyzerAlertDefinition.MessageSubject, transportSyncExceptionAnalyzerAlertDefinition.MessageBody, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
						responderDefinition.RecurrenceIntervalSeconds = (int)this.ResponderRecurrenceInterval.TotalSeconds;
						base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
					}
				}
			}
		}

		private TransportSyncExceptionAnalyzerAlertDefinition CreateAlertDefinition(AggregationSubscriptionType subscriptionType, Component component)
		{
			return new TransportSyncExceptionAnalyzerAlertDefinition
			{
				RedEvent = string.Format("TxSync{0}RequestsWithExceptionsReachedThreshold", subscriptionType),
				MessageSubject = string.Format("The number of {0} subscription sync requests with exceptions has exceeded threshold.", subscriptionType),
				MessageBody = "{Probe.ExtensionXml}",
				RecurrenceInterval = this.ExceptionRequestsMonitorRecurrenceInterval,
				MonitoringInterval = this.ExceptionRequestsMonitoringInterval,
				IsEnabled = this.ExceptionRequestsMonitorEnabled,
				Component = component
			};
		}

		private void Configure(TracingContext context)
		{
			this.ExceptionRequestsMonitorRecurrenceInterval = this.ReadAttribute("ExceptionRequestsMonitorRecurrenceInterval", TransportSyncExceptionAnalyzerMaintenance.defaultMonitorRecurrenceInteral);
			this.ExceptionRequestsMonitoringInterval = this.ReadAttribute("ExceptionRequestsMonitoringInterval", TransportSyncExceptionAnalyzerMaintenance.defaultMonitoringInterval);
			this.ResponderRecurrenceInterval = this.ReadAttribute("ResponderRecurrenceInterval", TransportSyncExceptionAnalyzerMaintenance.defaultResponderRecurrenceInterval);
			this.AlertResponderEnabled = this.ReadAttribute("AlertResponderEnabled", TransportSyncExceptionAnalyzerMaintenance.defaultAlertResponderEnabled);
			this.ExceptionRequestsMonitorEnabled = this.ReadAttribute("ExceptionRequestsMonitorEnabled", TransportSyncExceptionAnalyzerMaintenance.defaultExceptionRequestsMonitorEnabled);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.TransportSyncTracer, context, "Configuration value are initialized successfully", null, "Configure", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\TransportSync\\Discovery\\TransportSyncExceptionAnalyzerMaintenance.cs", 191);
		}

		private static TimeSpan defaultMonitorRecurrenceInteral = TimeSpan.FromMinutes(5.0);

		private static TimeSpan defaultResponderRecurrenceInterval = TimeSpan.FromMinutes(5.0);

		private static TimeSpan defaultMonitoringInterval = TimeSpan.FromMinutes(15.0);

		private static bool defaultAlertResponderEnabled = true;

		private static bool defaultExceptionRequestsMonitorEnabled = true;
	}
}
