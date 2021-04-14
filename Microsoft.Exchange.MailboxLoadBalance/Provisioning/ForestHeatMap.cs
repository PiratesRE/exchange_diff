using System;
using System.Threading;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Provisioning
{
	internal class ForestHeatMap : HeatMap
	{
		public ForestHeatMap(LoadBalanceAnchorContext context)
		{
			this.context = context;
		}

		public override LoadContainer GetLoadTopology()
		{
			ForestHeatMapConstructionRequest forestHeatMapConstructionRequest = new ForestHeatMapConstructionRequest(this.context);
			this.context.QueueManager.MainProcessingQueue.EnqueueRequest(forestHeatMapConstructionRequest);
			forestHeatMapConstructionRequest.WaitExecutionAndThrowOnFailure(Timeout.InfiniteTimeSpan);
			return forestHeatMapConstructionRequest.Topology;
		}

		public override void UpdateBands(Band[] bands)
		{
		}

		private readonly LoadBalanceAnchorContext context;
	}
}
