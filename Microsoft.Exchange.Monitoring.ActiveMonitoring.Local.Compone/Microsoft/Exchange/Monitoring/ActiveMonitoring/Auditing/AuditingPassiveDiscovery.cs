using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Auditing
{
	public sealed class AuditingPassiveDiscovery : DiscoveryWorkItem
	{
		public static string MonitorName(string eventName)
		{
			return eventName + "Monitor";
		}

		public static string ResponderName(string eventName)
		{
			return eventName + "EscalateResponder";
		}

		public static string FailureTriggerErrorMask(string failureTriggerError)
		{
			return NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, failureTriggerError, null);
		}

		protected override void CreateWorkTasks(CancellationToken cancellationToken)
		{
			this.breadcrumbs = new Breadcrumbs(1024, this.traceContext);
			try
			{
				if (!LocalEndpointManager.IsDataCenter)
				{
					this.breadcrumbs.Drop("CreateWorkTasks: Datacenter only.");
				}
				else if (!LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
				{
					this.breadcrumbs.Drop("CreateWorkTasks: Mailbox role is not present on this server.");
				}
				else
				{
					this.Configure();
					if (this.monitorActivationStatus[PassiveMonitorType.MailboxAuditingAvailability])
					{
						this.breadcrumbs.Drop("Creating Mailbox Auditing Availability Failure work definitions");
						this.CreateFailureMonitorAndResponder(AuditingPassiveDiscovery.MailboxAuditingAvailabilityFailureEvent, AuditingPassiveDiscovery.FailureTriggerErrorMask(AuditingPassiveDiscovery.MailboxAuditingAvailabilityFailureError), ExchangeComponent.Compliance, 1800, 1800, 2, Strings.MailboxAuditingAvailabilityFailureEscalationSubject, Strings.MailboxAuditingAvailabilityFailureEscalationBody, NotificationServiceClass.UrgentInTraining);
					}
					if (this.monitorActivationStatus[PassiveMonitorType.AdminAuditingAvailability])
					{
						this.breadcrumbs.Drop("Creating Admin Auditing Availability Failure work definitions");
						this.CreateFailureMonitorAndResponder(AuditingPassiveDiscovery.AdminAuditingAvailabilityFailureEvent, AuditingPassiveDiscovery.FailureTriggerErrorMask(AuditingPassiveDiscovery.AdminAuditingAvailabilityFailureError), ExchangeComponent.Compliance, 1800, 1800, 2, Strings.AdminAuditingAvailabilityFailureEscalationSubject, Strings.AdminAuditingAvailabilityFailureEscalationBody, NotificationServiceClass.UrgentInTraining);
					}
					if (this.monitorActivationStatus[PassiveMonitorType.SynchronousAuditSearchAvailability])
					{
						this.breadcrumbs.Drop("Creating Synchronous Audit Search Availability Failure work definitions");
						this.CreateFailureMonitorAndResponder(AuditingPassiveDiscovery.SynchronousAuditSearchAvailabilityFailureEvent, AuditingPassiveDiscovery.FailureTriggerErrorMask(AuditingPassiveDiscovery.SynchronousAuditSearchAvailabilityFailureError), ExchangeComponent.Compliance, 1800, 1800, 2, Strings.SynchronousAuditSearchAvailabilityFailureEscalationSubject, Strings.SynchronousAuditSearchAvailabilityFailureEscalationBody, NotificationServiceClass.Urgent);
					}
					if (this.monitorActivationStatus[PassiveMonitorType.AsynchronousAuditSearchAvailability])
					{
						this.breadcrumbs.Drop("Creating Asynchronous Audit Search Availability Failure work definitions");
						this.CreateFailureMonitorAndResponder(AuditingPassiveDiscovery.AsynchronousAuditSearchAvailabilityFailureEvent, AuditingPassiveDiscovery.FailureTriggerErrorMask(AuditingPassiveDiscovery.AsynchronousAuditSearchAvailabilityFailureError), ExchangeComponent.Compliance, 1800, 1800, 2, Strings.AsynchronousAuditSearchAvailabilityFailureEscalationSubject, Strings.AsynchronousAuditSearchAvailabilityFailureEscalationBody, NotificationServiceClass.Urgent);
					}
				}
			}
			finally
			{
				this.ReportResult();
			}
		}

		protected override Trace Trace
		{
			get
			{
				return ExTraceGlobals.AuditTracer;
			}
		}

		private void CreateFailureMonitorAndResponder(string eventName, string sampleMask, Component exchangeComponent, int monitoringIntervalSeconds, int recurrenceIntervalSeconds, int failureThreshold, string escalationSubject, string escalationBody, NotificationServiceClass alertClass)
		{
			string text = AuditingPassiveDiscovery.MonitorName(eventName);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition(text, sampleMask, exchangeComponent.Name, exchangeComponent, monitoringIntervalSeconds, recurrenceIntervalSeconds, failureThreshold, true);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, this.traceContext);
			ResponderDefinition definition = EscalateResponder.CreateDefinition(AuditingPassiveDiscovery.ResponderName(eventName), exchangeComponent.Name, text, monitorDefinition.ConstructWorkItemResultName(), exchangeComponent.Name, ServiceHealthStatus.Unhealthy, ExchangeComponent.Auditing.EscalationTeam, escalationSubject, escalationBody, true, alertClass, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, this.traceContext);
		}

		private void ReportResult()
		{
			string text = this.breadcrumbs.ToString();
			base.Result.StateAttribute5 = text;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.AuditTracer, this.traceContext, text, null, "ReportResult", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Auditing\\AuditingPassiveDiscovery.cs", 262);
		}

		private void Configure()
		{
			foreach (object obj in Enum.GetValues(typeof(PassiveMonitorType)))
			{
				PassiveMonitorType passiveMonitorType = (PassiveMonitorType)obj;
				this.monitorActivationStatus[passiveMonitorType] = this.ReadAttribute(passiveMonitorType + "MonitorEnabled", false);
			}
		}

		internal const string MonitorNameSuffix = "Monitor";

		private const string ResponderNameSuffix = "EscalateResponder";

		private const int DefaultFailureThreshold = 2;

		private const int DefaultIntervalSeconds = 1800;

		public static readonly string AdminAuditingAvailabilityFailureEvent = "ComplianceAdminAuditingAvailabilityFailure";

		public static readonly string AdminAuditingAvailabilityFailureError = "AdminAuditingFailureAboveThreshold";

		public static readonly string MailboxAuditingAvailabilityFailureEvent = "ComplianceMbxAuditingAvailabilityFailure";

		public static readonly string MailboxAuditingAvailabilityFailureError = "MailboxAuditingFailureAboveThreshold";

		public static readonly string SynchronousAuditSearchAvailabilityFailureEvent = "ComplianceSyncAuditSearchAvailabilityFailure";

		public static readonly string SynchronousAuditSearchAvailabilityFailureError = "SynchronousAuditSearchFailureAboveThreshold";

		public static readonly string AsynchronousAuditSearchAvailabilityFailureEvent = "ComplianceAsyncAuditSearchAvailabilityFailure";

		public static readonly string AsynchronousAuditSearchAvailabilityFailureError = "AsynchronousAuditSearchFailureAboveThreshold";

		private Breadcrumbs breadcrumbs;

		private Dictionary<PassiveMonitorType, bool> monitorActivationStatus = new Dictionary<PassiveMonitorType, bool>();

		private TracingContext traceContext = TracingContext.Default;
	}
}
