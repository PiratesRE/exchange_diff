using System;
using System.Collections.Concurrent;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;

namespace Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CachingTopologyExtractorFactory : TopologyExtractorFactory
	{
		public CachingTopologyExtractorFactory(ILogger logger, TopologyExtractorFactory realFactory) : base(logger)
		{
			this.realFactory = realFactory;
			this.extractors = new ConcurrentDictionary<Guid, TopologyExtractor>();
		}

		public override TopologyExtractor GetExtractor(DirectoryObject directoryObject)
		{
			if (directoryObject is DirectoryMailbox)
			{
				return this.realFactory.GetExtractor(directoryObject);
			}
			TopologyExtractor value;
			if (!this.extractors.TryGetValue(directoryObject.Guid, out value))
			{
				TopologyExtractor extractor = this.realFactory.GetExtractor(directoryObject);
				if (extractor != null)
				{
					value = new CachingTopologyExtractor(this.realFactory, directoryObject, base.Logger, extractor, SimpleTimer.CreateTimer(LoadBalanceADSettings.Instance.Value.LocalCacheRefreshPeriod));
				}
				this.extractors.TryAdd(directoryObject.Guid, value);
			}
			return this.extractors[directoryObject.Guid];
		}

		private readonly ConcurrentDictionary<Guid, TopologyExtractor> extractors;

		private readonly TopologyExtractorFactory realFactory;
	}
}
