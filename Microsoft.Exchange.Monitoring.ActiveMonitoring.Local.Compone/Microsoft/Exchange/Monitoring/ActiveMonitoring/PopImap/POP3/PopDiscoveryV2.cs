using System;
using System.Net;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.PopImap.Probes;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PopImap.POP3
{
	public sealed class PopDiscoveryV2 : PopImapDiscoveryCommon
	{
		internal static PopImapAdConfiguration GetConfiguration(TracingContext context)
		{
			PopImapAdConfiguration retVal = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ITopologyConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 90, "GetConfiguration", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Pop3\\PopDiscoveryV2.cs");
				retVal = PopImapAdConfiguration.FindOne<Pop3AdConfiguration>(session);
			});
			if (!adoperationResult.Succeeded)
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.POP3Tracer, context, "PopDiscovery:: DoWork(): Unable to retrieve Pop Configuration: {0}", adoperationResult.Exception.Message, null, "GetConfiguration", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Pop3\\PopDiscoveryV2.cs", 100);
			}
			return retVal;
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			PopImapAdConfiguration configuration = PopDiscovery.GetConfiguration(base.TraceContext);
			if (!PopImapDiscoveryCommon.GetEndpointsFromConfig(configuration, out PopDiscoveryV2.cafeEndpoint, out PopDiscoveryV2.mbxEndpoint))
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.POP3Tracer, base.TraceContext, "PopDiscovery.DoWork: Failed to Autodetect Pop settings. Using Default values.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Pop3\\PopDiscoveryV2.cs", 120);
				PopDiscoveryV2.cafeEndpoint = new IPEndPoint(IPAddress.Loopback, PopDiscoveryV2.DefaultCafePort);
				PopDiscoveryV2.mbxEndpoint = new IPEndPoint(IPAddress.Loopback, PopDiscoveryV2.DefaultMbxPort);
			}
			PopDiscoveryV2.endpointManager = LocalEndpointManager.Instance;
			try
			{
				if (PopDiscoveryV2.endpointManager.ExchangeServerRoleEndpoint == null)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.POP3Tracer, base.TraceContext, "PopDiscovery.DoWork: No Exchange roles installed on server, skipping item creation.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Pop3\\PopDiscoveryV2.cs", 140);
					return;
				}
			}
			catch (EndpointManagerEndpointUninitializedException ex)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.POP3Tracer, base.TraceContext, string.Format("PopDiscovery:: DoWork(): ExchangeServerRoleEndpoint initialisation failed.  Exception:{0}", ex.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Pop3\\PopDiscoveryV2.cs", 149);
				return;
			}
			RegistryKey registryKey;
			if (!PopDiscoveryV2.endpointManager.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.POP3Tracer, base.TraceContext, "PopDiscovery.DoWork: Mailbox role is not installed on this server, no need to create Pop Mailbox related work items", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Pop3\\PopDiscoveryV2.cs", 159);
			}
			else
			{
				try
				{
					if (PopDiscoveryV2.endpointManager.MailboxDatabaseEndpoint == null)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.POP3Tracer, base.TraceContext, "PopDiscovery:: DoWork(): Could not find MailboxDatabaseEndpoint", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Pop3\\PopDiscoveryV2.cs", 172);
						return;
					}
				}
				catch (EndpointManagerEndpointUninitializedException ex2)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.POP3Tracer, base.TraceContext, string.Format("PopDiscovery:: DoWork(): MailboxDatabaseEndpoint initialisation failed.  Exception:{0}", ex2.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Pop3\\PopDiscoveryV2.cs", 178);
					return;
				}
				registryKey = RegistryHelper.OpenKey("SYSTEM\\CurrentControlSet\\services\\", "MSExchangePop3BE", false, false);
				int num = (int)registryKey.GetValue("Start");
				if (num == 2)
				{
					this.CreatePopProtocolContext(true);
					if (!VariantConfiguration.InvariantNoFlightingSnapshot.ActiveMonitoring.PopImapDiscoveryCommon.Enabled)
					{
						this.CreatePopServiceContext(true);
					}
				}
				else
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.POP3Tracer, base.TraceContext, "PopDiscovery.DoWork: Mailbox Pop service is not set to AUTOMATIC, will not create Pop Mailbox related work items", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Pop3\\PopDiscoveryV2.cs", 198);
				}
			}
			if (!PopDiscoveryV2.endpointManager.ExchangeServerRoleEndpoint.IsCafeRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.POP3Tracer, base.TraceContext, "PopDiscovery.DoWork: CAFE role is not installed on this server, no need to create Pop CAFE related work items", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Pop3\\PopDiscoveryV2.cs", 208);
				return;
			}
			try
			{
				if (PopDiscoveryV2.endpointManager.MailboxDatabaseEndpoint == null)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.POP3Tracer, base.TraceContext, "PopDiscovery:: DoWork(): Could not find MailboxDatabaseEndpoint", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Pop3\\PopDiscoveryV2.cs", 221);
					return;
				}
			}
			catch (Exception ex3)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.POP3Tracer, base.TraceContext, string.Format("PopDiscovery:: DoWork(): MailboxDatabaseEndpoint object threw exception.  Exception:{0}", ex3.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Pop3\\PopDiscoveryV2.cs", 227);
				return;
			}
			registryKey = RegistryHelper.OpenKey("SYSTEM\\CurrentControlSet\\services\\", "MSExchangePop3", false, false);
			int num2 = (int)registryKey.GetValue("Start");
			if (num2 == 2)
			{
				this.CreatePopProtocolContext(false);
				if (!VariantConfiguration.InvariantNoFlightingSnapshot.ActiveMonitoring.PopImapDiscoveryCommon.Enabled)
				{
					this.CreatePopServiceContext(false);
					return;
				}
			}
			else
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.POP3Tracer, base.TraceContext, "PopDiscovery.DoWork: CAFE Pop service is not set to AUTOMATIC, will not create Pop CAFE related work items", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Pop3\\PopDiscoveryV2.cs", 247);
			}
		}

		private void CreatePopProtocolContext(bool isMbx)
		{
			string text = isMbx ? "MSExchangePop3BE" : "MSExchangePop3";
			ProbeDefinition probeDefinition;
			if (isMbx)
			{
				probeDefinition = PopSelfTestProbe.CreateDefinition(PopDiscoveryV2.AssemblyPath);
				probeDefinition.SecondaryEndpoint = PopDiscoveryV2.mbxEndpoint.Port.ToString();
			}
			else
			{
				probeDefinition = PopProxyTestProbe.CreateDefinition(PopDiscoveryV2.AssemblyPath);
				probeDefinition.SecondaryEndpoint = PopDiscoveryV2.cafeEndpoint.Port.ToString();
			}
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = Microsoft.Office.Datacenter.ActiveMonitoring.OverallConsecutiveProbeFailuresMonitor.CreateDefinition(probeDefinition.Name.Replace("Probe", "Monitor"), probeDefinition.Name, isMbx ? ExchangeComponent.PopProtocol.Name : ExchangeComponent.PopProxy.Name, isMbx ? ExchangeComponent.PopProtocol : ExchangeComponent.PopProxy, 4, true, 240);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.TargetResource = text;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Imap health on BE are not impacted any issues";
			if (isMbx)
			{
				monitorDefinition.IsHaImpacting = true;
			}
			monitorDefinition.MonitorStateTransitions = PopImapDiscoveryCommon.CreateResponderChain(monitorDefinition.Name, text, text, base.Broker, false, isMbx ? PopImapDiscoveryCommon.TargetScope.PST : PopImapDiscoveryCommon.TargetScope.PT, base.TraceContext);
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate POP health is not impacted by any issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
		}

		private void CreatePopServiceContext(bool isMbx)
		{
			ProbeDefinition probeDefinition;
			MonitorDefinition monitorDefinition;
			if (isMbx)
			{
				probeDefinition = PopDeepTestProbe.CreateDefinition(PopDiscoveryV2.AssemblyPath);
				probeDefinition.SecondaryEndpoint = PopDiscoveryV2.mbxEndpoint.Port.ToString();
				monitorDefinition = Microsoft.Office.Datacenter.ActiveMonitoring.OverallConsecutiveProbeFailuresMonitor.CreateDefinition(probeDefinition.Name.Replace("Probe", "Monitor"), probeDefinition.Name, ExchangeComponent.PopProtocol.Name, ExchangeComponent.PopProtocol, 5, true, 900);
				monitorDefinition.RecurrenceIntervalSeconds = 0;
				monitorDefinition.TargetResource = "MSExchangePop3BE";
				monitorDefinition.IsHaImpacting = true;
				monitorDefinition.ServicePriority = 0;
				monitorDefinition.ScenarioDescription = "Validate Imap health on BE are not impacted any issues";
			}
			else
			{
				probeDefinition = PopCustomerTouchPointProbe.CreateDefinition(PopDiscoveryV2.AssemblyPath);
				probeDefinition.SecondaryEndpoint = PopDiscoveryV2.cafeEndpoint.Port.ToString();
				monitorDefinition = Microsoft.Office.Datacenter.ActiveMonitoring.OverallConsecutiveProbeFailuresMonitor.CreateDefinition(probeDefinition.Name.Replace("Probe", "Monitor"), probeDefinition.Name, ExchangeComponent.Pop.Name, ExchangeComponent.Pop, 5, true, 900);
				monitorDefinition.RecurrenceIntervalSeconds = 0;
				monitorDefinition.TargetResource = "MSExchangePop3BE";
				monitorDefinition.IsHaImpacting = true;
				monitorDefinition.ServicePriority = 0;
				monitorDefinition.ScenarioDescription = "Validate Imap health on BE are not impacted any issues";
			}
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			monitorDefinition.MonitorStateTransitions = PopImapDiscoveryCommon.CreateResponderChain(monitorDefinition.Name, isMbx ? "MSExchangePop3BE" : "MSExchangePop3", isMbx ? ExchangeComponent.PopProtocol.Name : ExchangeComponent.Pop.Name, base.Broker, false, isMbx ? PopImapDiscoveryCommon.TargetScope.PST : PopImapDiscoveryCommon.TargetScope.PT, base.TraceContext);
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate POP health is not impacted by any issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
		}

		private const string BrickServiceName = "MSExchangePop3BE";

		private const string CafeServiceName = "MSExchangePop3";

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly Type OverallConsecutiveProbeFailuresMonitor = typeof(OverallConsecutiveProbeFailuresMonitor);

		private static readonly int DefaultMbxPort = 1995;

		private static readonly int DefaultCafePort = 995;

		private static LocalEndpointManager endpointManager;

		private static IPEndPoint cafeEndpoint;

		private static IPEndPoint mbxEndpoint;
	}
}
