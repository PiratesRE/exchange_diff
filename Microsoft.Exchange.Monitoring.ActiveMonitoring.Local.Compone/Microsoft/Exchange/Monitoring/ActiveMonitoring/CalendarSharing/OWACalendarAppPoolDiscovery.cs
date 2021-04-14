using System;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.CalendarSharing.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.CalendarSharing
{
	public sealed class OWACalendarAppPoolDiscovery : MaintenanceWorkItem
	{
		public string OWACalendarAppPoolUrl { get; private set; }

		public bool IsOnPremisesEnabled { get; private set; }

		public int UnrecoverableTransitionSpan { get; private set; }

		public int ProbeRecurrenceInterval { get; private set; }

		public int MonitorInterval { get; private set; }

		public int MonitorRecurrenceInterval { get; private set; }

		public int ResponderRecurrenceInterval { get; private set; }

		public int ProbeTimeout { get; private set; }

		public int AlertResponderWaitInterval { get; private set; }

		public int ResetAppPoolResponderWaitInterval { get; private set; }

		public int FailureCount { get; private set; }

		public bool IsAlertResponderEnabled { get; private set; }

		public bool IsRestartAppPoolResponderEnabled { get; private set; }

		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				LocalEndpointManager instance = LocalEndpointManager.Instance;
				this.OWACalendarAppPoolUrl = this.ReadAttribute("OWACalendarAppPoolUrl", OWACalendarAppPoolDiscovery.DefaultOWACalendarAppPoolUrl);
				this.IsOnPremisesEnabled = this.ReadAttribute("EnableOnPrem", false);
				this.ProbeRecurrenceInterval = (int)this.ReadAttribute("ProbeRecurrenceInterval", OWACalendarAppPoolDiscovery.DefaultProbeRecurrenceInterval).TotalSeconds;
				this.MonitorRecurrenceInterval = (int)this.ReadAttribute("MonitorRecurrenceInterval", OWACalendarAppPoolDiscovery.DefaultMonitorRecurrenceInterval).TotalSeconds;
				this.MonitorInterval = (int)this.ReadAttribute("MonitorInterval", OWACalendarAppPoolDiscovery.DefaultMonitorInterval).TotalSeconds;
				this.ResponderRecurrenceInterval = (int)this.ReadAttribute("ResponderRecurrenceInterval", OWACalendarAppPoolDiscovery.DefaultResponderRecurrenceInterval).TotalSeconds;
				this.ProbeTimeout = (int)this.ReadAttribute("ProbeTimeout", OWACalendarAppPoolDiscovery.DefaultProbeTimeout).TotalSeconds;
				this.AlertResponderWaitInterval = (int)this.ReadAttribute("AlertResponderWaitInterval", OWACalendarAppPoolDiscovery.DefaultAlertResponderWaitInterval).TotalSeconds;
				this.ResetAppPoolResponderWaitInterval = (int)this.ReadAttribute("ResetAppPoolResponderWaitInterval", OWACalendarAppPoolDiscovery.DefaultResetAppPoolResponderWaitInterval).TotalSeconds;
				this.FailureCount = this.ReadAttribute("FailureCount", OWACalendarAppPoolDiscovery.DefaultFailureCount);
				this.UnrecoverableTransitionSpan = (int)this.ReadAttribute("UnrecoverableTransitionSpan", OWACalendarAppPoolDiscovery.DefaultUnrecoverableTransitionSpan).TotalSeconds;
				this.IsAlertResponderEnabled = this.ReadAttribute("AlertResponderEnabled", true);
				this.IsRestartAppPoolResponderEnabled = this.ReadAttribute("RestartAppPoolResponderEnabled", true);
				this.breadcrumbs = new Breadcrumbs(1024, base.TraceContext);
				if (!LocalEndpointManager.IsDataCenter && !this.IsOnPremisesEnabled)
				{
					this.breadcrumbs.Drop("OWACalendarAppPoolDiscovery.DoWork: Skip creating the probe on On-Prem servers with IsOnPremEnabled flag set to false");
				}
				else if (!instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled || instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled)
				{
					this.breadcrumbs.Drop("OWACalendarAppPoolDiscovery.DoWork: Skip creating the probe for non-MBX server or MBX server with Cafe installed");
				}
				else
				{
					this.SetupOWACalendarAppPoolMonitoring(base.TraceContext, instance);
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CalendarSharingTracer, base.TraceContext, "OWACalendarAppPoolDiscovery.DoWork: Created OWACalendarAppPool probe, monitor and responder for server {0}", Environment.MachineName, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\CalendarSharing\\OWACalendarAppPoolDiscovery.cs", 168);
				}
			}
			finally
			{
				this.ReportResult();
			}
		}

		private void ReportResult()
		{
			string text = this.breadcrumbs.ToString();
			base.Result.StateAttribute5 = text;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.CalendarSharingTracer, base.TraceContext, text, null, "ReportResult", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\CalendarSharing\\OWACalendarAppPoolDiscovery.cs", 189);
		}

		private void SetupOWACalendarAppPoolMonitoring(TracingContext traceContext, LocalEndpointManager endpointManager)
		{
			Strings.OWACalendarAppPoolEscalationSubject(Environment.MachineName);
			Strings.OWACalendarAppPoolEscalationBody;
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CalendarSharingTracer, base.TraceContext, "OWACalendarAppPoolDiscovery.SetupOWACalendarAppPoolMonitoring: Creating {0} for this server", "OWACalendarSelfTestProbe", null, "SetupOWACalendarAppPoolMonitoring", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\CalendarSharing\\OWACalendarAppPoolDiscovery.cs", 206);
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = OWACalendarAppPoolDiscovery.AssemblyPath;
			probeDefinition.TypeName = OWACalendarAppPoolDiscovery.OWACalendarAppPoolProbeTypeName;
			probeDefinition.Name = "OWACalendarSelfTestProbe";
			probeDefinition.TargetResource = Environment.MachineName;
			probeDefinition.RecurrenceIntervalSeconds = this.ProbeRecurrenceInterval;
			probeDefinition.TimeoutSeconds = this.ProbeTimeout;
			probeDefinition.MaxRetryAttempts = 0;
			probeDefinition.Endpoint = this.OWACalendarAppPoolUrl;
			probeDefinition.ServiceName = ExchangeComponent.Calendaring.Name;
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CalendarSharingTracer, base.TraceContext, "OWACalendarAppPoolDiscovery.SetupOWACalendarAppPoolMonitoring: Creating {0} for this server", "OWACalendarSelfTestMonitor", null, "SetupOWACalendarAppPoolMonitoring", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\CalendarSharing\\OWACalendarAppPoolDiscovery.cs", 226);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("OWACalendarSelfTestMonitor", probeDefinition.ConstructWorkItemResultName(), ExchangeComponent.Calendaring.Name, ExchangeComponent.Calendaring, this.FailureCount, true, this.MonitorInterval);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, this.UnrecoverableTransitionSpan)
			};
			monitorDefinition.RecurrenceIntervalSeconds = this.MonitorRecurrenceInterval;
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Validate OWA calendar health is not impacetd by apppool issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string text = monitorDefinition.ConstructWorkItemResultName();
			if (this.IsRestartAppPoolResponderEnabled)
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CalendarSharingTracer, base.TraceContext, "OWACalendarAppPoolDiscovery.SetupOWACalendarAppPoolMonitoring: Creating {0} for this server", "OWACalendarSelfTestRecycleAppPool", null, "SetupOWACalendarAppPoolMonitoring", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\CalendarSharing\\OWACalendarAppPoolDiscovery.cs", 264);
				string responderName = "OWACalendarSelfTestRecycleAppPool";
				string monitorName = text;
				string appPoolName = "OWACalendarAppPool";
				ServiceHealthStatus responderTargetState = ServiceHealthStatus.Degraded;
				bool enabled = true;
				ResponderDefinition responderDefinition = ResetIISAppPoolResponder.CreateDefinition(responderName, monitorName, appPoolName, responderTargetState, DumpMode.None, null, 15.0, 0, ExchangeComponent.Calendaring.Name, enabled, "Dag");
				responderDefinition.MinimumSecondsBetweenEscalates = this.ResetAppPoolResponderWaitInterval;
				responderDefinition.WaitIntervalSeconds = this.ResetAppPoolResponderWaitInterval;
				responderDefinition.RecurrenceIntervalSeconds = this.ResponderRecurrenceInterval;
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			}
			if (this.IsAlertResponderEnabled)
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.CalendarSharingTracer, base.TraceContext, "OWACalendarAppPoolDiscovery.SetupOWACalendarAppPoolMonitoring: Creating {0} for this server", "OWACalendarSelfTestEscalate", null, "SetupOWACalendarAppPoolMonitoring", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\CalendarSharing\\OWACalendarAppPoolDiscovery.cs", 288);
				ResponderDefinition responderDefinition2 = EscalateResponder.CreateDefinition("OWACalendarSelfTestEscalate", ExchangeComponent.Calendaring.Name, "OWACalendarSelfTestMonitor", text, Environment.MachineName, ServiceHealthStatus.Unrecoverable, ExchangeComponent.Calendaring.EscalationTeam, Strings.OWACalendarAppPoolEscalationSubject(Environment.MachineName), Strings.OWACalendarAppPoolEscalationBody, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
				responderDefinition2.WaitIntervalSeconds = this.AlertResponderWaitInterval;
				responderDefinition2.MinimumSecondsBetweenEscalates = this.AlertResponderWaitInterval;
				responderDefinition2.RecurrenceIntervalSeconds = this.ResponderRecurrenceInterval;
				responderDefinition2.NotificationServiceClass = NotificationServiceClass.Scheduled;
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
			}
		}

		private const string ProbeName = "OWACalendarSelfTestProbe";

		private const string MonitorName = "OWACalendarSelfTestMonitor";

		private const string EscalateResponderName = "OWACalendarSelfTestEscalate";

		private const string RecycleAppPoolResponderName = "OWACalendarSelfTestRecycleAppPool";

		private const string OWACalendarAppPoolName = "OWACalendarAppPool";

		public static readonly string DefaultOWACalendarAppPoolUrl = "http://localhost:81/owa/calendar/ping.owa";

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly TimeSpan DefaultProbeRecurrenceInterval = TimeSpan.FromMinutes(3.0);

		private static readonly TimeSpan DefaultMonitorRecurrenceInterval = TimeSpan.FromMinutes(0.0);

		private static readonly TimeSpan DefaultMonitorInterval = TimeSpan.FromMinutes(12.0);

		private static readonly TimeSpan DefaultResponderRecurrenceInterval = TimeSpan.FromMinutes(3.0);

		private static readonly TimeSpan DefaultAlertResponderWaitInterval = TimeSpan.FromHours(2.0);

		private static readonly TimeSpan DefaultResetAppPoolResponderWaitInterval = TimeSpan.FromHours(2.0);

		private static readonly TimeSpan DefaultProbeTimeout = TimeSpan.FromMinutes(2.0);

		private static readonly TimeSpan DefaultUnrecoverableTransitionSpan = TimeSpan.FromMinutes(15.0);

		private static readonly int DefaultFailureCount = 4;

		private Breadcrumbs breadcrumbs;

		private static readonly string OWACalendarAppPoolProbeTypeName = typeof(OWACalendarAppPoolProbe).FullName;
	}
}
