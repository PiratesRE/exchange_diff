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
	internal sealed class RwsSyntheticTenantMailboxUsageDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsSyntheticTenantMailboxUsageDiscovery.DoWork: enter", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsSyntheticTenantMailboxUsageDiscovery.cs", 68);
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
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, "Skip RWS monitoring on non datacenter environment", null, "ShouldDoDeployment", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsSyntheticTenantMailboxUsageDiscovery.cs", 91);
				return false;
			}
			if (Datacenter.IsGallatinDatacenter())
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, "Skip RWS monitoring on Gallatin datacenter environment", null, "ShouldDoDeployment", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsSyntheticTenantMailboxUsageDiscovery.cs", 101);
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
					WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, "Skip RWS monitoring on the server which doesn't have AM enabled", null, "ShouldDoDeployment", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsSyntheticTenantMailboxUsageDiscovery.cs", 133);
				}
			}
			return false;
		}

		private void CreateProbeDefinitions()
		{
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsSyntheticTenantMailboxUsageDiscovery.CreateProbe: Creating probe {0}", "RwsSyntheticTenantMailboxUsageProbe", null, "CreateProbeDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsSyntheticTenantMailboxUsageDiscovery.cs", 148);
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
				AssemblyPath = RwsSyntheticTenantMailboxUsageDiscovery.assemblyPath,
				TypeName = RwsSyntheticTenantMailboxUsageDiscovery.probeTypeName,
				Name = "RwsSyntheticTenantMailboxUsageProbe",
				ServiceName = base.Definition.ServiceName,
				RecurrenceIntervalSeconds = recurrenceIntervalSeconds,
				TimeoutSeconds = timeoutSeconds,
				MaxRetryAttempts = maxRetryAttempts,
				Enabled = true,
				WorkItemVersion = RwsSyntheticTenantMailboxUsageDiscovery.assemblyVersion,
				ExecutionLocation = string.Empty,
				Endpoint = endpoint,
				Account = base.Definition.Attributes["AzureAccount"],
				AccountPassword = base.Definition.Attributes["AzureAccountSecurePassword"]
			};
			probeDefinition.TargetResource = base.Definition.Attributes["TargetResource"];
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext).Wait();
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsSyntheticTenantMailboxUsageDiscovery.CreateProbe: Created probe {0}", "RwsSyntheticTenantMailboxUsageProbe", null, "CreateProbeDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsSyntheticTenantMailboxUsageDiscovery.cs", 197);
		}

		private void CreateMonitorDefinitions()
		{
			string text = "RwsSyntheticTenantMailboxUsageMonitor";
			string sampleMask = "RwsSyntheticTenantMailboxUsageProbe";
			int failureCount = 3;
			int monitoringInterval = 2000;
			int recurrenceIntervalSeconds = 0;
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(text, sampleMask, ExchangeComponent.Rws.Service, ExchangeComponent.Rws, failureCount, true, monitoringInterval);
			monitorDefinition.TargetResource = Environment.MachineName;
			monitorDefinition.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext).Wait();
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.RWSTracer, base.TraceContext, "RwsSyntheticTenantMailboxUsageDiscovery.CreateMonitor: Creating monitor {0} of type {1}", text, RwsSyntheticTenantMailboxUsageDiscovery.OverallConsecutiveProbeFailuresMonitorTypeName, null, "CreateMonitorDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsSyntheticTenantMailboxUsageDiscovery.cs", 232);
		}

		private void CreateResponderDefinitions()
		{
			string text = "RwsSyntheticTenantMailboxUsageEscalateResponder";
			string alertTypeId = "Exchange/Rws/RwsSyntheticTenantMailboxUsageProbe/Responder";
			string escalationSubjectUnhealthy = string.Format("Synthetic Tenant data is not valid in CFR Mailboxusage table. Detect Machine = {0}", Environment.MachineName);
			string escalationMessageUnhealthy = "Synthetic tenant data availability/completeness issue for MailboxUsage table detected";
			ResponderDefinition definition = EscalateResponder.CreateDefinition(text, ExchangeComponent.Rws.Service, alertTypeId, "RwsSyntheticTenantMailboxUsageMonitor", Environment.MachineName, ServiceHealthStatus.None, ExchangeComponent.Rws.EscalationTeam, escalationSubjectUnhealthy, escalationMessageUnhealthy, true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext).Wait();
			WTFDiagnostics.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, string.Format("RwsSyntheticTenantMailboxUsageDiscovery.CreateResponder: Created responder {0} of type {1}", text, RwsSyntheticTenantMailboxUsageDiscovery.responderTypeName), null, "CreateResponderDefinitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Rws\\RwsSyntheticTenantMailboxUsageDiscovery.cs", 268);
		}

		private const string ProbeName = "RwsSyntheticTenantMailboxUsageProbe";

		private static string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

		private static string assemblyPath = Assembly.GetExecutingAssembly().Location;

		private static string probeTypeName = typeof(RwsSyntheticTenantMailboxUsageProbe).FullName;

		private static string responderTypeName = typeof(EscalateResponder).FullName;

		private static readonly string OverallConsecutiveProbeFailuresMonitorTypeName = typeof(OverallConsecutiveProbeFailuresMonitor).FullName;
	}
}
