using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;

namespace Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CachingTopologyExtractor : TopologyExtractor
	{
		public CachingTopologyExtractor(TopologyExtractorFactory topologyExtractorFactory, DirectoryObject directoryObject, ILogger logger, TopologyExtractor topologyExtractor, ITimer timer) : base(directoryObject, topologyExtractorFactory)
		{
			AnchorUtil.ThrowOnNullArgument(logger, "logger");
			AnchorUtil.ThrowOnNullArgument(timer, "timer");
			this.directoryObject = directoryObject;
			this.logger = logger;
			this.timer = timer;
			this.topologyExtractor = (topologyExtractor ?? base.ExtractorFactory.GetExtractor(this.directoryObject));
			this.timer.SetAction(new Action(this.RefreshCachedValue), true);
		}

		public override LoadEntity ExtractEntity()
		{
			this.logger.LogVerbose("Retrieving cached entity for {0}.", new object[]
			{
				this.directoryObject
			});
			this.EnsureCacheIsInitialized();
			return this.cachedValue;
		}

		public override LoadContainer ExtractTopology()
		{
			this.logger.LogVerbose("Retrieving cached topology for {0}.", new object[]
			{
				this.directoryObject
			});
			this.EnsureCacheIsInitialized();
			return this.cachedValue;
		}

		private void EnsureCacheIsInitialized()
		{
			while (this.cachedValue == null)
			{
				this.timer.WaitExecution(TimeSpan.FromMinutes(5.0));
			}
		}

		private void RefreshCachedValue()
		{
			using (OperationTracker.Create(this.logger, "Refreshing topology for {0}.", new object[]
			{
				this.directoryObject
			}))
			{
				this.cachedValue = this.topologyExtractor.ExtractTopology();
				ExAssert.RetailAssert(this.cachedValue != null, "ExtractTopology for directoryObject: {0} should never return null.  TopologyExtractor: {1}", new object[]
				{
					this.directoryObject,
					this.topologyExtractor
				});
			}
		}

		private readonly DirectoryObject directoryObject;

		private readonly ILogger logger;

		private readonly ITimer timer;

		private readonly TopologyExtractor topologyExtractor;

		private LoadContainer cachedValue;
	}
}
