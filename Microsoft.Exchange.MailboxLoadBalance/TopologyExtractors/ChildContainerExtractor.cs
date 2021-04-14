using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;

namespace Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ChildContainerExtractor
	{
		public ChildContainerExtractor(DirectoryObject child, LoadContainer loadContainer, ILogger logger, TopologyExtractor extractor)
		{
			this.child = child;
			this.loadContainer = loadContainer;
			this.logger = logger;
			this.childExtractor = extractor;
		}

		public void ExtractContainer()
		{
			IOperationRetryManager operationRetryManager = LoadBalanceOperationRetryManager.Create(this.logger);
			operationRetryManager.TryRun(new Action(this.ExtractContainerAndAddToParent));
		}

		private void ExtractContainerAndAddToParent()
		{
			this.logger.LogInformation("Retrieving topology for child object {0} using extractor {1}", new object[]
			{
				this.child.Identity,
				this.childExtractor
			});
			LoadContainer loadContainer = this.childExtractor.ExtractTopology();
			this.loadContainer.AddChild(loadContainer);
			this.loadContainer.ConsumedLoad += loadContainer.ConsumedLoad;
			this.loadContainer.MaximumLoad += loadContainer.MaximumLoad;
			this.loadContainer.CommittedLoad += loadContainer.CommittedLoad;
		}

		private readonly DirectoryObject child;

		private readonly LoadContainer loadContainer;

		private readonly ILogger logger;

		private readonly TopologyExtractor childExtractor;
	}
}
