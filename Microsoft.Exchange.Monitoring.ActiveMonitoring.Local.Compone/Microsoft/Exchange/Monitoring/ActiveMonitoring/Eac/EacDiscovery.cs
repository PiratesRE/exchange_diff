using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Eac.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Net.MonitoringWebClient;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Eac
{
	public sealed class EacDiscovery : MaintenanceWorkItem
	{
		public bool LogSuccessProbeResult { get; private set; }

		public int ConsecutiveFailureCount { get; private set; }

		public int ProbeMaxRetryAttempts { get; private set; }

		public int EacBackEndPingProbeRecurrenceInterval { get; private set; }

		public int EacBackEndPingProbeTimeout { get; private set; }

		public int EacBackEndPingMonitorRecurrenceInterval { get; private set; }

		public int EacBackEndLogonProbeRecurrenceInterval { get; private set; }

		public int EacBackEndLogonProbeTimeout { get; private set; }

		public TimeSpan EacBackEndLogonMonitorRecurrenceInterval { get; private set; }

		public TimeSpan SecondaryMonitoringInterval { get; private set; }

		public TimeSpan MonitorStateTransitions { get; private set; }

		public int AvailabilityPercentage { get; private set; }

		public int DegradedTransitionSpan { get; private set; }

		public int UnrecoverableTransitionSpan { get; private set; }

		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				LocalEndpointManager instance = LocalEndpointManager.Instance;
				this.LogSuccessProbeResult = this.ReadAttribute("LogSuccessProbeResult", true);
				this.ConsecutiveFailureCount = this.ReadAttribute("ConsecutiveFailureCount", 4);
				this.ProbeMaxRetryAttempts = this.ReadAttribute("ProbeMaxRetryAttempts", 0);
				this.EacBackEndPingProbeRecurrenceInterval = (int)this.ReadAttribute("EacBackEndPingProbeRecurrenceInterval", TimeSpan.FromSeconds(60.0)).TotalSeconds;
				this.EacBackEndPingProbeTimeout = (int)this.ReadAttribute("EacBackEndPingProbeTimeout", TimeSpan.FromSeconds(30.0)).TotalSeconds;
				this.EacBackEndPingMonitorRecurrenceInterval = (int)this.ReadAttribute("EacBackEndPingMonitorRecurrenceInterval", TimeSpan.FromSeconds(0.0)).TotalSeconds;
				this.EacBackEndLogonProbeRecurrenceInterval = (int)this.ReadAttribute("EacBackEndLogonProbeRecurrenceInterval", TimeSpan.FromSeconds(300.0)).TotalSeconds;
				this.EacBackEndLogonProbeTimeout = (int)this.ReadAttribute("EacBackEndLogonProbeTimeout", TimeSpan.FromSeconds(120.0)).TotalSeconds;
				this.EacBackEndLogonMonitorRecurrenceInterval = this.ReadAttribute("EacBackEndLogonMonitorRecurrenceInterval", TimeSpan.FromSeconds(3600.0));
				this.SecondaryMonitoringInterval = this.ReadAttribute("SecondaryMonitoringInterval", TimeSpan.FromSeconds(7200.0));
				this.MonitorStateTransitions = this.ReadAttribute("MonitorStateTransitions", TimeSpan.FromSeconds(7200.0));
				this.AvailabilityPercentage = this.ReadAttribute("AvailabilityPercentage", 60);
				this.DegradedTransitionSpan = (int)this.ReadAttribute("DegradedTransitionSpan", TimeSpan.FromSeconds(0.0)).TotalSeconds;
				this.UnrecoverableTransitionSpan = (int)this.ReadAttribute("UnrecoverableTransitionSpan", TimeSpan.FromSeconds(1200.0)).TotalSeconds;
				if (instance.ExchangeServerRoleEndpoint == null || !instance.ExchangeServerRoleEndpoint.IsClientAccessRoleInstalled)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.ECPTracer, base.TraceContext, "EacDiscovery.DoWork: no client access role installed, skip EacBackEndPingProbe and EacBackEndLogonProbe", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Eac\\EacDiscovery.cs", 168);
				}
				else
				{
					this.SetupEacBackEndPingProbeContext();
					if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.ECPTracer, base.TraceContext, "EacDiscovery.DoWork: no mailbox database found on this server, skip EacBackEndLogonProbe", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Eac\\EacDiscovery.cs", 176);
					}
					else
					{
						this.SetupEacBackEndLogonProbeContext(instance);
					}
				}
			}
			catch (EndpointManagerEndpointUninitializedException)
			{
				WTFDiagnostics.TraceWarning(ExTraceGlobals.ECPTracer, base.TraceContext, "EacDiscovery.DoWork: EndpointManagerEndpointUninitializedException is caught.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Eac\\EacDiscovery.cs", 186);
			}
		}

		private void SetupEacBackEndPingProbeContext()
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.ECPTracer, base.TraceContext, "EacDiscovery.SetupEacBackEndPingProbeContext: Begin to setup context for EacBackEndPingProbe", null, "SetupEacBackEndPingProbeContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Eac\\EacDiscovery.cs", 198);
			string text = this.BuildWorkItemName("EacBackEndPing", "Probe");
			ProbeDefinition definition = this.CreateProbe(text, EacDiscovery.AssemblyPath, EacDiscovery.EacBackEndPingProbeTypeName, EacDiscovery.EacBackEndUrl, this.EacBackEndPingProbeRecurrenceInterval, this.EacBackEndPingProbeTimeout, this.ProbeMaxRetryAttempts, "EacBackEndPingProbe", null);
			base.Broker.AddWorkDefinition<ProbeDefinition>(definition, base.TraceContext);
			string name = this.BuildWorkItemName("EacBackEndPing", "Monitor");
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(name, text, ExchangeComponent.Ecp.Name, ExchangeComponent.Ecp, this.ConsecutiveFailureCount, true, 300);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, this.DegradedTransitionSpan),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, this.UnrecoverableTransitionSpan)
			};
			monitorDefinition.RecurrenceIntervalSeconds = this.EacBackEndPingMonitorRecurrenceInterval;
			monitorDefinition.IsHaImpacting = false;
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Validate EAC health is not impacted by BE connectivity issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string escalationSubjectUnhealthy = Strings.EacSelfTestEscalationSubject(Environment.MachineName);
			string escalationMessageUnhealthy = Strings.EacSelfTestEscalationBody(Environment.MachineName, Path.Combine(EacDiscovery.EacMonitoringLogFolderPath.Value, "EacBackEndPingProbe"));
			ResponderDefinition responderDefinition = ResetIISAppPoolResponder.CreateDefinition(this.BuildWorkItemName("EacBackEndPing", "ResetIISAppPool"), monitorDefinition.Name, "MSExchangeECPAppPool", ServiceHealthStatus.Degraded, DumpMode.None, null, 15.0, 0, "Exchange", true, null);
			responderDefinition.ServiceName = ExchangeComponent.Ecp.Name;
			ResponderDefinition definition2 = EscalateResponder.CreateDefinition(this.BuildWorkItemName("EacBackEndPing", "Escalate"), ExchangeComponent.Ecp.Name, monitorDefinition.Name, monitorDefinition.Name, Environment.MachineName, ServiceHealthStatus.Unrecoverable, ExchangeComponent.Ecp.EscalationTeam, escalationSubjectUnhealthy, escalationMessageUnhealthy, true, NotificationServiceClass.Scheduled, 14400, EacDiscovery.EacDailyScheduledPattern, false);
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition2, base.TraceContext);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.ECPTracer, base.TraceContext, "EacDiscovery.SetupEacBackEndPingProbeContext: Finish to setup context for EacBackEndPingProbe", null, "SetupEacBackEndPingProbeContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Eac\\EacDiscovery.cs", 273);
		}

		private void SetupEacBackEndLogonProbeContext(LocalEndpointManager endpointManager)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.ECPTracer, base.TraceContext, "EacDiscovery.SetupEacBackEndLogonProbeContext: Begin to setup context for EacBackEndLogonProbe", null, "SetupEacBackEndLogonProbeContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Eac\\EacDiscovery.cs", 285);
			string text = this.BuildWorkItemName("EacBackEndLogon", "Probe");
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in endpointManager.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend)
			{
				if (string.IsNullOrWhiteSpace(mailboxDatabaseInfo.MonitoringAccount))
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ECPTracer, base.TraceContext, "EacDiscovery.SetupEacBackEndLogonProbe: Ignore mailbox database {0} because it does not have monitoring mailbox", mailboxDatabaseInfo.MailboxDatabaseName, null, "SetupEacBackEndLogonProbeContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Eac\\EacDiscovery.cs", 298);
				}
				else
				{
					ProbeDefinition probeDefinition = this.CreateProbe(text, EacDiscovery.AssemblyPath, EacDiscovery.EacBackEndLogonProbeTypeName, EacDiscovery.EacBackEndUrl, this.EacBackEndLogonProbeRecurrenceInterval, this.EacBackEndLogonProbeTimeout, this.ProbeMaxRetryAttempts, "EacBackEndLogonProbe", mailboxDatabaseInfo);
					probeDefinition.Attributes["DatabaseGuid"] = mailboxDatabaseInfo.MailboxDatabaseGuid.ToString();
					base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
				}
			}
			string name = this.BuildWorkItemName("EacBackEndLogon", "Monitor");
			MonitorDefinition monitorDefinition = OverallPercentSuccessByStateAttribute1Monitor.CreateDefinition(name, text, ExchangeComponent.Ecp.Name, ExchangeComponent.Ecp, (double)this.AvailabilityPercentage, this.SecondaryMonitoringInterval, this.EacBackEndLogonMonitorRecurrenceInterval, this.MonitorStateTransitions, "", true);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, this.DegradedTransitionSpan),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, this.UnrecoverableTransitionSpan)
			};
			monitorDefinition.IsHaImpacting = false;
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Validate EAC health is not impacted by BE logon issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string escalationSubjectUnhealthy = Strings.EacDeepTestEscalationSubject(Environment.MachineName);
			string escalationMessageUnhealthy = Strings.EacDeepTestEscalationBody(Environment.MachineName, Path.Combine(EacDiscovery.EacMonitoringLogFolderPath.Value, "EacBackEndLogonProbe"));
			ResponderDefinition responderDefinition = ResetIISAppPoolResponder.CreateDefinition(this.BuildWorkItemName("EacBackEndLogon", "ResetIISAppPool"), monitorDefinition.Name, "MSExchangeECPAppPool", ServiceHealthStatus.Degraded, DumpMode.None, null, 15.0, 0, "Exchange", true, null);
			responderDefinition.ServiceName = ExchangeComponent.Ecp.Name;
			ResponderDefinition definition = EscalateResponder.CreateDefinition(this.BuildWorkItemName("EacBackEndLogon", "Escalate"), ExchangeComponent.Ecp.Name, monitorDefinition.Name, monitorDefinition.Name, Environment.MachineName, ServiceHealthStatus.Unrecoverable, ExchangeComponent.Ecp.EscalationTeam, escalationSubjectUnhealthy, escalationMessageUnhealthy, true, NotificationServiceClass.Scheduled, 14400, EacDiscovery.EacDailyScheduledPattern, false);
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.ECPTracer, base.TraceContext, "EacDiscovery.SetupEacBackEndLogonProbeContext: Finish to setup context for EacBackEndLogonProbe", null, "SetupEacBackEndLogonProbeContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Eac\\EacDiscovery.cs", 378);
		}

		private ProbeDefinition CreateProbe(string probeName, string assemblyPath, string probeTypeName, string endPoint, int recurrenceIntervalSeconds, int timeoutSeconds, int maxRetryAttempts, string logFileInstance = null, MailboxDatabaseInfo dbInfo = null)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.ECPTracer, base.TraceContext, string.Format("EacDiscovery.CreateProbe: Creating probe {0} of type {1}", probeName, probeTypeName), null, "CreateProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Eac\\EacDiscovery.cs", 408);
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.Name = probeName;
			probeDefinition.ServiceName = ExchangeComponent.Ecp.Name;
			probeDefinition.AssemblyPath = assemblyPath;
			probeDefinition.TypeName = probeTypeName;
			probeDefinition.Endpoint = endPoint;
			probeDefinition.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			probeDefinition.TimeoutSeconds = timeoutSeconds;
			probeDefinition.MaxRetryAttempts = maxRetryAttempts;
			probeDefinition.Attributes["SslValidationOptions"] = SslValidationOptions.NoSslValidation.ToString();
			if (!string.IsNullOrEmpty(logFileInstance))
			{
				probeDefinition.Attributes["LogFileInstanceName"] = logFileInstance;
				probeDefinition.Attributes["LogSuccessProbeResult"] = this.LogSuccessProbeResult.ToString();
			}
			if (dbInfo != null)
			{
				string monitoringDomain = dbInfo.MonitoringAccountUserPrincipalName.Substring(dbInfo.MonitoringAccountUserPrincipalName.IndexOf('@') + 1);
				probeDefinition.Account = dbInfo.MonitoringAccountUserPrincipalName;
				probeDefinition.AccountPassword = dbInfo.MonitoringAccountPassword;
				probeDefinition.AccountDisplayName = dbInfo.MonitoringAccountUserPrincipalName;
				probeDefinition.TargetResource = dbInfo.MailboxDatabaseName;
				OwaUtils.AddBackendAuthenticationParameters(probeDefinition, dbInfo.MonitoringAccountSid, monitoringDomain);
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.ECPTracer, base.TraceContext, string.Format("EacDiscovery.CreateProbe: Created probe {0} of type {1}", probeName, probeTypeName), null, "CreateProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Eac\\EacDiscovery.cs", 445);
			return probeDefinition;
		}

		private string BuildWorkItemName(string probeType, string workItemType)
		{
			return string.Format("{0}{1}", probeType, workItemType);
		}

		private const string EacBackEndPingString = "EacBackEndPing";

		private const string EacBackEndLogonString = "EacBackEndLogon";

		private const string ProbeString = "Probe";

		private const string MonitorString = "Monitor";

		private const string EscalateString = "Escalate";

		private const string ResetIISAppPoolString = "ResetIISAppPool";

		private const string EacAppPoolName = "MSExchangeECPAppPool";

		private static readonly Lazy<string> EacMonitoringLogFolderPath = new Lazy<string>(() => Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\Monitoring\\ECP\\"));

		private static readonly string EacBackEndUrl = "https://localhost:444/ecp/";

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string EacBackEndPingProbeTypeName = typeof(EacBackEndPingProbe).FullName;

		private static readonly string EacBackEndLogonProbeTypeName = typeof(EacBackEndLogonProbe).FullName;

		private static readonly string EacDailyScheduledPattern = "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday/09:00/17:00";
	}
}
