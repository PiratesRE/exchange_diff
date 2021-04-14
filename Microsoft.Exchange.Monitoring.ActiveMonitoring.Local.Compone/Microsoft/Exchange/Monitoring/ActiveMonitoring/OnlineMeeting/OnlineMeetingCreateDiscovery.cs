using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.OnlineMeeting.Probes;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.OnlineMeeting
{
	public sealed class OnlineMeetingCreateDiscovery : MaintenanceWorkItem
	{
		public int ProbeRecurrenceInterval { get; private set; }

		public int ProbeTimeout { get; private set; }

		public int ResponderWaitInterval { get; private set; }

		public int FailureCount { get; private set; }

		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				LocalEndpointManager instance = LocalEndpointManager.Instance;
				this.ProbeRecurrenceInterval = (int)this.ReadAttribute("ProbeRecurrenceInterval", OnlineMeetingCreateDiscovery.DefaultProbeRecurrenceInterval).TotalSeconds;
				this.ProbeTimeout = (int)this.ReadAttribute("ProbeTimeout", OnlineMeetingCreateDiscovery.DefaultProbeTimeout).TotalSeconds;
				this.ResponderWaitInterval = (int)this.ReadAttribute("ResponderWaitInterval", OnlineMeetingCreateDiscovery.DefaultResponderWaitInterval).TotalSeconds;
				this.FailureCount = this.ReadAttribute("FailureCount", OnlineMeetingCreateDiscovery.DefaultFailureCount);
				this.breadcrumbs = new Breadcrumbs(1024, base.TraceContext);
				if (!LocalEndpointManager.IsDataCenter)
				{
					this.breadcrumbs.Drop("OnlineMeetingCreateDiscovery.DoWork: Skip creating the probe if not datacenter");
				}
				else if (!instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
				{
					this.breadcrumbs.Drop("OnlineMeetingCreateDiscovery.DoWork: Skip creating the probe for non-MBX server");
				}
				else
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OnlineMeetingTracer, base.TraceContext, "OnlineMeetingCreateDiscovery.SetupOnlineMeetingCreateProbe: Creating OnlineMeetingCreate BE probe for server {0}", Environment.MachineName, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OnlineMeeting\\OnlineMeetingCreateDiscovery.cs", 132);
					this.SetupOnlineMeetingCreateProbe(base.TraceContext, instance);
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OnlineMeetingTracer, base.TraceContext, "OnlineMeetingCreateDiscovery.SetupOnlineMeetingCreateProbe: Created OnlineMeetingCreate BE probe for server {0}", Environment.MachineName, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OnlineMeeting\\OnlineMeetingCreateDiscovery.cs", 140);
				}
			}
			finally
			{
				this.ReportResult();
			}
		}

		private void ReportResult()
		{
			if (base.Result != null)
			{
				string text = this.breadcrumbs.ToString();
				base.Result.StateAttribute5 = text;
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OnlineMeetingTracer, base.TraceContext, text, null, "ReportResult", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OnlineMeeting\\OnlineMeetingCreateDiscovery.cs", 163);
			}
		}

		private void SetupOnlineMeetingCreateProbe(TracingContext traceContext, LocalEndpointManager endpointManager)
		{
			IEnumerable<MailboxDatabaseInfo> enumerable = endpointManager.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend;
			enumerable = from x in enumerable
			where !string.IsNullOrEmpty(x.MonitoringAccount) && !string.IsNullOrEmpty(x.MonitoringAccountPassword)
			select x;
			if (enumerable == null || enumerable.Count<MailboxDatabaseInfo>() == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OnlineMeetingTracer, base.TraceContext, "OnlineMeetingCreateDiscovery.DoWork: No mailbox databases were found with a monitoring mailbox", null, "SetupOnlineMeetingCreateProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OnlineMeeting\\OnlineMeetingCreateDiscovery.cs", 186);
				return;
			}
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in enumerable)
			{
				if (DirectoryAccessor.Instance.IsDatabaseCopyActiveOnLocalServer(mailboxDatabaseInfo.MailboxDatabaseGuid))
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.OnlineMeetingTracer, base.TraceContext, "OnlineMeetingCreateDiscovery.SetupOnlineMeetingCreateProbe: Creating OnlineMeetingCreate probe for this server", null, "SetupOnlineMeetingCreateProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OnlineMeeting\\OnlineMeetingCreateDiscovery.cs", 197);
					ProbeDefinition probeDefinition = new ProbeDefinition();
					probeDefinition.AssemblyPath = OnlineMeetingCreateDiscovery.AssemblyPath;
					probeDefinition.TypeName = OnlineMeetingCreateDiscovery.OnlineMeetingCreateProbeTypeName;
					probeDefinition.Name = "OnlineMeetingCreateProbe";
					probeDefinition.Account = mailboxDatabaseInfo.MonitoringAccount + "@" + mailboxDatabaseInfo.MonitoringAccountDomain;
					probeDefinition.AccountPassword = mailboxDatabaseInfo.MonitoringAccountPassword;
					probeDefinition.AccountDisplayName = mailboxDatabaseInfo.MonitoringAccount;
					probeDefinition.SecondaryAccount = mailboxDatabaseInfo.MonitoringAccount + "@" + mailboxDatabaseInfo.MonitoringAccountDomain;
					probeDefinition.SecondaryAccountPassword = mailboxDatabaseInfo.MonitoringAccountPassword;
					probeDefinition.SecondaryAccountDisplayName = mailboxDatabaseInfo.MonitoringAccount;
					probeDefinition.TargetResource = Environment.MachineName;
					probeDefinition.RecurrenceIntervalSeconds = this.ProbeRecurrenceInterval;
					probeDefinition.TimeoutSeconds = this.ProbeTimeout;
					probeDefinition.MaxRetryAttempts = 0;
					probeDefinition.Endpoint = OnlineMeetingCreateDiscovery.ProbeEndPoint.TrimEnd(new char[]
					{
						'/'
					}) + "/ews/exchange.asmx";
					probeDefinition.SecondaryEndpoint = "/owa/service.svc?action=CreateOnlineMeeting";
					probeDefinition.ServiceName = ExchangeComponent.OnlineMeeting.Name;
					base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
					this.CopyAttributes(OnlineMeetingCreateDiscovery.StandardAttributes, probeDefinition);
					probeDefinition.Attributes["DatabaseGuid"] = mailboxDatabaseInfo.MailboxDatabaseGuid.ToString();
					probeDefinition.Attributes["UMMbxAccountSipUri"] = mailboxDatabaseInfo.MonitoringAccountSipAddress;
					break;
				}
			}
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("OnlineMeetingCreateMonitor", "OnlineMeetingCreateProbe", ExchangeComponent.OnlineMeeting.Name, ExchangeComponent.OnlineMeeting, this.FailureCount, true, 300);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.OnlineMeetingTracer, base.TraceContext, "Configured monitor " + monitorDefinition.Name, null, "SetupOnlineMeetingCreateProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OnlineMeeting\\OnlineMeetingCreateDiscovery.cs", 243);
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate OnlineMeeting health is not impacted by any issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string alertMask = monitorDefinition.ConstructWorkItemResultName();
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition("OnlineMeetingCreateResponder", ExchangeComponent.OnlineMeeting.Name, "OnlineMeetingCreateMonitor", alertMask, Environment.MachineName, ServiceHealthStatus.None, ExchangeComponent.OnlineMeeting.EscalationTeam, "The OnlineMeetingCreate request failed on {Probe.MachineName}", "The OnlineMeetingCreate request failed with the error below.\n\n Error: {Probe.Error} \n Exception: {Probe.Exception} \n FailureContext: {Probe.FailureContext} \n Execution Context: {Probe.ExecutionContext} \n ResultName : {Probe.ResultName}", true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.WaitIntervalSeconds = this.ResponderWaitInterval;
			responderDefinition.NotificationServiceClass = NotificationServiceClass.UrgentInTraining;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.OnlineMeetingTracer, base.TraceContext, "OnlineMeetingCreateDiscovery.SetupOnlineMeetingCreateProbe: Created OnlineMeetingCreate probe for this server", null, "SetupOnlineMeetingCreateProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\OnlineMeeting\\OnlineMeetingCreateDiscovery.cs", 268);
		}

		public const string OnlineMeetingCreateUrl = "/owa/service.svc?action=CreateOnlineMeeting";

		private const string ProbeName = "OnlineMeetingCreateProbe";

		private const string MonitorName = "OnlineMeetingCreateMonitor";

		private const string ResponderName = "OnlineMeetingCreateResponder";

		public const string MailboxDatabaseGuidParameterName = "DatabaseGuid";

		public const string MailboxDatabaseSipUriParameterName = "UMMbxAccountSipUri";

		public static readonly string ProbeEndPoint = Uri.UriSchemeHttps + "://localhost/";

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly TimeSpan DefaultProbeRecurrenceInterval = TimeSpan.FromMinutes(15.0);

		private static readonly TimeSpan DefaultResponderWaitInterval = TimeSpan.FromHours(8.0);

		private static readonly TimeSpan DefaultProbeTimeout = TimeSpan.FromMinutes(2.0);

		private static readonly int DefaultFailureCount = 4;

		private Breadcrumbs breadcrumbs;

		private static readonly string OnlineMeetingCreateProbeTypeName = typeof(OnlineMeetingCreateProbe).FullName;

		private static readonly string[] StandardAttributes = new string[]
		{
			"ApiRetryCount",
			"IsOutsideInMonitoring",
			"PrimaryAuthN",
			"TargetPort",
			"TrustAnySslCertificate",
			"UserAgentPart",
			"ApiRetrySleepInMilliSeconds"
		};
	}
}
