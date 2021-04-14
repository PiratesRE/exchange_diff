using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Data.LoadMetrics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;
using Microsoft.Exchange.MailboxLoadBalance.SoftDeletedRemoval;

namespace Microsoft.Exchange.MailboxLoadBalance.QueueProcessing
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SoftDeletedCleanUpRequest : BaseRequest, ILoadEntityVisitor
	{
		public SoftDeletedCleanUpRequest(LoadContainer forestTopology, IClientFactory clientFactory, int threshold, ILogger logger)
		{
			this.forestTopology = forestTopology;
			this.clientFactory = clientFactory;
			this.threshold = threshold;
			this.logger = logger;
		}

		public bool Visit(LoadContainer container)
		{
			if (container.ContainerType != ContainerType.Database)
			{
				return true;
			}
			LoadMetric instance = PhysicalSize.Instance;
			ByteQuantifiedSize sizeMetric = container.MaximumLoad.GetSizeMetric(instance);
			ByteQuantifiedSize byteQuantifiedSize = sizeMetric * this.threshold / 100;
			ByteQuantifiedSize sizeMetric2 = container.ConsumedLoad.GetSizeMetric(instance);
			this.logger.LogVerbose("Database {0} has maximum physical size {1}, SoftDeletedThreshold of {2}, target size {3}, consumed physical size {4}", new object[]
			{
				container,
				sizeMetric,
				this.threshold,
				byteQuantifiedSize,
				sizeMetric2
			});
			if (sizeMetric2 >= byteQuantifiedSize)
			{
				SoftDeletedDatabaseCleanupRequest request = new SoftDeletedDatabaseCleanupRequest(this.clientFactory, (DirectoryDatabase)container.DirectoryObject, byteQuantifiedSize);
				base.Queue.EnqueueRequest(request);
			}
			return false;
		}

		public bool Visit(LoadEntity entity)
		{
			return false;
		}

		protected override void ProcessRequest()
		{
			this.forestTopology.Accept(this);
		}

		private readonly IClientFactory clientFactory;

		private readonly LoadContainer forestTopology;

		private readonly int threshold;

		private readonly ILogger logger;
	}
}
