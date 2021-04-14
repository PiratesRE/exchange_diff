using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Owa.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Net.MonitoringWebClient;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Owa
{
	public sealed class OwaDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			try
			{
				if (instance.ExchangeServerRoleEndpoint == null)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery:: DoWork(): Could not find ExchangeServerRoleEndpoint", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 198);
					return;
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, string.Format("OwaDiscovery:: DoWork(): ExchangeServerRoleEndpoint object threw exception.  Exception:{0}", ex.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 204);
				return;
			}
			try
			{
				if (instance.MailboxDatabaseEndpoint == null)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery:: DoWork(): Could not find MailboxDatabaseEndpoint", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 214);
					return;
				}
			}
			catch (Exception ex2)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, string.Format("OwaDiscovery:: DoWork(): MailboxDatabaseEndpoint object threw exception.  Exception:{0}", ex2.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 220);
				return;
			}
			this.SetupBrickProtocolProbes(instance);
			this.SetupBrickMailboxProbes(instance);
			this.SetupCafeMailboxProbes(instance);
		}

		private void SetupBrickProtocolProbes(LocalEndpointManager endpointManager)
		{
			if (!endpointManager.ExchangeServerRoleEndpoint.IsClientAccessRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery.SetupBrickProtocolProbes: OWA wasn't found on this server", null, "SetupBrickProtocolProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 237);
				return;
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery.SetupBrickProtocolProbes: Creating OWA protocol probeDefinition for this server", null, "SetupBrickProtocolProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 245);
			this.CreateOwaProtocolProbe();
			WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery.SetupBrickProtocolProbes: Created OWA protocol probeDefinition for this server", null, "SetupBrickProtocolProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 253);
		}

		private void SetupBrickMailboxProbes(LocalEndpointManager endpointManager)
		{
			if (!endpointManager.ExchangeServerRoleEndpoint.IsClientAccessRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery.SetupBrickMailboxProbes: OWA wasn't found on this server", null, "SetupBrickMailboxProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 267);
				return;
			}
			if (endpointManager.MailboxDatabaseEndpoint == null || endpointManager.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery.SetupBrickMailboxProbes: no mailbox database found on this server", null, "SetupBrickMailboxProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 278);
				return;
			}
			ICollection<MailboxDatabaseInfo> mailboxDatabaseInfoCollectionForBackend = endpointManager.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend;
			TimeSpan probeFrequency = TimeSpan.FromSeconds((double)(30 * mailboxDatabaseInfoCollectionForBackend.Count));
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in mailboxDatabaseInfoCollectionForBackend)
			{
				if (string.IsNullOrWhiteSpace(mailboxDatabaseInfo.MonitoringAccount))
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery.SetupBrickMailboxProbes: Ignore mailbox database {0} because it does not have monitoring mailbox", mailboxDatabaseInfo.MailboxDatabaseName, null, "SetupBrickMailboxProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 304);
				}
				else
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery.SetupBrickMailboxProbes: Creating OWA probeDefinition for mailbox database {0}", mailboxDatabaseInfo.MailboxDatabaseName, null, "SetupBrickMailboxProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 312);
					this.CreateOwaMailboxProbe(mailboxDatabaseInfo, probeFrequency);
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery.SetupBrickMailboxProbes: Created OWA Brick probeDefinition for mailbox database {0}", mailboxDatabaseInfo.MailboxDatabaseName, null, "SetupBrickMailboxProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 323);
				}
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery.SetupBrickMailboxProbes: Creating OWA monitor for mailbox probes", null, "SetupBrickMailboxProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 330);
			this.CreateOwaMailboxMonitor();
			WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery.SetupBrickMailboxProbes: Created OWA monitor for mailbox probes", null, "SetupBrickMailboxProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 337);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery.SetupBrickMailboxProbes: Creating OWA responders for mailbox probes", null, "SetupBrickMailboxProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 342);
			this.CreateOwaMailboxResponders();
			WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery.SetupBrickMailboxProbes: Created OWA responders for mailbox probes", null, "SetupBrickMailboxProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 349);
		}

		private void SetupCafeMailboxProbes(LocalEndpointManager endpointManager)
		{
			if (LocalEndpointManager.IsDataCenter || LocalEndpointManager.IsDataCenterDedicated)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery.SetupCafeMailboxProbes: CTP probes are not supported in datacenter", null, "SetupCafeMailboxProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 363);
				return;
			}
			if (!endpointManager.ExchangeServerRoleEndpoint.IsCafeRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery.SetupCafeMailboxProbes: cafe role not found on this server", null, "SetupCafeMailboxProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 373);
				return;
			}
			if (endpointManager.MailboxDatabaseEndpoint == null || endpointManager.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe.Count == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery.SetupCafeMailboxProbes: no mailbox database found on this server", null, "SetupCafeMailboxProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 384);
				return;
			}
			if (this.CheckIfADFSAuthenticationIsEnabledOnServer())
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery.SetupCafeMailboxProbes: CTP probes are not supported when ADFS authentication is configured ", null, "SetupCafeMailboxProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 394);
				return;
			}
			ICollection<MailboxDatabaseInfo> mailboxDatabaseInfoCollectionForCafe = endpointManager.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe;
			TimeSpan probeFrequency = TimeSpan.FromSeconds((double)(60 * mailboxDatabaseInfoCollectionForCafe.Count));
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in mailboxDatabaseInfoCollectionForCafe)
			{
				if (string.IsNullOrWhiteSpace(mailboxDatabaseInfo.MonitoringAccount))
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery.SetupCafeMailboxProbes: Ignore mailbox database {0} because it does not have monitoring mailbox", mailboxDatabaseInfo.MailboxDatabaseName, null, "SetupCafeMailboxProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 420);
				}
				else
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery.SetupCafeMailboxProbes: Creating OWA probeDefinition for mailbox database {0}", mailboxDatabaseInfo.MailboxDatabaseName, null, "SetupCafeMailboxProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 428);
					this.CreateOwaClientAccessProbe(mailboxDatabaseInfo, "https://localhost/owa/", probeFrequency);
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery.SetupCafeMailboxProbes: Created OWA Cafe probeDefinition for mailbox database {0}", mailboxDatabaseInfo.MailboxDatabaseName, null, "SetupCafeMailboxProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 439);
				}
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery.SetupCafeMailboxProbes: Creating OWA monitors for Cafe probeDefinition", null, "SetupCafeMailboxProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 446);
			this.CreateOwaClientAccessMonitor();
			WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery.SetupCafeMailboxProbes: Created OWA Cafe monitors", null, "SetupCafeMailboxProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 453);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery.SetupCafeMailboxProbes: Creating OWA responders for Cafe probeDefinition", null, "SetupCafeMailboxProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 458);
			this.CreateOwaClientAccessResponders();
			WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "OwaDiscovery.SetupCafeMailboxProbes: Created OWA Cafe responders", null, "SetupCafeMailboxProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaDiscovery.cs", 465);
		}

		private void CreateOwaMailboxProbe(MailboxDatabaseInfo dbInfo, TimeSpan probeFrequency)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			OwaDiscovery.PopulateOwaMailboxProbeDefinition(probeDefinition, dbInfo, probeFrequency);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
		}

		internal static void PopulateOwaMailboxProbeDefinition(ProbeDefinition probeDefinition, MailboxDatabaseInfo dbInfo, TimeSpan probeFrequency)
		{
			string name = OwaDiscovery.BuildWorkItemName(OwaDiscovery.OwaProtocolString, OwaDiscovery.OwaDeepTestString, OwaDiscovery.ProbeString);
			string endpoint = "https://localhost:444/owa/";
			Component owaProtocol = ExchangeComponent.OwaProtocol;
			probeDefinition.AssemblyPath = OwaDiscovery.AssemblyPath;
			probeDefinition.TypeName = OwaDiscovery.OwaMailboxProbeTypeName;
			probeDefinition.Name = name;
			probeDefinition.ServiceName = owaProtocol.Name;
			probeDefinition.RecurrenceIntervalSeconds = (int)probeFrequency.TotalSeconds;
			probeDefinition.TimeoutSeconds = 300;
			probeDefinition.MaxRetryAttempts = 0;
			probeDefinition.Account = dbInfo.MonitoringAccount + "@" + dbInfo.MonitoringAccountDomain;
			probeDefinition.AccountPassword = dbInfo.MonitoringAccountPassword;
			probeDefinition.AccountDisplayName = probeDefinition.Account;
			probeDefinition.Endpoint = endpoint;
			probeDefinition.TargetResource = dbInfo.MailboxDatabaseName;
			if (LocalEndpointManager.IsDataCenter || LocalEndpointManager.IsDataCenterDedicated || OwaDiscovery.IsDogfood)
			{
				probeDefinition.AccountDisplayName = probeDefinition.AccountDisplayName + " - Pwd: " + dbInfo.MonitoringAccountPassword;
			}
			probeDefinition.Attributes["SslValidationOptions"] = SslValidationOptions.NoSslValidation.ToString();
			probeDefinition.Attributes["LogFileInstanceName"] = "MailboxProbe";
			probeDefinition.Attributes["AcceptPassiveCopyErrorAsSuccess"] = "True";
			probeDefinition.Attributes["DatabaseGuid"] = dbInfo.MailboxDatabaseGuid.ToString();
			if (LocalEndpointManager.IsDataCenter)
			{
				probeDefinition.Attributes["DownloadStaticFile"] = "False";
			}
			OwaUtils.AddBackendAuthenticationParameters(probeDefinition, dbInfo.MonitoringAccountSid, dbInfo.MonitoringAccountDomain);
		}

		private void CreateOwaMailboxMonitor()
		{
			string sampleMask = OwaDiscovery.BuildWorkItemName(OwaDiscovery.OwaProtocolString, OwaDiscovery.OwaDeepTestString, OwaDiscovery.ProbeString);
			string name = OwaDiscovery.BuildWorkItemName(OwaDiscovery.OwaProtocolString, OwaDiscovery.OwaDeepTestString, OwaDiscovery.MonitorString);
			Component owaProtocol = ExchangeComponent.OwaProtocol;
			MonitorDefinition monitorDefinition = OverallPercentSuccessByStateAttribute1Monitor.CreateDefinition(name, sampleMask, owaProtocol.Name, owaProtocol, 90.0, TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(5.0), "", true);
			if (this.ReadAttribute(OwaDiscovery.EnableFailoverParameterName, false) || this.ReadAttribute(OwaDiscovery.EnableRestartParameterName, false))
			{
				monitorDefinition.IsHaImpacting = true;
			}
			else
			{
				monitorDefinition.IsHaImpacting = false;
			}
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy1, TimeSpan.FromMinutes(6.0)),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy2, TimeSpan.FromMinutes(12.0)),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, TimeSpan.FromMinutes(20.0))
			};
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate OWA health is not impacted by mailbox connectivity issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
		}

		private void CreateOwaMailboxResponders()
		{
			string text = OwaDiscovery.BuildWorkItemName(OwaDiscovery.OwaProtocolString, OwaDiscovery.OwaDeepTestString, OwaDiscovery.MonitorString);
			string escalationSubjectUnhealthy = Strings.OwaDeepTestEscalationSubject(Environment.MachineName);
			string escalationMessageUnhealthy = this.GetEscalationBody(new Func<string, string, LocalizedString>(Strings.OwaDeepTestEscalationHtmlBody), new Func<string, string, LocalizedString>(Strings.OwaDeepTestEscalationBody), Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\Monitoring\\OWA\\MailboxProbe"));
			Component owaProtocol = ExchangeComponent.OwaProtocol;
			ResponderDefinition responderDefinition = ResetIISAppPoolResponder.CreateDefinition(OwaDiscovery.BuildWorkItemName(OwaDiscovery.OwaProtocolString, OwaDiscovery.OwaDeepTestString, OwaDiscovery.RestartString), text, "MSExchangeOWAAppPool", ServiceHealthStatus.Degraded, DumpMode.FullDump, null, 25.0, 90, "Exchange", true, "Dag");
			responderDefinition.ServiceName = owaProtocol.Name;
			responderDefinition.RecurrenceIntervalSeconds = 0;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			if (this.ReadAttribute(OwaDiscovery.EnableFailoverParameterName, false))
			{
				ResponderDefinition responderDefinition2 = SystemFailoverResponder.CreateDefinition(OwaDiscovery.BuildWorkItemName(OwaDiscovery.OwaProtocolString, OwaDiscovery.OwaDeepTestString, OwaDiscovery.FailoverString), text, ServiceHealthStatus.Unhealthy1, owaProtocol.Name, "Exchange", true);
				responderDefinition2.ServiceName = owaProtocol.Name;
				responderDefinition2.RecurrenceIntervalSeconds = 0;
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
			}
			if (this.ReadAttribute(OwaDiscovery.EnableRestartParameterName, false))
			{
				ResponderDefinition responderDefinition3 = DagForceRebootServerResponder.CreateDefinition(OwaDiscovery.BuildWorkItemName(OwaDiscovery.OwaProtocolString, OwaDiscovery.OwaDeepTestString, OwaDiscovery.KillServerString), text, ServiceHealthStatus.Unhealthy2);
				responderDefinition3.ServiceName = owaProtocol.Name;
				responderDefinition3.RecurrenceIntervalSeconds = 0;
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition3, base.TraceContext);
			}
			ResponderDefinition responderDefinition4 = EscalateResponder.CreateDefinition(OwaDiscovery.BuildWorkItemName(OwaDiscovery.OwaProtocolString, OwaDiscovery.OwaDeepTestString, OwaDiscovery.EscalateString), owaProtocol.Name, text, text, Environment.MachineName, ServiceHealthStatus.Unrecoverable, owaProtocol.EscalationTeam, escalationSubjectUnhealthy, escalationMessageUnhealthy, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition4.RecurrenceIntervalSeconds = 0;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition4, base.TraceContext);
		}

		private void CreateOwaProtocolProbe()
		{
			string sampleMask = OwaDiscovery.BuildWorkItemName(OwaDiscovery.OwaProtocolString, OwaDiscovery.OwaSelfTestString, OwaDiscovery.ProbeString);
			string text = OwaDiscovery.BuildWorkItemName(OwaDiscovery.OwaProtocolString, OwaDiscovery.OwaSelfTestString, OwaDiscovery.MonitorString);
			string escalationSubjectUnhealthy = Strings.OwaSelfTestEscalationSubject(Environment.MachineName);
			string escalationMessageUnhealthy = this.GetEscalationBody(new Func<string, string, LocalizedString>(Strings.OwaSelfTestEscalationHtmlBody), new Func<string, string, LocalizedString>(Strings.OwaSelfTestEscalationBody), Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\Monitoring\\OWA\\ProtocolProbe"));
			Component owaProtocol = ExchangeComponent.OwaProtocol;
			ProbeDefinition probeDefinition = new ProbeDefinition();
			OwaDiscovery.PopulateOwaProtocolProbeDefinition(probeDefinition);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(text, sampleMask, owaProtocol.Name, owaProtocol, 4, true, 300);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			if (this.ReadAttribute(OwaDiscovery.EnableFailoverParameterName, false) || this.ReadAttribute(OwaDiscovery.EnableRestartParameterName, false))
			{
				monitorDefinition.IsHaImpacting = true;
			}
			else
			{
				monitorDefinition.IsHaImpacting = false;
			}
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy1, (int)TimeSpan.FromMinutes(10.0).TotalSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy2, (int)TimeSpan.FromMinutes(20.0).TotalSeconds),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)TimeSpan.FromMinutes(25.0).TotalSeconds)
			};
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate OWA health is not impacted by any protocol issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition responderDefinition = ResetIISAppPoolResponder.CreateDefinition(OwaDiscovery.BuildWorkItemName(OwaDiscovery.OwaProtocolString, OwaDiscovery.OwaSelfTestString, OwaDiscovery.RestartString), text, "MSExchangeOWAAppPool", ServiceHealthStatus.Degraded, DumpMode.FullDump, null, 25.0, 90, "Exchange", true, "Dag");
			responderDefinition.ServiceName = owaProtocol.Name;
			responderDefinition.RecurrenceIntervalSeconds = monitorDefinition.RecurrenceIntervalSeconds;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			if (this.ReadAttribute(OwaDiscovery.EnableFailoverParameterName, false))
			{
				ResponderDefinition responderDefinition2 = SystemFailoverResponder.CreateDefinition(OwaDiscovery.BuildWorkItemName(OwaDiscovery.OwaProtocolString, OwaDiscovery.OwaSelfTestString, OwaDiscovery.FailoverString), text, ServiceHealthStatus.Unhealthy1, owaProtocol.Name, "Exchange", true);
				responderDefinition2.ServiceName = owaProtocol.Name;
				responderDefinition2.RecurrenceIntervalSeconds = monitorDefinition.RecurrenceIntervalSeconds;
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
			}
			if (this.ReadAttribute(OwaDiscovery.EnableRestartParameterName, false))
			{
				ResponderDefinition responderDefinition3 = DagForceRebootServerResponder.CreateDefinition(OwaDiscovery.BuildWorkItemName(OwaDiscovery.OwaProtocolString, OwaDiscovery.OwaSelfTestString, OwaDiscovery.KillServerString), text, ServiceHealthStatus.Unhealthy2);
				responderDefinition3.ServiceName = owaProtocol.Name;
				responderDefinition3.RecurrenceIntervalSeconds = monitorDefinition.RecurrenceIntervalSeconds;
				base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition3, base.TraceContext);
			}
			ResponderDefinition responderDefinition4 = EscalateResponder.CreateDefinition(OwaDiscovery.BuildWorkItemName(OwaDiscovery.OwaProtocolString, OwaDiscovery.OwaSelfTestString, OwaDiscovery.EscalateString), owaProtocol.Name, text, text, Environment.MachineName, ServiceHealthStatus.Unrecoverable, owaProtocol.EscalationTeam, escalationSubjectUnhealthy, escalationMessageUnhealthy, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition4.RecurrenceIntervalSeconds = probeDefinition.RecurrenceIntervalSeconds;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition4, base.TraceContext);
		}

		internal static void PopulateOwaProtocolProbeDefinition(ProbeDefinition probeDefinition)
		{
			string name = OwaDiscovery.BuildWorkItemName(OwaDiscovery.OwaProtocolString, OwaDiscovery.OwaSelfTestString, OwaDiscovery.ProbeString);
			Component owaProtocol = ExchangeComponent.OwaProtocol;
			string endpoint = "https://localhost:444/owa/";
			probeDefinition.AssemblyPath = OwaDiscovery.AssemblyPath;
			probeDefinition.TypeName = OwaDiscovery.OwaProtocolProbeTypeName;
			probeDefinition.Name = name;
			probeDefinition.ServiceName = owaProtocol.Name;
			probeDefinition.RecurrenceIntervalSeconds = 60;
			probeDefinition.TimeoutSeconds = 130;
			probeDefinition.MaxRetryAttempts = 0;
			probeDefinition.Endpoint = endpoint;
			probeDefinition.Attributes["SslValidationOptions"] = SslValidationOptions.NoSslValidation.ToString();
			probeDefinition.Attributes["LogFileInstanceName"] = "ProtocolProbe";
			probeDefinition.Attributes["RequestTimeout"] = OwaDiscovery.OwaProtocolProbeRequestTimeout.TotalSeconds.ToString();
			probeDefinition.Attributes["ScenarioTimeout"] = OwaDiscovery.OwaProtocolProbeScenarioTimeout.TotalSeconds.ToString();
		}

		private void CreateOwaClientAccessProbe(MailboxDatabaseInfo dbInfo, string endPoint, TimeSpan probeFrequency)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			OwaDiscovery.PopulateOwaClientAccessProbeDefinition(probeDefinition, dbInfo, probeFrequency, endPoint);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
		}

		internal static void PopulateOwaClientAccessProbeDefinition(ProbeDefinition probeDefinition, MailboxDatabaseInfo dbInfo, TimeSpan probeFrequency, string endPoint)
		{
			Component owa = ExchangeComponent.Owa;
			string name = OwaDiscovery.BuildWorkItemName(OwaDiscovery.OwaProtocolString, OwaDiscovery.OwaCustomerTouchPointString, OwaDiscovery.ProbeString);
			probeDefinition.AssemblyPath = OwaDiscovery.AssemblyPath;
			probeDefinition.TypeName = OwaDiscovery.OwaClientAccessProbeTypeName;
			probeDefinition.ServiceName = owa.Name;
			probeDefinition.Name = name;
			probeDefinition.RecurrenceIntervalSeconds = (int)probeFrequency.TotalSeconds;
			probeDefinition.TimeoutSeconds = 300;
			probeDefinition.MaxRetryAttempts = 0;
			probeDefinition.Account = dbInfo.MonitoringAccount + "@" + dbInfo.MonitoringAccountDomain;
			probeDefinition.AccountPassword = dbInfo.MonitoringAccountPassword;
			probeDefinition.AccountDisplayName = probeDefinition.Account;
			probeDefinition.Endpoint = endPoint;
			if (LocalEndpointManager.IsDataCenter || LocalEndpointManager.IsDataCenterDedicated || OwaDiscovery.IsDogfood)
			{
				probeDefinition.AccountDisplayName = probeDefinition.AccountDisplayName + " - Pwd: " + dbInfo.MonitoringAccountPassword;
			}
			probeDefinition.Attributes["SslValidationOptions"] = SslValidationOptions.NoSslValidation.ToString();
			probeDefinition.Attributes["LogFileInstanceName"] = "ClientAccessProbe";
			probeDefinition.Attributes["AcceptPassiveCopyErrorAsSuccess"] = "False";
			probeDefinition.TargetResource = dbInfo.MailboxDatabaseName;
		}

		private void CreateOwaClientAccessMonitor()
		{
			string sampleMask = OwaDiscovery.BuildWorkItemName(OwaDiscovery.OwaProtocolString, OwaDiscovery.OwaCustomerTouchPointString, OwaDiscovery.ProbeString);
			string name = OwaDiscovery.BuildWorkItemName(OwaDiscovery.OwaProtocolString, OwaDiscovery.OwaCustomerTouchPointString, OwaDiscovery.MonitorString);
			Component owa = ExchangeComponent.Owa;
			MonitorDefinition monitorDefinition = OverallPercentSuccessByStateAttribute1Monitor.CreateDefinition(name, sampleMask, owa.Name, owa, 90.0, TimeSpan.FromMinutes(2.0), TimeSpan.FromMinutes(2.0), TimeSpan.FromMinutes(20.0), "", true);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)TimeSpan.FromMinutes(10.0).TotalSeconds)
			};
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate OWA health is not impacted any apppool issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
		}

		private void CreateOwaClientAccessResponders()
		{
			string text = OwaDiscovery.BuildWorkItemName(OwaDiscovery.OwaProtocolString, OwaDiscovery.OwaCustomerTouchPointString, OwaDiscovery.MonitorString);
			string name = OwaDiscovery.BuildWorkItemName(OwaDiscovery.OwaProtocolString, OwaDiscovery.OwaCustomerTouchPointString, OwaDiscovery.EscalateString);
			string escalationSubjectUnhealthy = Strings.OwaCustomerTouchPointEscalationSubject(Environment.MachineName);
			string escalationMessageUnhealthy = this.GetEscalationBody(new Func<string, string, LocalizedString>(Strings.OwaCustomerTouchPointEscalationHtmlBody), new Func<string, string, LocalizedString>(Strings.OwaCustomerTouchPointEscalationBody), Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\Monitoring\\OWA\\ClientAccessProbe"));
			Component owa = ExchangeComponent.Owa;
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(name, owa.Name, text, text, Environment.MachineName, ServiceHealthStatus.Unrecoverable, owa.EscalationTeam, escalationSubjectUnhealthy, escalationMessageUnhealthy, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.RecurrenceIntervalSeconds = 0;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private LocalizedString GetEscalationBody(Func<string, string, LocalizedString> htmlBodyDelegate, Func<string, string, LocalizedString> regularBodyDelegate, string logFileName)
		{
			string machineName = Environment.MachineName;
			if (LocalEndpointManager.IsDataCenter || LocalEndpointManager.IsDataCenterDedicated)
			{
				return htmlBodyDelegate(machineName, logFileName);
			}
			return regularBodyDelegate(machineName, logFileName);
		}

		private static string BuildWorkItemName(string protocolName, string probeType, string workItemType)
		{
			return string.Format("{0}{1}", probeType, workItemType);
		}

		private bool CheckIfADFSAuthenticationIsEnabledOnServer()
		{
			IEnumerable<ADOwaVirtualDirectory> localOwaVirtualDirectories = DirectoryAccessor.Instance.GetLocalOwaVirtualDirectories();
			foreach (ADOwaVirtualDirectory adowaVirtualDirectory in localOwaVirtualDirectories)
			{
				if (adowaVirtualDirectory.AdfsAuthentication)
				{
					return true;
				}
			}
			return false;
		}

		private const int OwaMailboxProbeTimeout = 300;

		private const int OwaProtocolProbeTimeout = 130;

		private const int OwaMailboxProbeRecurrenceIntervalSeconds = 30;

		private const int OwaProtocolProbeRecurrenceIntervalSeconds = 60;

		private const string OwaBrickUrl = "https://localhost:444/owa/";

		internal const string OwaCafeUrl = "https://localhost/owa/";

		private const string SnapIn = "Microsoft.Exchange.Management.Powershell.E2010";

		internal static readonly string EnableFailoverParameterName = "EnableFailover";

		internal static readonly string EnableRestartParameterName = "EnableRestart";

		internal static readonly string OwaProtocolString = "OWA.Protocol";

		internal static readonly string OwaSelfTestString = "OwaSelfTest";

		internal static readonly string OwaDeepTestString = "OwaDeepTest";

		internal static readonly string OwaCustomerTouchPointString = "OwaCtp";

		internal static readonly string ProbeString = "Probe";

		internal static readonly string MonitorString = "Monitor";

		internal static readonly string RestartString = "Restart";

		internal static readonly string FailoverString = "Failover";

		internal static readonly string KillServerString = "KillServer";

		internal static readonly string EscalateString = "Escalate";

		private static readonly TimeSpan OwaProtocolProbeRequestTimeout = TimeSpan.FromSeconds(30.0);

		private static readonly TimeSpan OwaProtocolProbeScenarioTimeout = TimeSpan.FromSeconds(40.0);

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string OwaMailboxProbeTypeName = typeof(OwaBackEndMailboxProbe).FullName;

		private static readonly string OwaProtocolProbeTypeName = typeof(OwaHealthCheckProbe).FullName;

		private static readonly string OwaClientAccessProbeTypeName = typeof(OwaCtpProbe).FullName;

		private static readonly bool IsDogfood = RegistryHelper.GetPropertyIntBool("IsDogfood", false, "Owa", string.Format("SOFTWARE\\Microsoft\\ExchangeServer\\{0}", "v15"), false);
	}
}
