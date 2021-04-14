using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors;

namespace Microsoft.Exchange.MailboxLoadBalance.Provisioning
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ForestHeatMapConstructionRequest : HeatMapConstructionRequest
	{
		public ForestHeatMapConstructionRequest(LoadBalanceAnchorContext context) : base(context)
		{
		}

		protected override LoadContainer BuildTopology(TopologyExtractorFactoryContext topologyExtractorContext)
		{
			TopologyExtractorFactory loadBalancingCentralFactory = topologyExtractorContext.GetLoadBalancingCentralFactory();
			TopologyExtractor extractor = loadBalancingCentralFactory.GetExtractor(base.ServiceContext.Directory.GetLocalForest());
			LoadContainer loadContainer = extractor.ExtractTopology();
			ExAssert.RetailAssert(loadContainer != null, "Extracted toplogy for the forest should never be null.");
			return loadContainer;
		}
	}
}
