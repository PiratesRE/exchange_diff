using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.LoadBalance;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;

namespace Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RemoteServerTopologyExtractor : TopologyExtractor
	{
		public RemoteServerTopologyExtractor(DirectoryObject directoryObject, TopologyExtractorFactory extractorFactory, Band[] bands, IClientFactory clientFactory) : base(directoryObject, extractorFactory)
		{
			this.bands = bands;
			this.clientFactory = clientFactory;
		}

		public override LoadContainer ExtractTopology()
		{
			LoadContainer localServerData;
			using (ILoadBalanceService loadBalanceClientForServer = this.clientFactory.GetLoadBalanceClientForServer((DirectoryServer)base.DirectoryObject, true))
			{
				localServerData = loadBalanceClientForServer.GetLocalServerData(this.bands);
			}
			return localServerData;
		}

		private readonly Band[] bands;

		private readonly IClientFactory clientFactory;
	}
}
