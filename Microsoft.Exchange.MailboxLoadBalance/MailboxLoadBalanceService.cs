using System;
using System.Collections.Generic;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.QueueProcessing;
using Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MailboxLoadBalanceService
	{
		public MailboxLoadBalanceService(LoadBalanceAnchorContext serviceContext)
		{
			AnchorUtil.ThrowOnNullArgument(serviceContext, "serviceContext");
			this.serviceContext = serviceContext;
		}

		public LoadContainer GetDatabaseData(Guid databaseGuid, bool includeMailboxes)
		{
			TopologyExtractorFactoryContext topologyExtractorFactoryContext = this.serviceContext.GetTopologyExtractorFactoryContext();
			TopologyExtractorFactory topologyExtractorFactory = includeMailboxes ? topologyExtractorFactoryContext.GetEntitySelectorFactory() : topologyExtractorFactoryContext.GetLoadBalancingLocalFactory(false);
			DirectoryDatabase database = this.serviceContext.Directory.GetDatabase(databaseGuid);
			return topologyExtractorFactory.GetExtractor(database).ExtractTopology();
		}

		public void MoveMailboxes(BandMailboxRebalanceData rebalanceData)
		{
			IList<Guid> nonMovableOrgsList = LoadBalanceUtils.GetNonMovableOrgsList(this.serviceContext.Settings);
			MailboxRebalanceRequest request = new MailboxRebalanceRequest(rebalanceData, this.serviceContext, nonMovableOrgsList);
			this.serviceContext.QueueManager.GetProcessingQueue(rebalanceData.SourceDatabase).EnqueueRequest(request);
		}

		private readonly LoadBalanceAnchorContext serviceContext;
	}
}
