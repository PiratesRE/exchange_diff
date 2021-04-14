using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Rws.Probes;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rws
{
	internal sealed class RwsSyntheticTenantMailboxUsageDetailDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsSyntheticTenantMailboxUsageDetailDiscovery.DoWork: enter", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsSyntheticTenantMailboxUsageDetailDiscovery.cs", 93);
			this.ProbeName = string.Format("RwsSyntheticTenantMailboxUsageDetail{0}Probe", base.Definition.Attributes["TargetGroup"]);
			this.MonitorName = string.Format("RwsSyntheticTenantMailboxUsageDetail{0}Monitor", base.Definition.Attributes["TargetGroup"]);
			this.ResponderName = string.Format("RwsSyntheticTenantMailboxUsageDetail{0}EscalateResponder", base.Definition.Attributes["TargetGroup"]);
			if (!this.ShouldDoDeployment())
			{
				return;
			}
			this.CreateProbeDefinitions();
			this.CreateMonitorDefinitions();
			this.CreateResponderDefinitions();
		}

		private bool ShouldDoDeployment()
		{
			if (!LocalEndpointManager.IsDataCenter)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, "Skip RWS monitoring on non datacenter environment", null, "ShouldDoDeployment", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsSyntheticTenantMailboxUsageDetailDiscovery.cs", 121);
				return false;
			}
			if (Datacenter.IsGallatinDatacenter())
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, "Skip RWS monitoring on Gallatin datacenter environment", null, "ShouldDoDeployment", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsSyntheticTenantMailboxUsageDetailDiscovery.cs", 131);
				return false;
			}
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Datamining\\Monitoring"))
			{
				if (registryKey != null && (int)registryKey.GetValue("ActiveMonitoringEnabled", 0) != 0)
				{
					if (ExEnvironment.IsTest)
					{
						return true;
					}
					if (ExEnvironment.IsSdfDomain && Regex.IsMatch(Environment.MachineName.Trim(), "^[a-zA-Z0-9]{3}SM01MS001$", RegexOptions.IgnoreCase))
					{
						return true;
					}
					if (Regex.IsMatch(Environment.MachineName.Trim(), "^[a-zA-Z0-9]{3}MG01MS101$", RegexOptions.IgnoreCase))
					{
						return true;
					}
				}
				else
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, "Skip RWS monitoring on the server which doesn't have AM enabled", null, "ShouldDoDeployment", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsSyntheticTenantMailboxUsageDetailDiscovery.cs", 163);
				}
			}
			return false;
		}

		private void CreateProbeDefinitions()
		{
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsSyntheticTenantMailboxUsageDetailDiscovery.CreateProbe: Creating probe {0}", this.ProbeName, null, "CreateProbeDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsSyntheticTenantMailboxUsageDetailDiscovery.cs", 178);
			int recurrenceIntervalSeconds = 600;
			int timeoutSeconds = 510;
			int maxRetryAttempts = 3;
			string endpoint = string.Empty;
			if (ExEnvironment.IsTest)
			{
				endpoint = base.Definition.Attributes["TdsConnString"];
			}
			else if (ExEnvironment.IsSdfDomain)
			{
				endpoint = base.Definition.Attributes["SdfConnString"];
			}
			else
			{
				endpoint = base.Definition.Attributes["PrdConnString"];
			}
			ProbeDefinition probeDefinition = new ProbeDefinition
			{
				AssemblyPath = RwsSyntheticTenantMailboxUsageDetailDiscovery.assemblyPath,
				TypeName = RwsSyntheticTenantMailboxUsageDetailDiscovery.probeTypeName,
				Name = this.ProbeName,
				ServiceName = base.Definition.ServiceName,
				RecurrenceIntervalSeconds = recurrenceIntervalSeconds,
				TimeoutSeconds = timeoutSeconds,
				MaxRetryAttempts = maxRetryAttempts,
				Enabled = true,
				WorkItemVersion = RwsSyntheticTenantMailboxUsageDetailDiscovery.assemblyVersion,
				ExecutionLocation = string.Empty,
				Endpoint = endpoint,
				Account = base.Definition.Attributes["AzureAccount"],
				AccountPassword = base.Definition.Attributes["AzureAccountSecurePassword"]
			};
			probeDefinition.TargetGroup = base.Definition.Attributes["TargetGroup"];
			probeDefinition.TargetResource = base.Definition.Attributes["TargetResource"];
			int num = 1;
			if (base.Definition.Attributes.ContainsKey("DuplicateCopyNumber") && !int.TryParse(base.Definition.Attributes["DuplicateCopyNumber"], out num))
			{
				num = 1;
			}
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext).Wait();
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsSyntheticTenantMailboxUsageDetailDiscovery.CreateProbe: Created probe {0}", this.ProbeName, null, "CreateProbeDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsSyntheticTenantMailboxUsageDetailDiscovery.cs", 237);
		}

		private void CreateMonitorDefinitions()
		{
			string monitorName = this.MonitorName;
			string probeName = this.ProbeName;
			int failureCount = 1;
			int monitoringInterval = 600;
			int recurrenceIntervalSeconds = 0;
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(monitorName, probeName, ExchangeComponent.Rws.Service, ExchangeComponent.Rws, failureCount, true, monitoringInterval);
			monitorDefinition.TargetResource = Environment.MachineName;
			monitorDefinition.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext).Wait();
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsSyntheticTenantMailboxUsageDetailDiscovery.CreateMonitor: Creating monitor {0} of type {1}", monitorName, RwsSyntheticTenantMailboxUsageDetailDiscovery.OverallConsecutiveProbeFailuresMonitorTypeName, null, "CreateMonitorDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsSyntheticTenantMailboxUsageDetailDiscovery.cs", 272);
		}

		private void CreateResponderDefinitions()
		{
			string responderName = this.ResponderName;
			string alertTypeId = string.Format("Exchange/Rws/{0}/Responder", this.ProbeName);
			string escalationSubjectUnhealthy = string.Format("Synthetic Tenant data is not valid in CFR MailboxUsageDetail table. Detect Machine = {0}", Environment.MachineName);
			string escalationMessageUnhealthy = "Synthetic tenant data availability/completeness issue for MailboxUsageDetail table detected";
			ResponderDefinition definition = EscalateResponder.CreateDefinition(responderName, ExchangeComponent.Rws.Service, alertTypeId, this.MonitorName, Environment.MachineName, ServiceHealthStatus.None, ExchangeComponent.Rws.EscalationTeam, escalationSubjectUnhealthy, escalationMessageUnhealthy, true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext).Wait();
			WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, string.Format("RwsSyntheticTenantMailboxUsageDetailDiscovery.CreateResponder: Created responder {0} of type {1}", responderName, RwsSyntheticTenantMailboxUsageDetailDiscovery.responderTypeName), null, "CreateResponderDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsSyntheticTenantMailboxUsageDetailDiscovery.cs", 310);
		}

		private const int DefaultRecurrenceIntervalSeconds = 600;

		private const int DefaultTimeoutSeconds = 300;

		private const int DefaultMaxRetryAttempts = 3;

		private static string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

		private static string assemblyPath = Assembly.GetExecutingAssembly().Location;

		private static string probeTypeName = typeof(RwsSyntheticTenantMailboxUsageDetailProbe).FullName;

		private static string responderTypeName = typeof(EscalateResponder).FullName;

		private static readonly string OverallConsecutiveProbeFailuresMonitorTypeName = typeof(OverallConsecutiveProbeFailuresMonitor).FullName;

		private string ProbeName;

		private string MonitorName;

		private string ResponderName;
	}
}
