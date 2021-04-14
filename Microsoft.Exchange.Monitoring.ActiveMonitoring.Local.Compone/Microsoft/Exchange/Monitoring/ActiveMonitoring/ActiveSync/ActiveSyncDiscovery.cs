using System;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveSync.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveSync
{
	public sealed class ActiveSyncDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			try
			{
				if (instance.ExchangeServerRoleEndpoint == null)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.ActiveSyncTracer, base.TraceContext, "ActiveSyncDiscovery:: DoWork(): Could not find ExchangeServerRoleEndpoint", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\ActiveSync\\ActiveSyncDiscovery.cs", 107);
					return;
				}
			}
			catch (EndpointManagerEndpointUninitializedException ex)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.ActiveSyncTracer, base.TraceContext, string.Format("ActiveSyncDiscovery:: DoWork(): ExchangeServerRoleEndpoint initialisation failed.  Exception:{0}", ex.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\ActiveSync\\ActiveSyncDiscovery.cs", 113);
				return;
			}
			if (instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled && !VariantConfiguration.InvariantNoFlightingSnapshot.ActiveMonitoring.ActiveSyncDiscovery.Enabled)
			{
				try
				{
					if (instance.MailboxDatabaseEndpoint == null)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.ActiveSyncTracer, base.TraceContext, "ActiveSyncDiscovery:: DoWork(): Could not find MailboxDatabaseEndpoint", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\ActiveSync\\ActiveSyncDiscovery.cs", 125);
						return;
					}
				}
				catch (EndpointManagerEndpointUninitializedException ex2)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.ActiveSyncTracer, base.TraceContext, string.Format("ActiveSyncDiscovery:: DoWork(): MailboxDatabaseEndpoint initialisation failed.  Exception:{0}", ex2.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\ActiveSync\\ActiveSyncDiscovery.cs", 131);
					return;
				}
				WTFDiagnostics.TraceInformation(ExTraceGlobals.ActiveSyncTracer, base.TraceContext, "ActiveSyncDiscovery:: DoWork(): Creating Cafe Service Probe", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\ActiveSync\\ActiveSyncDiscovery.cs", 135);
				this.CreateServiceProbe(false, base.TraceContext, "https://localhost/Microsoft-Server-ActiveSync");
				this.CreateServiceMonitor(false, base.TraceContext);
			}
			if (instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				try
				{
					if (instance.MailboxDatabaseEndpoint == null)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.ActiveSyncTracer, base.TraceContext, "ActiveSyncDiscovery:: DoWork(): Could not find MailboxDatabaseEndpoint", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\ActiveSync\\ActiveSyncDiscovery.cs", 159);
						return;
					}
				}
				catch (Exception ex3)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.ActiveSyncTracer, base.TraceContext, string.Format("ActiveSyncDiscovery:: DoWork(): MailboxDatabaseEndpoint object threw exception.  Exception:{0}", ex3.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\ActiveSync\\ActiveSyncDiscovery.cs", 165);
					return;
				}
				WTFDiagnostics.TraceInformation(ExTraceGlobals.ActiveSyncTracer, base.TraceContext, "ActiveSyncDiscovery:: DoWork(): Found Mailbox Role", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\ActiveSync\\ActiveSyncDiscovery.cs", 169);
				if (!VariantConfiguration.InvariantNoFlightingSnapshot.ActiveMonitoring.ActiveSyncDiscovery.Enabled)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.ActiveSyncTracer, base.TraceContext, "ActiveSyncDiscovery:: DoWork(): Creating Mailbox Service Probe", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\ActiveSync\\ActiveSyncDiscovery.cs", 174);
					this.CreateServiceProbe(true, base.TraceContext, "https://localhost:444/Microsoft-Server-ActiveSync/Proxy");
					if (ActiveSyncDiscovery.wasBeProbeCreated)
					{
						this.CreateServiceMonitor(true, base.TraceContext);
					}
				}
				this.CreateMbxLightModeProbe(base.TraceContext);
			}
			if (ActiveSyncDiscovery.wasCafeProbeCreated || ActiveSyncDiscovery.wasBeProbeCreated)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.ActiveSyncTracer, base.TraceContext, "ActiveSyncDiscovery:: DoWork(): Creating Perf Counters", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\ActiveSync\\ActiveSyncDiscovery.cs", 201);
				string perfCounterName = "MSExchange Throttling\\Task queue length\\eas";
				MonitorDefinition monitorDefinition = OverallConsecutiveSampleValueAboveThresholdMonitor.CreateDefinition("RequestsQueuedGt500Monitor", PerformanceCounterNotificationItem.GenerateResultName(perfCounterName), ExchangeComponent.ActiveSync.Name, ExchangeComponent.ActiveSync, 500.0, 2, true);
				monitorDefinition.ServicePriority = 0;
				monitorDefinition.ScenarioDescription = "Validate ActiveSync health is not impacted by excessive queue length";
				monitorDefinition.TargetResource = "MSExchangeSyncAppPool";
				base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
				ResponderDefinition definition = EscalateResponder.CreateDefinition("RequestsQueuedGt500Escalate", ExchangeComponent.ActiveSync.Name, monitorDefinition.Name, monitorDefinition.Name, "MSExchangeSyncAppPool", ServiceHealthStatus.None, "Pop3, Imap4, ActiveSync", Strings.EscalationSubjectUnhealthy, Strings.EscalationMessageFailuresUnhealthy(Strings.RequestsQueuedOver500EscalationMessage), true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
				base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
			}
		}

		private MonitorStateTransition[] CreateResponderChain(bool mbxProbe, bool lightMode, bool isPingProbe, string monitorName, string targetResource)
		{
			bool enabled = VariantConfiguration.InvariantNoFlightingSnapshot.ActiveMonitoring.ActiveSyncDiscovery.Enabled;
			string name = "DefaultActiveSyncFailsafeEscalate";
			string escalationSubjectUnhealthy = string.Format(Strings.EscalationSubjectUnhealthy, ExchangeComponent.ActiveSync.Name, ExchangeComponent.ActiveSync.Name);
			string text = Strings.GenericOverallXFailureEscalationMessage(ExchangeComponent.ActiveSync.Name);
			MonitorStateTransition[] result;
			string responderName;
			string name2;
			if (lightMode)
			{
				result = new MonitorStateTransition[]
				{
					new MonitorStateTransition(ServiceHealthStatus.Degraded1, 0),
					new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)TimeSpan.FromMinutes(20.0).TotalSeconds)
				};
				string probeName = string.Format("ActiveSyncSelfTest{0}", "Probe");
				responderName = string.Format("ActiveSyncSelfTest{0}", "RestartWebAppPool");
				name2 = string.Format("ActiveSyncSelfTest{0}", "Escalate");
				escalationSubjectUnhealthy = Strings.ActiveSyncEscalationSubject(string.Format("ActiveSyncSelfTest{0}", "Probe"), Environment.MachineName).ToString();
				text = (enabled ? Strings.ActiveSyncSelfTestEscalationBodyDC(Environment.MachineName, probeName).ToString() : Strings.ActiveSyncSelfTestEscalationBodyENT(Environment.MachineName, probeName).ToString());
				if (VariantConfiguration.InvariantNoFlightingSnapshot.ActiveMonitoring.ActiveSyncDiscovery.Enabled)
				{
					text += string.Format("<br><br><a href='{0}'>Battlecards</a><br><a href='{1}'>OneNote Battlecards</a>", "http://aka.ms/decisiontrees?Page=RecoveryHome&service=Exchange&escalationteam=Pop3,%20Imap4,%20ActiveSync&alerttypeid=ActiveSyncSelfTestMonitor&id=0&alertname=dummy", "onenote:///\\\\exstore\\files\\userfiles\\servicesoncall\\OnCall%20Battlecard\\New%20Battlecards.one#EAS/Pop/Imap%20Synthetic%20Self,%20Deep%20and%20Proxy%20Test%20Issues&section-id={9E24DEE5-34D7-4463-AB20-386D859233BA}&page-id={E9CB5DFA-5F3B-4A87-B8D6-42F25F981A8E}&end");
				}
			}
			else if (mbxProbe)
			{
				result = new MonitorStateTransition[]
				{
					new MonitorStateTransition(ServiceHealthStatus.Degraded1, 0),
					new MonitorStateTransition(ServiceHealthStatus.Unrecoverable1, (int)TimeSpan.FromMinutes(60.0).TotalSeconds)
				};
				string probeName = string.Format("ActiveSyncDeepTest{0}", "Probe");
				responderName = string.Format("ActiveSyncDeepTest{0}", "RestartWebAppPool");
				name2 = string.Format("ActiveSyncDeepTest{0}", "Escalate");
				name = string.Format("ActiveSyncDeepTest{0}", "FailsafeEscalate");
				escalationSubjectUnhealthy = Strings.ActiveSyncEscalationSubject(string.Format("ActiveSyncDeepTest{0}", "Probe"), Environment.MachineName).ToString();
				text = (enabled ? Strings.ActiveSyncDeepTestEscalationBodyDC(Environment.MachineName, probeName).ToString() : Strings.ActiveSyncDeepTestEscalationBodyENT(Environment.MachineName, probeName).ToString());
				if (VariantConfiguration.InvariantNoFlightingSnapshot.ActiveMonitoring.ActiveSyncDiscovery.Enabled)
				{
					text += string.Format("<br><br><a href='{0}'>Battlecards</a><br><a href='{1}'>OneNote Battlecards</a>", "http://aka.ms/decisiontrees?Page=RecoveryHome&service=Exchange&escalationteam=Pop3,%20Imap4,%20ActiveSync&alerttypeid=ActiveSyncSelfTestMonitor&id=0&alertname=dummy", "onenote:///\\\\exstore\\files\\userfiles\\servicesoncall\\OnCall%20Battlecard\\New%20Battlecards.one#EAS/Pop/Imap%20Synthetic%20Self,%20Deep%20and%20Proxy%20Test%20Issues&section-id={9E24DEE5-34D7-4463-AB20-386D859233BA}&page-id={E9CB5DFA-5F3B-4A87-B8D6-42F25F981A8E}&end");
				}
			}
			else
			{
				result = new MonitorStateTransition[]
				{
					new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
					new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)TimeSpan.FromMinutes(30.0).TotalSeconds)
				};
				string probeName = string.Format("ActiveSyncCTP{0}", "Probe");
				responderName = string.Format("ActiveSyncCTP{0}", "RestartWebAppPool");
				name2 = string.Format("ActiveSyncCTP{0}", "Escalate");
				escalationSubjectUnhealthy = Strings.ActiveSyncEscalationSubject(string.Format("ActiveSyncCTP{0}", "Probe"), Environment.MachineName).ToString();
				text = (enabled ? Strings.ActiveSyncCustomerTouchPointEscalationBodyDC(Environment.MachineName, probeName).ToString() : Strings.ActiveSyncCustomerTouchPointEscalationBodyENT(Environment.MachineName, probeName).ToString());
			}
			if (mbxProbe)
			{
				ResponderDefinition responderDefinition = ResetIISAppPoolResponder.CreateDefinition(responderName, monitorName, "MSExchangeSyncAppPool", ServiceHealthStatus.Degraded1, DumpMode.FullDump, null, 25.0, 90, "Exchange", true, "Dag");
				responderDefinition.ServiceName = ExchangeComponent.ActiveSyncProtocol.Name;
				responderDefinition.TargetResource = targetResource;
				responderDefinition.RecurrenceIntervalSeconds = 30;
				responderDefinition.TimeoutSeconds = 25;
				if (!lightMode)
				{
					responderDefinition.ActionOnCorrelatedMonitors = CorrelatedMonitorAction.GenerateException;
					responderDefinition.CorrelatedMonitors = new CorrelatedMonitorInfo[]
					{
						StoreMonitoringHelpers.GetStoreCorrelation(null, ActiveSyncProbeUtil.ReturnComponentExceptions(ActiveSyncProbeUtil.EasFailingComponent.Mailbox))
					};
				}
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			}
			ResponderDefinition responderDefinition2 = EscalateResponder.CreateDefinition(name2, mbxProbe ? ExchangeComponent.ActiveSyncProtocol.Name : ExchangeComponent.ActiveSync.Name, monitorName, monitorName, targetResource, ServiceHealthStatus.Unrecoverable, "Pop3, Imap4, ActiveSync", escalationSubjectUnhealthy, text, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			if (mbxProbe && !lightMode)
			{
				responderDefinition2.ActionOnCorrelatedMonitors = CorrelatedMonitorAction.GenerateException;
				responderDefinition2.CorrelatedMonitors = new CorrelatedMonitorInfo[]
				{
					StoreMonitoringHelpers.GetStoreCorrelation(null, ActiveSyncProbeUtil.ReturnComponentExceptions(ActiveSyncProbeUtil.EasFailingComponent.Mailbox))
				};
				responderDefinition2.Enabled = false;
			}
			responderDefinition2.RecurrenceIntervalSeconds = 30;
			responderDefinition2.TimeoutSeconds = 25;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
			if (mbxProbe && !isPingProbe)
			{
				ResponderDefinition responderDefinition3 = EscalateResponder.CreateDefinition(name, mbxProbe ? ExchangeComponent.ActiveSyncProtocol.Name : ExchangeComponent.ActiveSync.Name, monitorName, monitorName, targetResource, ServiceHealthStatus.Unrecoverable1, "Pop3, Imap4, ActiveSync", escalationSubjectUnhealthy, text, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
				responderDefinition3.RecurrenceIntervalSeconds = 30;
				responderDefinition3.TimeoutSeconds = 25;
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition3, base.TraceContext);
				responderDefinition2.Enabled = false;
			}
			return result;
		}

		private void CreateMbxLightModeProbe(TracingContext traceContext)
		{
			ProbeDefinition probeDefinition = ActiveSyncAppPoolPingProbe.CreateDefinition(ActiveSyncDiscovery.AssemblyPath);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(string.Format("ActiveSyncSelfTest{0}", "Monitor"), probeDefinition.Name, ExchangeComponent.ActiveSyncProtocol.Name, ExchangeComponent.ActiveSyncProtocol, 4, true, 240);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.TargetResource = "MSExchangeSyncAppPool";
			monitorDefinition.IsHaImpacting = true;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate ActiveSync health on BE are not impacted any issues";
			monitorDefinition.MonitorStateTransitions = this.CreateResponderChain(true, true, false, monitorDefinition.Name, "MSExchangeSyncAppPool");
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ActiveSyncDiscovery.wasBeProbeCreated = true;
		}

		private void CreateServiceProbe(bool isMbxProbe, TracingContext traceContext, string endPoint)
		{
			int recurrence = 180;
			int recurrence2 = 240;
			if (isMbxProbe)
			{
				ProbeDefinition definition = ActiveSyncMailboxDeepProbe.CreateDefinition(ActiveSyncDiscovery.AssemblyPath, endPoint, recurrence);
				base.Broker.AddWorkDefinition<ProbeDefinition>(definition, base.TraceContext);
			}
			else
			{
				ProbeDefinition definition = ActiveSyncCustomerTouchPointProbe.CreateDefinition(ActiveSyncDiscovery.AssemblyPath, endPoint, recurrence2);
				base.Broker.AddWorkDefinition<ProbeDefinition>(definition, base.TraceContext);
			}
			if (isMbxProbe)
			{
				ActiveSyncDiscovery.wasBeProbeCreated = true;
				return;
			}
			ActiveSyncDiscovery.wasCafeProbeCreated = true;
		}

		private void CreateServiceMonitor(bool isMbxProbe, TracingContext traceContext)
		{
			string name;
			string sampleMask;
			if (isMbxProbe)
			{
				name = string.Format("ActiveSyncDeepTest{0}", "Monitor");
				sampleMask = string.Format("ActiveSyncDeepTest{0}", "Probe");
			}
			else
			{
				name = string.Format("ActiveSyncCTP{0}", "Monitor");
				sampleMask = string.Format("ActiveSyncCTP{0}", "Probe");
			}
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(name, sampleMask, isMbxProbe ? ExchangeComponent.ActiveSyncProtocol.Name : ExchangeComponent.ActiveSync.Name, isMbxProbe ? ExchangeComponent.ActiveSyncProtocol : ExchangeComponent.ActiveSync, 5, true, isMbxProbe ? 900 : 1200);
			monitorDefinition.TargetResource = ExchangeComponent.ActiveSync.Name;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate ActiveSync health is not impacted by any issues";
			if (isMbxProbe)
			{
				monitorDefinition.IsHaImpacting = true;
			}
			monitorDefinition.MonitorStateTransitions = this.CreateResponderChain(isMbxProbe, false, false, monitorDefinition.Name, ExchangeComponent.ActiveSync.Name);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
		}

		private const string ServiceResource = "MSExchangeSyncAppPool";

		private const int MaxRetryAttempt = 3;

		private const string ActiveSyncMDT = "ActiveSyncDeepTest{0}";

		private const string ActiveSyncPST = "ActiveSyncSelfTest{0}";

		private const string ActiveSyncCTP = "ActiveSyncCTP{0}";

		private const string Probe = "Probe";

		private const string Monitor = "Monitor";

		private const string BattleCardPageUrl = "http://aka.ms/decisiontrees?Page=RecoveryHome&service=Exchange&escalationteam=Pop3,%20Imap4,%20ActiveSync&alerttypeid=ActiveSyncSelfTestMonitor&id=0&alertname=dummy";

		private const string OneNoteBattleCardUrl = "onenote:///\\\\exstore\\files\\userfiles\\servicesoncall\\OnCall%20Battlecard\\New%20Battlecards.one#EAS/Pop/Imap%20Synthetic%20Self,%20Deep%20and%20Proxy%20Test%20Issues&section-id={9E24DEE5-34D7-4463-AB20-386D859233BA}&page-id={E9CB5DFA-5F3B-4A87-B8D6-42F25F981A8E}&end";

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static bool wasCafeProbeCreated = false;

		private static bool wasBeProbeCreated = false;
	}
}
