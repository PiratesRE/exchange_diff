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

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PopImap.IMAP4
{
	public sealed class ImapDiscoveryV2 : PopImapDiscoveryCommon
	{
		internal static PopImapAdConfiguration GetConfiguration(TracingContext context)
		{
			PopImapAdConfiguration retVal = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ITopologyConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 90, "GetConfiguration", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Imap4\\ImapDiscoveryV2.cs");
				retVal = PopImapAdConfiguration.FindOne<Imap4AdConfiguration>(session);
			});
			if (!adoperationResult.Succeeded)
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.IMAP4Tracer, context, "ImapDiscovery:: DoWork(): Unable to retrieve Imap Configuration: {0}", adoperationResult.Exception.Message, null, "GetConfiguration", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Imap4\\ImapDiscoveryV2.cs", 100);
			}
			return retVal;
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			PopImapAdConfiguration configuration = ImapDiscovery.GetConfiguration(base.TraceContext);
			if (!PopImapDiscoveryCommon.GetEndpointsFromConfig(configuration, out ImapDiscoveryV2.cafeEndpoint, out ImapDiscoveryV2.mbxEndpoint))
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.IMAP4Tracer, base.TraceContext, "ImapDiscovery.DoWork: Failed to Autodetect Imap settings. Using Default values.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Imap4\\ImapDiscoveryV2.cs", 120);
				ImapDiscoveryV2.cafeEndpoint = new IPEndPoint(IPAddress.Loopback, ImapDiscoveryV2.DefaultCafePort);
				ImapDiscoveryV2.mbxEndpoint = new IPEndPoint(IPAddress.Loopback, ImapDiscoveryV2.DefaultMbxPort);
			}
			ImapDiscoveryV2.endpointManager = LocalEndpointManager.Instance;
			try
			{
				if (ImapDiscoveryV2.endpointManager.ExchangeServerRoleEndpoint == null)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.IMAP4Tracer, base.TraceContext, "ImapDiscovery.DoWork: No Exchange roles installed on server, skipping item creation.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Imap4\\ImapDiscoveryV2.cs", 140);
					return;
				}
			}
			catch (EndpointManagerEndpointUninitializedException ex)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.IMAP4Tracer, base.TraceContext, string.Format("ImapDiscovery:: DoWork(): ExchangeServerRoleEndpoint initialisation failed.  Exception:{0}", ex.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Imap4\\ImapDiscoveryV2.cs", 149);
				return;
			}
			RegistryKey registryKey;
			if (!ImapDiscoveryV2.endpointManager.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.IMAP4Tracer, base.TraceContext, "ImapDiscovery.DoWork: Mailbox role is not installed on this server, no need to create Imap Mailbox related work items", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Imap4\\ImapDiscoveryV2.cs", 159);
			}
			else
			{
				try
				{
					if (ImapDiscoveryV2.endpointManager.MailboxDatabaseEndpoint == null)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.IMAP4Tracer, base.TraceContext, "ImapDiscovery:: DoWork(): Could not find MailboxDatabaseEndpoint", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Imap4\\ImapDiscoveryV2.cs", 172);
						return;
					}
				}
				catch (EndpointManagerEndpointUninitializedException ex2)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.IMAP4Tracer, base.TraceContext, string.Format("ImapDiscovery:: DoWork(): MailboxDatabaseEndpoint initialisation failed.  Exception:{0}", ex2.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Imap4\\ImapDiscoveryV2.cs", 178);
					return;
				}
				registryKey = RegistryHelper.OpenKey("SYSTEM\\CurrentControlSet\\services\\", "MSExchangeImap4BE", false, false);
				int num = (int)registryKey.GetValue("Start");
				if (num == 2)
				{
					this.CreateImapProtocolContext(true);
					if (!VariantConfiguration.InvariantNoFlightingSnapshot.ActiveMonitoring.PopImapDiscoveryCommon.Enabled)
					{
						this.CreateImapServiceContext(true);
					}
				}
				else
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.IMAP4Tracer, base.TraceContext, "ImapDiscovery.DoWork: Mailbox Imap service is not set to AUTOMATIC, will not create Imap Mailbox related work items", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Imap4\\ImapDiscoveryV2.cs", 198);
				}
			}
			if (!ImapDiscoveryV2.endpointManager.ExchangeServerRoleEndpoint.IsCafeRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.IMAP4Tracer, base.TraceContext, "ImapDiscovery.DoWork: CAFE role is not installed on this server, no need to create Imap CAFE related work items", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Imap4\\ImapDiscoveryV2.cs", 208);
				return;
			}
			try
			{
				if (ImapDiscoveryV2.endpointManager.MailboxDatabaseEndpoint == null)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.IMAP4Tracer, base.TraceContext, "ImapDiscovery:: DoWork(): Could not find MailboxDatabaseEndpoint", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Imap4\\ImapDiscoveryV2.cs", 221);
					return;
				}
			}
			catch (Exception ex3)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.IMAP4Tracer, base.TraceContext, string.Format("ImapDiscovery:: DoWork(): MailboxDatabaseEndpoint object threw exception.  Exception:{0}", ex3.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Imap4\\ImapDiscoveryV2.cs", 227);
				return;
			}
			registryKey = RegistryHelper.OpenKey("SYSTEM\\CurrentControlSet\\services\\", "MSExchangeImap4", false, false);
			int num2 = (int)registryKey.GetValue("Start");
			if (num2 == 2)
			{
				this.CreateImapProtocolContext(false);
				if (!VariantConfiguration.InvariantNoFlightingSnapshot.ActiveMonitoring.PopImapDiscoveryCommon.Enabled)
				{
					this.CreateImapServiceContext(false);
					return;
				}
			}
			else
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.IMAP4Tracer, base.TraceContext, "ImapDiscovery.DoWork: CAFE Imap service is not set to AUTOMATIC, will not create Imap CAFE related work items", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PopImap\\Imap4\\ImapDiscoveryV2.cs", 247);
			}
		}

		private void CreateImapProtocolContext(bool isMbx)
		{
			string text = isMbx ? "MSExchangeImap4BE" : "MSExchangeImap4";
			ProbeDefinition probeDefinition;
			if (isMbx)
			{
				probeDefinition = ImapSelfTestProbe.CreateDefinition(ImapDiscoveryV2.AssemblyPath);
				probeDefinition.SecondaryEndpoint = ImapDiscoveryV2.mbxEndpoint.Port.ToString();
			}
			else
			{
				probeDefinition = ImapProxyTestProbe.CreateDefinition(ImapDiscoveryV2.AssemblyPath);
				probeDefinition.SecondaryEndpoint = ImapDiscoveryV2.cafeEndpoint.Port.ToString();
			}
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = Microsoft.Office.Datacenter.ActiveMonitoring.OverallConsecutiveProbeFailuresMonitor.CreateDefinition(probeDefinition.Name.Replace("Probe", "Monitor"), probeDefinition.Name, isMbx ? ExchangeComponent.ImapProtocol.Name : ExchangeComponent.ImapProxy.Name, isMbx ? ExchangeComponent.ImapProtocol : ExchangeComponent.ImapProxy, 4, true, 240);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.TargetResource = text;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate Imap health on BE are not impacted any issues";
			if (isMbx)
			{
				monitorDefinition.IsHaImpacting = true;
			}
			monitorDefinition.MonitorStateTransitions = PopImapDiscoveryCommon.CreateResponderChain(monitorDefinition.Name, text, text, base.Broker, true, isMbx ? PopImapDiscoveryCommon.TargetScope.PST : PopImapDiscoveryCommon.TargetScope.PT, base.TraceContext);
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate IMAP health is not impacted by any issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
		}

		private void CreateImapServiceContext(bool isMbx)
		{
			ProbeDefinition probeDefinition;
			MonitorDefinition monitorDefinition;
			if (isMbx)
			{
				probeDefinition = ImapDeepTestProbe.CreateDefinition(ImapDiscoveryV2.AssemblyPath);
				probeDefinition.SecondaryEndpoint = ImapDiscoveryV2.mbxEndpoint.Port.ToString();
				monitorDefinition = Microsoft.Office.Datacenter.ActiveMonitoring.OverallConsecutiveProbeFailuresMonitor.CreateDefinition(probeDefinition.Name.Replace("Probe", "Monitor"), probeDefinition.Name, ExchangeComponent.ImapProtocol.Name, ExchangeComponent.ImapProtocol, 5, true, 900);
				monitorDefinition.RecurrenceIntervalSeconds = 0;
				monitorDefinition.TargetResource = "MSExchangeImap4BE";
				monitorDefinition.IsHaImpacting = true;
				monitorDefinition.ServicePriority = 0;
				monitorDefinition.ScenarioDescription = "Validate Imap health on BE are not impacted any issues";
			}
			else
			{
				probeDefinition = ImapCustomerTouchPointProbe.CreateDefinition(ImapDiscoveryV2.AssemblyPath);
				probeDefinition.SecondaryEndpoint = ImapDiscoveryV2.cafeEndpoint.Port.ToString();
				monitorDefinition = Microsoft.Office.Datacenter.ActiveMonitoring.OverallConsecutiveProbeFailuresMonitor.CreateDefinition(probeDefinition.Name.Replace("Probe", "Monitor"), probeDefinition.Name, ExchangeComponent.Imap.Name, ExchangeComponent.Imap, 5, true, 1200);
				monitorDefinition.RecurrenceIntervalSeconds = 0;
				monitorDefinition.TargetResource = "MSExchangeImap4";
				monitorDefinition.ServicePriority = 0;
				monitorDefinition.ScenarioDescription = "Validate Imap health on BE are not impacted any issues";
			}
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			if (isMbx)
			{
				monitorDefinition.IsHaImpacting = true;
			}
			monitorDefinition.MonitorStateTransitions = PopImapDiscoveryCommon.CreateResponderChain(monitorDefinition.Name, isMbx ? "MSExchangeImap4BE" : "MSExchangeImap4", isMbx ? ExchangeComponent.ImapProtocol.Name : ExchangeComponent.Imap.Name, base.Broker, true, isMbx ? PopImapDiscoveryCommon.TargetScope.MDT : PopImapDiscoveryCommon.TargetScope.CTP, base.TraceContext);
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = "Validate IMAP health is not impacted by any issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
		}

		private const string BrickServiceName = "MSExchangeImap4BE";

		private const string CafeServiceName = "MSExchangeImap4";

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly Type OverallConsecutiveProbeFailuresMonitor = typeof(OverallConsecutiveProbeFailuresMonitor);

		private static readonly int DefaultMbxPort = 1993;

		private static readonly int DefaultCafePort = 993;

		private static LocalEndpointManager endpointManager;

		private static IPEndPoint cafeEndpoint;

		private static IPEndPoint mbxEndpoint;
	}
}
