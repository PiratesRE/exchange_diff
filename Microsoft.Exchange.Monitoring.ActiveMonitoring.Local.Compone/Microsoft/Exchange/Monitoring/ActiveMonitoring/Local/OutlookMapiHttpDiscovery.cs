using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ClientAccess;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.RpcClientAccess;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	public sealed class OutlookMapiHttpDiscovery : OutlookDiscovery
	{
		internal override MonitorStateTransition[] GetProtocolMonitorStateTransitions()
		{
			return OutlookMapiHttpDiscovery.ProtocolMonitorStateTransitions;
		}

		protected override bool DidDiscoveryExecuteSuccessfully()
		{
			return OutlookMapiHttpDiscovery.didDiscoveryExecuteSuccessfully;
		}

		protected override void SetSuccessfulDiscoveryExecutionStatus()
		{
			OutlookMapiHttpDiscovery.didDiscoveryExecuteSuccessfully = true;
		}

		private static ResponderDefinition CreateCafeMapiHttpProxyRestartResponder(MonitorIdentity monitorIdentity)
		{
			string appPool = CafeProtocols.Get(HttpProtocol.Mapi).AppPool;
			return OutlookDiscovery.CreateAppPoolRestartResponder(monitorIdentity, appPool, ServiceHealthStatus.Degraded);
		}

		protected override ProbeIdentity GetDeepTest()
		{
			return OutlookConnectivity.MapiHttpDeepTest;
		}

		protected override void AddDeepTest(IEnumerable<MailboxDatabaseInfo> databases, ProbeIdentity baseProbeIdentity, Func<ProbeDefinition, MailboxDatabaseInfo, ProbeDefinition> probeModifier)
		{
			if (base.CheckMonitoringAccountsAvailable<MailboxDatabaseInfo>(databases, baseProbeIdentity, Strings.NoBackendMonitoringAccountsAvailable))
			{
				int numberOfResources = databases.Count<MailboxDatabaseInfo>();
				base.AddForEachDatabaseForScheduledAndOnDemandExecution(databases, baseProbeIdentity, (ProbeIdentity probeIdentity, MailboxDatabaseInfo dbInfo) => OutlookDiscovery.CreateProbe<LocalProbe.MapiHttpDeepTest>(probeIdentity).ConfigureDeepTest(numberOfResources).SetSecondaryEndpointAsPersonalizedServerName(dbInfo).SuppressOnFreshBootUntilServiceIsStarted("W3SVC").ApplyModifier(probeModifier, dbInfo).ConfigureAuthenticationForBackendProbe(dbInfo, this.UseServerAuthforBackEndProbes));
				base.CreateRelatedWorkItems<ProbeIdentity>(baseProbeIdentity, delegate(ProbeIdentity probeIdentity)
				{
					base.AddMonitorAndResponders(base.CreatePercentSuccessMonitor(probeIdentity, Extensions.ProtocolDeepTestAvailabilityThreshold, (int)(Extensions.ProtocolDeepTestFailureDetectionTime.TotalSeconds / (double)Extensions.ProtocolDeepTestProbeIntervalInSeconds * (100.0 - Extensions.ProtocolDeepTestAvailabilityThreshold) / 100.0 / 2.0), Extensions.ProtocolDeepTestFailureDetectionTime, Extensions.ProtocolDeepTestMonitorRecurrenceInterval).LimitRespondersTo(new ServiceHealthStatus[]
					{
						ServiceHealthStatus.Degraded1,
						ServiceHealthStatus.Unrecoverable
					}));
				});
			}
		}

		protected override void AddSelfTest(IEnumerable<MailboxDatabaseInfo> databases)
		{
			ProbeIdentity mapiHttpSelfTest = OutlookConnectivity.MapiHttpSelfTest;
			if (base.CheckMonitoringAccountsAvailable<MailboxDatabaseInfo>(databases, mapiHttpSelfTest, Strings.NoBackendMonitoringAccountsAvailable))
			{
				MailboxDatabaseInfo resource = (from db in databases
				orderby db.MonitoringAccount
				select db).First<MailboxDatabaseInfo>();
				base.CreateRelatedWorkItems<ProbeIdentity, MailboxDatabaseInfo>(mapiHttpSelfTest, resource, delegate(ProbeIdentity probeIdentity, MailboxDatabaseInfo dbInfo)
				{
					ProbeDefinition definition = base.AddWorkDefinition<ProbeDefinition>(OutlookDiscovery.CreateProbe<LocalProbe.MapiHttpSelfTest>(probeIdentity).ConfigureSelfTest().TargetPrimaryMailbox(dbInfo).ConfigureAuthenticationForBackendProbe(dbInfo, this.UseServerAuthforBackEndProbes).SetSecondaryEndpointAsPersonalizedServerName(dbInfo).SuppressOnFreshBootUntilServiceIsStarted("W3SVC"));
					base.AddMonitorAndResponders(base.CreateSnappyMonitor(definition).LimitRespondersTo(new ServiceHealthStatus[]
					{
						ServiceHealthStatus.Degraded1,
						ServiceHealthStatus.Unrecoverable
					}));
				});
			}
		}

		protected override ProbeIdentity GetMailboxCTPTest()
		{
			return OutlookConnectivity.MapiHttpCtp;
		}

		protected override ProbeIdentity GetArchiveCTPTest()
		{
			return OutlookConnectivity.MapiHttpArchiveCtp;
		}

		protected override void AddCtp(IEnumerable<MailboxDatabaseInfo> databases, ProbeIdentity baseProbeIdentity, Func<ProbeDefinition, MailboxDatabaseInfo, ProbeDefinition> probeModifier, Func<MonitorDefinition, MonitorDefinition> monitorModifier)
		{
			if (base.CheckMonitoringAccountsAvailable<MailboxDatabaseInfo>(databases, baseProbeIdentity, Strings.NoCafeMonitoringAccountsAvailable))
			{
				int numberOfResources = databases.Count<MailboxDatabaseInfo>();
				base.AddForEachDatabaseForScheduledAndOnDemandExecution(databases, baseProbeIdentity, (ProbeIdentity probeIdentity, MailboxDatabaseInfo dbInfo) => OutlookDiscovery.CreateProbe<LocalProbe.MapiHttpCtp>(probeIdentity).ConfigureCtp(numberOfResources).ForceSslCtpAuthenticationMethod().ApplyModifier(probeModifier, dbInfo).AuthenticateAsUser(dbInfo));
				base.CreateRelatedWorkItems<ProbeIdentity>(baseProbeIdentity, delegate(ProbeIdentity probeIdentity)
				{
					this.AddMonitorAndResponders(this.CreateChunkingMonitor(probeIdentity, numberOfResources, Extensions.CtpFailureDetectionTime).LimitRespondersTo(new ServiceHealthStatus[]
					{
						ServiceHealthStatus.Degraded,
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
				OutlookDiscovery.CreateAppPoolRestartResponder(monitor, "MSExchangeMapiMailboxAppPool", ServiceHealthStatus.Degraded1),
				OutlookMapiHttpDiscovery.CreateCafeMapiHttpProxyRestartResponder(monitor)
			};
		}

		private const string MapiHttpBackendAppPoolName = "MSExchangeMapiMailboxAppPool";

		private const string IisServiceName = "W3SVC";

		private static bool didDiscoveryExecuteSuccessfully = false;

		private static MonitorStateTransition[] ProtocolMonitorStateTransitions = new MonitorStateTransition[]
		{
			new MonitorStateTransition(ServiceHealthStatus.Degraded, TimeSpan.Zero),
			new MonitorStateTransition(ServiceHealthStatus.Degraded1, TimeSpan.Zero),
			new MonitorStateTransition(ServiceHealthStatus.Degraded2, OutlookDiscovery.FullDumpTimeout.Add(TimeSpan.FromMinutes(1.0))),
			new MonitorStateTransition(ServiceHealthStatus.Unhealthy1, TimeSpan.FromMinutes(12.0)),
			new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, TimeSpan.FromMinutes(15.0))
		};
	}
}
