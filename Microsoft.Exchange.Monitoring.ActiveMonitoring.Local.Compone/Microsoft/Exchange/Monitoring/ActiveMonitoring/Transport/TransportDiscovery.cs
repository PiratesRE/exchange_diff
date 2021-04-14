using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.MailFlowTestHelper;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Transport
{
	public sealed class TransportDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.ExchangeServerRoleEndpoint == null)
			{
				return;
			}
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 47, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Transport\\Discovery\\TransportDiscovery.cs");
			if (topologyConfigurationSession == null)
			{
				return;
			}
			if (!CrossPremiseTestMailFlowHelper.IsCrossPremise(topologyConfigurationSession))
			{
				return;
			}
			if (instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled && !Datacenter.IsMicrosoftHostedOnly(true))
			{
				TransportDiscovery.workitems.Add(new TestMailflow());
			}
			TransportDiscovery.workitems.ForEach(delegate(IWorkItem wi)
			{
				wi.Initialize(base.Definition, base.Broker, base.TraceContext);
			});
		}

		private static List<IWorkItem> workitems = new List<IWorkItem>();
	}
}
