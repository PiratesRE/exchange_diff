using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;

namespace Microsoft.Exchange.MailboxLoadBalance.QueueProcessing
{
	internal class RebalancingRequestMoveStarter : IRebalancingRequestProcessor
	{
		public RebalancingRequestMoveStarter(IClientFactory clientFactory, ILogger logger, IRequestQueueManager queueManager)
		{
			this.clientFactory = clientFactory;
			this.logger = logger;
			this.queueManager = queueManager;
		}

		public void ProcessRebalanceRequest(BandMailboxRebalanceData rebalanceRequest)
		{
			AnchorUtil.ThrowOnNullArgument(rebalanceRequest, "rebalanceRequest");
			BandRebalanceRequest request = new BandRebalanceRequest(rebalanceRequest, this.clientFactory, this.logger);
			LoadContainer sourceDatabase = rebalanceRequest.SourceDatabase;
			IRequestQueue processingQueue = this.GetProcessingQueue(sourceDatabase);
			processingQueue.EnqueueRequest(request);
		}

		private IRequestQueue GetProcessingQueue(LoadContainer container)
		{
			if (container.Parent == null)
			{
				return this.queueManager.GetProcessingQueue(container);
			}
			if (container.DirectoryObjectIdentity.ObjectType == DirectoryObjectType.DatabaseAvailabilityGroup)
			{
				return this.queueManager.GetProcessingQueue(container);
			}
			return this.GetProcessingQueue(container.Parent);
		}

		private readonly IClientFactory clientFactory;

		private readonly ILogger logger;

		private readonly IRequestQueueManager queueManager;
	}
}
