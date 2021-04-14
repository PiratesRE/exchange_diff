using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.ApplicationLogic.Autodiscover;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.RpcClientAccess;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	public sealed class OutlookRpcDiscovery : OutlookDiscovery
	{
		protected override void AddBackendActiveMonitoring()
		{
			base.AddBackendActiveMonitoring();
			this.AddServiceTest();
		}

		internal override MonitorStateTransition[] GetProtocolMonitorStateTransitions()
		{
			return OutlookRpcDiscovery.ProtocolMonitorStateTransitions;
		}

		protected override bool DidDiscoveryExecuteSuccessfully()
		{
			return OutlookRpcDiscovery.didDiscoveryExecuteSuccessfully;
		}

		protected override void SetSuccessfulDiscoveryExecutionStatus()
		{
			OutlookRpcDiscovery.didDiscoveryExecuteSuccessfully = true;
		}

		private MonitorDefinition CreateCheckMemoryRestartSnappyMonitor(ProbeIdentity probeIdentity)
		{
			MonitorDefinition monitorDefinition = base.CreateSnappyMonitor(probeIdentity).ConfigureMonitorStateTransitions(new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, TimeSpan.FromMinutes(0.0)),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, TimeSpan.FromMinutes(15.0))
			});
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.ScenarioDescription = string.Format("Validate {0} health is not impacted by recoverable issues", Extensions.MomtComponentName);
			return monitorDefinition;
		}

		private static ResponderDefinition CreateMapiMTCheckMemoryRestartResponder(MonitorIdentity monitorIdentity, ServiceHealthStatus targetState)
		{
			string targetResource = string.Format("{0}_{1}_{2}", "MSExchangeRPC", "MSExchangeRpcProxyAppPool", targetState);
			ResponderIdentity responderIdentity = monitorIdentity.CreateResponderIdentity("Restart", targetResource);
			string name = responderIdentity.Name;
			string alertMask = monitorIdentity.GetAlertMask();
			string serviceName = responderIdentity.ServiceName;
			return MapiMTCheckMemoryRestartResponder.CreateDefinition(name, alertMask, targetState, 15, 120, 0, false, DumpMode.FullDump, null, 15.0, 0, serviceName, null, true, true, "Dag").Apply(responderIdentity);
		}

		protected override ProbeIdentity GetDeepTest()
		{
			return OutlookConnectivity.DeepTest;
		}

		protected override void AddDeepTest(IEnumerable<MailboxDatabaseInfo> databases, ProbeIdentity baseProbeIdentity, Func<ProbeDefinition, MailboxDatabaseInfo, ProbeDefinition> probeModifier)
		{
			if (base.CheckMonitoringAccountsAvailable<MailboxDatabaseInfo>(databases, baseProbeIdentity, Strings.NoBackendMonitoringAccountsAvailable))
			{
				int numberOfResources = databases.Count<MailboxDatabaseInfo>();
				base.AddForEachDatabaseForScheduledAndOnDemandExecution(databases, baseProbeIdentity, (ProbeIdentity probeIdentity, MailboxDatabaseInfo dbInfo) => OutlookDiscovery.CreateProbe<LocalProbe.DeepTest>(probeIdentity).ConfigureDeepTest(numberOfResources).SuppressOnFreshBootUntilServiceIsStarted("MSExchangeRPC").ApplyModifier(probeModifier, dbInfo).ConfigureAuthenticationForBackendProbe(dbInfo, this.UseServerAuthforBackEndProbes));
				base.CreateRelatedWorkItems<ProbeIdentity>(baseProbeIdentity, delegate(ProbeIdentity probeIdentity)
				{
					base.AddMonitorAndResponders(base.CreatePercentSuccessMonitor(probeIdentity, Extensions.ProtocolDeepTestAvailabilityThreshold, (int)(Extensions.ProtocolDeepTestFailureDetectionTime.TotalSeconds / (double)Extensions.ProtocolDeepTestProbeIntervalInSeconds * (100.0 - Extensions.ProtocolDeepTestAvailabilityThreshold) / 100.0 / 2.0), Extensions.ProtocolDeepTestFailureDetectionTime, Extensions.ProtocolDeepTestMonitorRecurrenceInterval).ConfigureMonitorStateTransitions(new MonitorStateTransition[]
					{
						new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, TimeSpan.FromMinutes(0.0))
					}));
				});
			}
		}

		private void AddServiceTest()
		{
			base.CreateRelatedWorkItems<ProbeIdentity>(ProbeIdentity.Create(ExchangeComponent.OutlookProtocol, ProbeType.Service, null, "MSExchangeRPC"), delegate(ProbeIdentity probeIdentity)
			{
				base.AddWorkDefinition<ProbeDefinition>(OutlookDiscovery.CreateProbe<GenericServiceProbe>(probeIdentity).ConfigureSelfTest());
			});
		}

		protected override void AddSelfTest(IEnumerable<MailboxDatabaseInfo> databases)
		{
			ProbeIdentity rpcSelfTest = OutlookConnectivity.RpcSelfTest;
			if (base.CheckMonitoringAccountsAvailable<MailboxDatabaseInfo>(databases, rpcSelfTest, Strings.NoBackendMonitoringAccountsAvailable))
			{
				MailboxDatabaseInfo resource = (from db in databases
				orderby db.MonitoringAccount
				select db).First<MailboxDatabaseInfo>();
				base.CreateRelatedWorkItems<ProbeIdentity, MailboxDatabaseInfo>(rpcSelfTest, resource, delegate(ProbeIdentity probeIdentity, MailboxDatabaseInfo dbInfo)
				{
					base.AddWorkDefinition<ProbeDefinition>(OutlookDiscovery.CreateProbe<LocalProbe.SelfTest>(probeIdentity).ConfigureSelfTest().TargetPrimaryMailbox(dbInfo).ConfigureAuthenticationForBackendProbe(dbInfo, this.UseServerAuthforBackEndProbes).SuppressOnFreshBootUntilServiceIsStarted("MSExchangeRPC"));
					base.AddMonitorAndResponders(this.CreateCheckMemoryRestartSnappyMonitor(probeIdentity).LimitRespondersTo(new ServiceHealthStatus[]
					{
						ServiceHealthStatus.Degraded,
						ServiceHealthStatus.Unrecoverable
					}));
				});
			}
		}

		protected override ProbeIdentity GetMailboxCTPTest()
		{
			return OutlookConnectivity.Ctp;
		}

		protected override ProbeIdentity GetArchiveCTPTest()
		{
			return OutlookConnectivity.ArchiveCtp;
		}

		protected override void AddCtp(IEnumerable<MailboxDatabaseInfo> databases, ProbeIdentity baseProbeIdentity, Func<ProbeDefinition, MailboxDatabaseInfo, ProbeDefinition> probeModifier, Func<MonitorDefinition, MonitorDefinition> monitorModifier)
		{
			List<AutodiscoverRpcHttpSettings> rpcHttpServiceSettings = DirectoryAccessor.Instance.GetRpcHttpServiceSettings();
			if (base.CheckMonitoringAccountsAvailable<MailboxDatabaseInfo>(databases, baseProbeIdentity, Strings.NoCafeMonitoringAccountsAvailable) && base.CheckRpcProxyAuthenticationSettingsAvailable(rpcHttpServiceSettings, baseProbeIdentity))
			{
				int numberOfResources = databases.Count<MailboxDatabaseInfo>();
				AutodiscoverRpcHttpSettings settings = rpcHttpServiceSettings.First<AutodiscoverRpcHttpSettings>();
				base.AddForEachDatabaseForScheduledAndOnDemandExecution(databases, baseProbeIdentity, (ProbeIdentity probeIdentity, MailboxDatabaseInfo dbInfo) => OutlookDiscovery.CreateProbe<LocalProbe.Ctp>(probeIdentity).ConfigureCtp(numberOfResources).ConfigureCtpAuthenticationMethod(settings).ApplyModifier(probeModifier, dbInfo).AuthenticateAsUser(dbInfo));
				base.CreateRelatedWorkItems<ProbeIdentity>(baseProbeIdentity, delegate(ProbeIdentity probeIdentity)
				{
					this.AddMonitorAndResponders(this.CreateChunkingMonitor(probeIdentity, numberOfResources, Extensions.CtpFailureDetectionTime).LimitRespondersTo(new ServiceHealthStatus[]
					{
						ServiceHealthStatus.Unrecoverable
					}).ApplyModifier(monitorModifier));
				});
			}
		}

		protected override ResponderDefinition[] CreateProtocolSpecificResponders(MonitorIdentity monitor)
		{
			return new ResponderDefinition[]
			{
				OutlookDiscovery.CreateEscalateResponder(monitor),
				OutlookRpcDiscovery.CreateMapiMTCheckMemoryRestartResponder(monitor, ServiceHealthStatus.Degraded)
			};
		}

		private const string RpcClientAccessServiceName = "MSExchangeRPC";

		private static bool didDiscoveryExecuteSuccessfully = false;

		private static MonitorStateTransition[] ProtocolMonitorStateTransitions = new MonitorStateTransition[]
		{
			new MonitorStateTransition(ServiceHealthStatus.Degraded, TimeSpan.Zero),
			new MonitorStateTransition(ServiceHealthStatus.Degraded1, TimeSpan.FromSeconds(10.0)),
			new MonitorStateTransition(ServiceHealthStatus.Degraded2, OutlookDiscovery.FullDumpTimeout.Add(TimeSpan.FromMinutes(1.0))),
			new MonitorStateTransition(ServiceHealthStatus.Unhealthy, TimeSpan.FromMinutes(5.0)),
			new MonitorStateTransition(ServiceHealthStatus.Unhealthy1, TimeSpan.FromMinutes(10.0)),
			new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, TimeSpan.FromMinutes(20.0))
		};
	}
}
