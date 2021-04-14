using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	internal class Directory : IDirectory
	{
		public ADSite[] GetADSites()
		{
			ADSite[] sites = null;
			ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 30, "GetADSites", "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\RpcHttp\\Directory.cs");
				ADPagedReader<ADSite> adpagedReader = topologyConfigurationSession.FindPaged<ADSite>(null, QueryScope.SubTree, null, null, 0);
				sites = adpagedReader.ReadAllPages();
			});
			return sites;
		}

		public ClientAccessArray[] GetClientAccessArrays()
		{
			ClientAccessArray[] arrays = null;
			ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 50, "GetClientAccessArrays", "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\RpcHttp\\Directory.cs");
				ADPagedReader<ClientAccessArray> adpagedReader = topologyConfigurationSession.FindPaged<ClientAccessArray>(null, QueryScope.SubTree, ClientAccessArray.PriorTo15ExchangeObjectVersionFilter, null, 0);
				arrays = adpagedReader.ReadAllPages();
			});
			return arrays;
		}

		public Server[] GetServers()
		{
			Server[] servers = null;
			ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 70, "GetServers", "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\RpcHttp\\Directory.cs");
				ADPagedReader<Server> adpagedReader = topologyConfigurationSession.FindPaged<Server>(null, QueryScope.SubTree, null, null, 0);
				servers = adpagedReader.ReadAllPages();
			});
			return servers;
		}
	}
}
