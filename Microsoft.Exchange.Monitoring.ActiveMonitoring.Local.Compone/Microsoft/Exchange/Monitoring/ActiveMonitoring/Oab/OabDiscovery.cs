using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Oab.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Oab
{
	public sealed class OabDiscovery : MaintenanceWorkItem
	{
		public bool IsOnPremisesEnabled { get; private set; }

		public int OabProtocolProbeTimeout { get; private set; }

		public int OabMailboxProbeTimeout { get; private set; }

		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.Configure();
			if (!LocalEndpointManager.IsDataCenter && !this.IsOnPremisesEnabled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OABTracer, base.TraceContext, "OabDiscovery.DoWork: In case of on-premises, EnableOnPrem should be true in order to create probe/monitor/responder", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabDiscovery.cs", 141);
				base.Result.StateAttribute1 = "OabDiscovery.DoWork: In case of on-premises, EnableOnPrem should be true in order to create probe/monitor/responder";
				return;
			}
			try
			{
				LocalEndpointManager instance = LocalEndpointManager.Instance;
				if (!instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.OABTracer, base.TraceContext, "OabDiscovery.DoWork: Skip creating the probe for non-MBX server", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabDiscovery.cs", 156);
				}
				else
				{
					this.SetupOabProbes(base.TraceContext, instance);
				}
			}
			catch (EndpointManagerEndpointUninitializedException ex)
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OABTracer, base.TraceContext, "OabDiscovery.DoWork: Endpoint initialization failed. Treating as transient error. Exception:{0}", ex.ToString(), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabDiscovery.cs", 170);
			}
		}

		private void Configure()
		{
			this.IsOnPremisesEnabled = this.ReadAttribute("EnableOnPrem", false);
			this.OabProtocolProbeTimeout = this.ReadAttribute("OabProtocolProbeTimeoutInSeconds", 20);
			this.OabMailboxProbeTimeout = this.ReadAttribute("OabMailboxProbeTimeoutInSeconds", 20);
		}

		private void SetupOabProbes(TracingContext traceContext, LocalEndpointManager endpointManager)
		{
			if (endpointManager.MailboxDatabaseEndpoint == null)
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OnlineMeetingTracer, base.TraceContext, "OabDiscovery.SetupOabProbes: No mailbox endpoint found {0}", Environment.MachineName, null, "SetupOabProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabDiscovery.cs", 199);
				return;
			}
			IEnumerable<MailboxDatabaseInfo> enumerable = endpointManager.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend;
			if (enumerable == null)
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OnlineMeetingTracer, base.TraceContext, "OabDiscovery.SetupOabProbes: No mailbox databases found {0}", Environment.MachineName, null, "SetupOabProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabDiscovery.cs", 212);
				return;
			}
			enumerable = from x in enumerable
			where !string.IsNullOrEmpty(x.MonitoringAccount) && !string.IsNullOrEmpty(x.MonitoringAccountPassword)
			select x;
			if (enumerable.Count<MailboxDatabaseInfo>() == 0)
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OnlineMeetingTracer, base.TraceContext, "OabDiscovery.SetupOabProbes: No mailbox databases were found with a monitoring mailbox {0}", Environment.MachineName, null, "SetupOabProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabDiscovery.cs", 227);
				return;
			}
			MailboxDatabaseInfo credentialHolder = enumerable.First<MailboxDatabaseInfo>();
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OABTracer, base.TraceContext, "OabDiscovery.SetupOabProbes: Creating Oab protocol probe for server {0}", Environment.MachineName, null, "SetupOabProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabDiscovery.cs", 239);
			this.SetupOabProtocolProbe(base.TraceContext, credentialHolder, endpointManager);
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OABTracer, base.TraceContext, "OabDiscovery.SetupOabProbes: Created Oab protocol probe for server {0}", Environment.MachineName, null, "SetupOabProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabDiscovery.cs", 248);
			if (LocalEndpointManager.IsDataCenter)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OABTracer, base.TraceContext, "OabDiscovery.SetupOabProbes: No oab, skip the work item", null, "SetupOabProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabDiscovery.cs", 299);
				return;
			}
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OABTracer, base.TraceContext, "OabDiscovery.SetupOabProbes: Creating Oab mailbox probe for server {0}", Environment.MachineName, null, "SetupOabProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabDiscovery.cs", 257);
			if (endpointManager.OfflineAddressBookEndpoint.OrganizationMailboxDatabases == null || endpointManager.OfflineAddressBookEndpoint.OrganizationMailboxDatabases.Length == 0)
			{
				throw new ApplicationException(Strings.OabMailboxNoOrgMailbox);
			}
			IEnumerable<Guid> first = from n in endpointManager.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend
			select n.MailboxDatabaseGuid;
			IEnumerable<Guid> source = first.Intersect(endpointManager.OfflineAddressBookEndpoint.OrganizationMailboxDatabases);
			IEnumerable<string> values = from n in source
			select n.ToString("D");
			string text = string.Join(",", values);
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OABTracer, base.TraceContext, "OabDiscovery.SetupOabProbes: OrgDatabases is {0}", text, null, "SetupOabProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabDiscovery.cs", 275);
			foreach (Guid guid in endpointManager.OfflineAddressBookEndpoint.OfflineAddressBooks)
			{
				string text2 = guid.ToString("D");
				string endPoint = string.Format("{0}{1}/oab.xml", "https://localhost:444/oab/", text2);
				this.CreateOabMailboxProbe(base.TraceContext, credentialHolder, endpointManager, endPoint, text2, text);
				WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.OABTracer, base.TraceContext, "OabDiscovery.SetupOabProbes: Created Oab Mailbox probe for OAB {0} on Server {1}", text2, Environment.MachineName, null, "SetupOabProbes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabDiscovery.cs", 289);
			}
		}

		private void SetupOabProtocolProbe(TracingContext traceContext, MailboxDatabaseInfo credentialHolder, LocalEndpointManager endpointManager)
		{
			string name = "OabProtocolProbe";
			string text = "OabProtocolMonitor";
			string text2 = "OabProtocolResponder";
			string escalationSubjectUnhealthy = Strings.OabProtocolEscalationSubject(Environment.MachineName);
			string escalationMessageUnhealthy = Strings.OabProtocolEscalationBody;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.OABTracer, base.TraceContext, "OabDiscovery.SetupOabProtocolProbe: Creating Oab protocol probe for this server", null, "SetupOabProtocolProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabDiscovery.cs", 322);
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = OabDiscovery.AssemblyPath;
			probeDefinition.TypeName = OabDiscovery.OabProtocolProbeTypeName;
			probeDefinition.Name = name;
			probeDefinition.TargetResource = Environment.MachineName;
			probeDefinition.RecurrenceIntervalSeconds = 60;
			probeDefinition.TimeoutSeconds = this.OabProtocolProbeTimeout;
			probeDefinition.MaxRetryAttempts = 0;
			probeDefinition.Endpoint = string.Format("{0}{1}/oab.xml", "https://localhost:444/oab/", "123");
			probeDefinition.Account = credentialHolder.MonitoringAccount + "@" + credentialHolder.MonitoringAccountDomain;
			probeDefinition.AccountPassword = credentialHolder.MonitoringAccountPassword;
			probeDefinition.ServiceName = ExchangeComponent.Oab.Name;
			probeDefinition.Attributes["TrustAnySslCertificate"] = "True";
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition(text, probeDefinition.ConstructWorkItemResultName(), ExchangeComponent.Oab.Name, ExchangeComponent.Oab, 4, true, 300);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, (int)TimeSpan.FromMinutes(15.0).TotalSeconds)
			};
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Validate OAB health is not impacted by any issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string text3 = monitorDefinition.ConstructWorkItemResultName();
			ResponderDefinition responderDefinition = ResetIISAppPoolResponder.CreateDefinition(text2, text3, "MSExchangeOABAppPool", ServiceHealthStatus.Degraded, DumpMode.None, null, 15.0, 0, "Exchange", true, null);
			responderDefinition.ServiceName = ExchangeComponent.Oab.Name;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
			ResponderDefinition responderDefinition2 = EscalateResponder.CreateDefinition(text2, ExchangeComponent.Oab.Name, text, text3, Environment.MachineName, ServiceHealthStatus.Unrecoverable, ExchangeComponent.Oab.EscalationTeam, escalationSubjectUnhealthy, escalationMessageUnhealthy, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition2.WaitIntervalSeconds = 28800;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition2, base.TraceContext);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.OABTracer, base.TraceContext, "OabDiscovery.SetupOabProtocolProbe: Created Oab protocol probe for this server", null, "SetupOabProtocolProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Oab\\OabDiscovery.cs", 396);
		}

		private void CreateOabMailboxProbe(TracingContext traceContext, MailboxDatabaseInfo credentialHolder, LocalEndpointManager endpointManager, string endPoint, string oabGuid, string orgMbxDatabaseIds)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = OabDiscovery.AssemblyPath;
			probeDefinition.TypeName = OabDiscovery.OabMailboxProbeTypeName;
			probeDefinition.Name = "OabMailboxProbe";
			probeDefinition.TargetResource = oabGuid;
			probeDefinition.RecurrenceIntervalSeconds = 1800;
			probeDefinition.TimeoutSeconds = this.OabMailboxProbeTimeout;
			probeDefinition.MaxRetryAttempts = 0;
			probeDefinition.Endpoint = endPoint;
			probeDefinition.Account = credentialHolder.MonitoringAccount + "@" + credentialHolder.MonitoringAccountDomain;
			probeDefinition.AccountPassword = credentialHolder.MonitoringAccountPassword;
			probeDefinition.ServiceName = ExchangeComponent.Oab.Name;
			probeDefinition.Attributes["OrgMbxDBId"] = orgMbxDatabaseIds;
			probeDefinition.Attributes["TrustAnySslCertificate"] = "True";
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			this.CreateOabMailboxMonitor(probeDefinition, base.TraceContext, credentialHolder);
		}

		private void CreateOabMailboxMonitor(ProbeDefinition probeDef, TracingContext traceContext, MailboxDatabaseInfo dbInfo)
		{
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition("OabMailboxMonitor", probeDef.ConstructWorkItemResultName(), ExchangeComponent.Oab.Name, ExchangeComponent.Oab, 4000, 0, 4, true);
			monitorDefinition.TargetResource = probeDef.TargetResource;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0)
			};
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Validate OAB health is not impacted by mailbox issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			this.CreateOabMailboxResponders(probeDef, monitorDefinition, base.TraceContext, dbInfo);
		}

		private void CreateOabMailboxResponders(ProbeDefinition probeDef, MonitorDefinition monitorDef, TracingContext traceContext, MailboxDatabaseInfo dbInfo)
		{
			string alertMask = monitorDef.ConstructWorkItemResultName();
			string name = "OabMailboxResponder";
			string escalationSubjectUnhealthy = Strings.OabMailboxEscalationSubject(probeDef.TargetResource, Environment.MachineName);
			string escalationMessageUnhealthy = Strings.OabMailboxEscalationBody;
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(name, ExchangeComponent.Oab.Name, monitorDef.Name, alertMask, probeDef.TargetResource, ServiceHealthStatus.Unhealthy, ExchangeComponent.Oab.EscalationTeam, escalationSubjectUnhealthy, escalationMessageUnhealthy, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.WaitIntervalSeconds = 28800;
			responderDefinition.RecurrenceIntervalSeconds = 1800;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private const int OabMailboxProbeRecurrenceIntervalSeconds = 1800;

		private const int OabMailboxMonitorRecurrenceIntervalSeconds = 4000;

		private const int OabMailboxResponderRecurrenceIntervalSeconds = 1800;

		private const int OabProtocolProbeRecurrenceIntervalSeconds = 60;

		private const int OabResponderWaitIntervalSeconds = 28800;

		private const int OabProtocolProbeDefaultTimeout = 20;

		private const int OabMailboxProbeDefaultTimeout = 20;

		private const string OabBrickUrl = "https://localhost:444/oab/";

		private const string OabProtocolProbeName = "OabProtocolProbe";

		private const string OabProtocolMonitorName = "OabProtocolMonitor";

		private const string OabProtocolResponderName = "OabProtocolResponder";

		private const string OabMailboxProbeName = "OabMailboxProbe";

		private const string OabMailboxMonitorName = "OabMailboxMonitor";

		private const string OabMailboxResponderName = "OabMailboxResponder";

		private const string OabAppPoolName = "MSExchangeOABAppPool";

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string OabProtocolProbeTypeName = typeof(OabProtocolProbe).FullName;

		private static readonly string OabMailboxProbeTypeName = typeof(OabMailboxProbe).FullName;
	}
}
