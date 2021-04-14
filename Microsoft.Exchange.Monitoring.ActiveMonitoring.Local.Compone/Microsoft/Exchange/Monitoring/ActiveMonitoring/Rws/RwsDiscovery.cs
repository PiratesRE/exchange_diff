using System;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Rws.Probes;
using Microsoft.Exchange.Net.MonitoringWebClient;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rws
{
	public sealed class RwsDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsDiscovery.DoWork: enter", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsDiscovery.cs", 103);
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			this.SetupRwsCallProbeContext(instance, "https://localhost:444/ecp/reportingwebservice/reporting.svc/");
		}

		private ProbeDefinition CreateProbe(string probeName, string probeTypeName, string endPoint, int recurrenceIntervalSeconds, int timeoutSeconds, int maxRetryAttempts, MailboxDatabaseInfo dbInfo)
		{
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsDiscovery.CreateProbe: Creating probe {0}", probeName, null, "CreateProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsDiscovery.cs", 158);
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = RwsDiscovery.AssemblyPath;
			probeDefinition.TypeName = RwsDiscovery.RwsProbeTypeName;
			probeDefinition.Name = probeName;
			probeDefinition.ServiceName = ExchangeComponent.Rws.Service;
			probeDefinition.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			probeDefinition.TimeoutSeconds = timeoutSeconds;
			probeDefinition.MaxRetryAttempts = maxRetryAttempts;
			probeDefinition.Account = dbInfo.MonitoringAccount + "@" + dbInfo.MonitoringAccountDomain;
			probeDefinition.AccountPassword = dbInfo.MonitoringAccountPassword;
			probeDefinition.AccountDisplayName = dbInfo.MonitoringAccount;
			probeDefinition.Endpoint = endPoint;
			probeDefinition.TargetResource = ((dbInfo == null) ? string.Empty : dbInfo.MailboxDatabaseName);
			probeDefinition.Attributes["SslValidationOptions"] = SslValidationOptions.NoSslValidation.ToString();
			WTFDiagnostics.TraceInformation<string, string, string>(ExTraceGlobals.RWSTracer, base.TraceContext, "puid:{0}/sid:{1}/partitionId:{2}", dbInfo.MonitoringAccountPuid, dbInfo.MonitoringAccountSid, dbInfo.MonitoringAccountPartitionId, null, "CreateProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsDiscovery.cs", 183);
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsDiscovery.CreateProbe: Created probe {0}", probeName, null, "CreateProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsDiscovery.cs", 191);
			return probeDefinition;
		}

		private MonitorDefinition CreateMonitor(string monitorName, string sampleMask, double availabilityPercentage, TimeSpan recurrenceInterval, TimeSpan monitoringInterval, TimeSpan secondaryMonitoringInterval, MonitorStateTransition[] monitorStateTransitions)
		{
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsDiscovery.CreateMonitor: Creating monitor {0} of type {1}", monitorName, RwsDiscovery.OverallPercentSuccessByStateAttribute1MonitorTypeName, null, "CreateMonitor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsDiscovery.cs", 220);
			MonitorDefinition monitorDefinition = OverallPercentSuccessByStateAttribute1Monitor.CreateDefinition(monitorName, sampleMask, ExchangeComponent.Rws.Service, ExchangeComponent.Rws, availabilityPercentage, monitoringInterval, recurrenceInterval, secondaryMonitoringInterval, "", true);
			monitorDefinition.MonitorStateTransitions = monitorStateTransitions;
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsDiscovery.CreateMonitor: Creating monitor {0} of type {1}", monitorName, RwsDiscovery.OverallPercentSuccessByStateAttribute1MonitorTypeName, null, "CreateMonitor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsDiscovery.cs", 239);
			return monitorDefinition;
		}

		private ResponderDefinition CreateResponder(string responderName, string assemblyPath, string responderTypeName, ServiceHealthStatus targetHealthState, string alertTypeId, string alertMask, int recurrenceIntervalSeconds, int timeoutSeconds, int waitIntervalSeconds, string serviceName)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, string.Format("RwsDiscovery.CreateResponder: Creating responder {0} of type {1}", responderName, responderTypeName), null, "CreateResponder", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsDiscovery.cs", 275);
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.Name = responderName;
			responderDefinition.AssemblyPath = assemblyPath;
			responderDefinition.TypeName = responderTypeName;
			responderDefinition.TargetHealthState = targetHealthState;
			responderDefinition.AlertTypeId = alertTypeId;
			responderDefinition.AlertMask = alertMask;
			responderDefinition.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			responderDefinition.TimeoutSeconds = timeoutSeconds;
			responderDefinition.WaitIntervalSeconds = waitIntervalSeconds;
			responderDefinition.ServiceName = serviceName;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, string.Format("RwsDiscovery.CreateResponder: Created responder {0} of type {1}", responderName, responderTypeName), null, "CreateResponder", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsDiscovery.cs", 292);
			return responderDefinition;
		}

		private void SetupRwsCallProbeContext(LocalEndpointManager endpointManager, string endpointUrl)
		{
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsDiscovery.SetupRwsCallContext: Begin to setup context for RwsCallProbe. Endpoint url: {0}", endpointUrl, null, "SetupRwsCallProbeContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsDiscovery.cs", 307);
			int timeoutSeconds = 180;
			int maxRetryAttempts = 0;
			string text = this.BuildWorkItemName(RwsDiscovery.RwsSelfTestString, RwsDiscovery.ProbeString);
			int recurrenceIntervalSeconds = 600;
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in endpointManager.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend)
			{
				if (string.IsNullOrWhiteSpace(mailboxDatabaseInfo.MonitoringAccount))
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsDiscovery.DoWork: Ignore mailbox database {0} because it does not have monitoring mailbox", mailboxDatabaseInfo.MailboxDatabaseName, null, "SetupRwsCallProbeContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsDiscovery.cs", 325);
				}
				else if (string.IsNullOrWhiteSpace(mailboxDatabaseInfo.MonitoringAccountPuid) || string.IsNullOrWhiteSpace(mailboxDatabaseInfo.MonitoringAccountPartitionId) || string.IsNullOrWhiteSpace(mailboxDatabaseInfo.MonitoringAccountSid))
				{
					WTFDiagnostics.TraceInformation<string, string, string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsDiscovery.DoWork: Ignore mailbox database {0} due to missing mandatory fields on the monitoring mailbox Sid={0} Partition={1} Puid={2}", mailboxDatabaseInfo.MonitoringAccountSid, mailboxDatabaseInfo.MonitoringAccountPartitionId, mailboxDatabaseInfo.MonitoringAccountPuid, null, "SetupRwsCallProbeContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsDiscovery.cs", 337);
				}
				else
				{
					ProbeDefinition definition = this.CreateProbe(text, RwsDiscovery.RwsProbeTypeName, endpointUrl, recurrenceIntervalSeconds, timeoutSeconds, maxRetryAttempts, mailboxDatabaseInfo);
					base.Broker.AddWorkDefinition<ProbeDefinition>(definition, base.TraceContext);
				}
			}
			MonitorDefinition monitorDefinition = this.CreateMonitor(this.BuildWorkItemName(RwsDiscovery.RwsSelfTestString, RwsDiscovery.MonitorString), text, 60.0, TimeSpan.FromMinutes(10.0), TimeSpan.FromMinutes(30.0), TimeSpan.FromMinutes(90.0), new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)TimeSpan.FromMinutes(30.0).TotalSeconds)
			});
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Valdiate RWS health is not impacted by any issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string responderName = this.BuildWorkItemName(RwsDiscovery.RwsSelfTestString, RwsDiscovery.ResetIISAppPoolString);
			string name = this.BuildWorkItemName(RwsDiscovery.RwsSelfTestString, RwsDiscovery.EscalateString);
			ResponderDefinition responderDefinition = this.CreateResponder(responderName, RwsDiscovery.AssemblyPath, RwsDiscovery.ResetIISAppPoolResponderTypeName, ServiceHealthStatus.Degraded, monitorDefinition.Name, monitorDefinition.Name, 300, 60, 300, ExchangeComponent.Rws.Service);
			responderDefinition.Attributes["AppPoolName"] = "MSExchangeReportingWebServiceAppPool";
			ResponderDefinition definition2 = OBDEscalateResponder.CreateDefinition(name, ExchangeComponent.Rws.Service, monitorDefinition.Name, monitorDefinition.Name, Environment.MachineName, ServiceHealthStatus.Unrecoverable, ExchangeComponent.Rws.EscalationTeam, Strings.EscalationSubjectUnhealthy, Strings.EscalationMessagePercentUnhealthy(Strings.GenericOverallXFailureEscalationMessage(ExchangeComponent.Rws.Name)), true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition2, base.TraceContext);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsDiscovery.SetupRwsCallProbeContext: Finish to setup context for RwsCallProbe", null, "SetupRwsCallProbeContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsDiscovery.cs", 413);
		}

		private string BuildWorkItemName(string probeType, string workItemType)
		{
			return string.Format("{0}{1}", probeType, workItemType);
		}

		private const string RwsAppPoolName = "MSExchangeReportingWebServiceAppPool";

		private const string RwsEndpointUrl = "https://localhost:444/ecp/reportingwebservice/reporting.svc/";

		internal static readonly string RwsSelfTestString = "RwsSelfTest";

		internal static readonly string ProbeString = "Probe";

		internal static readonly string MonitorString = "Monitor";

		internal static readonly string ResetIISAppPoolString = "ResetIISAppPool";

		internal static readonly string EscalateString = "Escalate";

		private static readonly string RwsMailboxActivityReportUrl = string.Format("{0}{1}", "https://localhost:444/ecp/reportingwebservice/reporting.svc/", "MailboxActivityDaily");

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string RwsProbeTypeName = typeof(RwsProbe).FullName;

		private static readonly string OverallPercentSuccessByStateAttribute1MonitorTypeName = typeof(OverallPercentSuccessByStateAttribute1Monitor).FullName;

		private static readonly string ResetIISAppPoolResponderTypeName = typeof(ResetIISAppPoolResponder).FullName;

		private static readonly string EscalateResponderTypeName = typeof(OBDEscalateResponder).FullName;
	}
}
