using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Data.LoadMetrics;
using Microsoft.Exchange.MailboxLoadBalance.QueueProcessing;
using Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AutomaticLoadBalancer
	{
		public AutomaticLoadBalancer(LoadBalanceAnchorContext anchorContext)
		{
			this.context = anchorContext;
		}

		private protected LoadBalanceAnchorContext Context { protected get; private set; }

		public void LoadBalanceForest()
		{
			ILoadBalance loadBalancer = this.context.CreateLoadBalancer(this.context.Logger);
			this.LoadBalanceForest(loadBalancer, true, this.context.Logger, Timeout.InfiniteTimeSpan);
		}

		public IList<BandMailboxRebalanceData> LoadBalanceForest(ILoadBalance loadBalancer, bool startMoves, ILogger logger, TimeSpan timeout)
		{
			RebalancingRequestMoveStarter moveStarter = new RebalancingRequestMoveStarter(this.context.ClientFactory, logger, this.context.QueueManager);
			LoadContainer loadTopology = this.context.HeatMap.GetLoadTopology();
			if (this.context.Settings.SoftDeletedCleanupEnabled)
			{
				int softDeletedCleanupThreshold = this.context.Settings.SoftDeletedCleanupThreshold;
				SoftDeletedCleanUpRequest request = new SoftDeletedCleanUpRequest(loadTopology, this.context.ClientFactory, softDeletedCleanupThreshold, logger);
				this.context.QueueManager.MainProcessingQueue.EnqueueRequest(request);
			}
			long aggregateConsumedLoad = loadTopology.GetAggregateConsumedLoad(InProgressLoadBalancingMoveCount.Instance);
			if (aggregateConsumedLoad > this.context.Settings.MaximumPendingMoveCount)
			{
				logger.LogWarning("Did not start load balancing run because current number of pending moves {0} is greater than MaximumPendingMoveCount {1}", new object[]
				{
					aggregateConsumedLoad,
					this.context.Settings.MaximumPendingMoveCount
				});
				return Array<BandMailboxRebalanceData>.Empty;
			}
			ForestLoadBalanceRequest forestLoadBalanceRequest = new ForestLoadBalanceRequest(loadBalancer, startMoves, logger, moveStarter, loadTopology, new PartitionExtractor());
			this.context.QueueManager.MainProcessingQueue.EnqueueRequest(forestLoadBalanceRequest);
			forestLoadBalanceRequest.WaitExecutionAndThrowOnFailure(timeout);
			return forestLoadBalanceRequest.Results;
		}

		private readonly LoadBalanceAnchorContext context;
	}
}
