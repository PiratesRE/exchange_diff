using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;

namespace Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors
{
	internal class TopologyExtractorFactoryContextPool
	{
		public virtual TopologyExtractorFactoryContext GetContext(IClientFactory clientFactory, Band[] bands, IList<Guid> nonMovableOrgs, ILogger logger)
		{
			AnchorUtil.ThrowOnNullArgument(clientFactory, "clientFactory");
			TimeSpan instanceCacheExpirationPeriod = LoadBalanceADSettings.Instance.Value.IdleRunDelay + LoadBalanceADSettings.Instance.Value.IdleRunDelay;
			TopologyExtractorFactoryContext context2;
			lock (this.factoryInstances.GetSyncRoot<TopologyExtractorFactoryContextPool.ContextInstanceRecord>())
			{
				this.factoryInstances.RemoveAll((TopologyExtractorFactoryContextPool.ContextInstanceRecord factoryContext) => factoryContext.IsExpired(instanceCacheExpirationPeriod));
				TopologyExtractorFactoryContextPool.ContextInstanceRecord contextInstanceRecord = this.factoryInstances.FirstOrDefault((TopologyExtractorFactoryContextPool.ContextInstanceRecord context) => context.Matches(clientFactory, bands, nonMovableOrgs, logger));
				if (contextInstanceRecord == null)
				{
					contextInstanceRecord = new TopologyExtractorFactoryContextPool.ContextInstanceRecord(new TopologyExtractorFactoryContext(clientFactory, bands, nonMovableOrgs, logger));
					this.factoryInstances.Add(contextInstanceRecord);
				}
				contextInstanceRecord.UpdateLastAccessTimestamp();
				context2 = contextInstanceRecord.Context;
			}
			return context2;
		}

		private readonly List<TopologyExtractorFactoryContextPool.ContextInstanceRecord> factoryInstances = new List<TopologyExtractorFactoryContextPool.ContextInstanceRecord>();

		private class ContextInstanceRecord
		{
			public ContextInstanceRecord(TopologyExtractorFactoryContext context)
			{
				this.Context = context;
			}

			public TopologyExtractorFactoryContext Context { get; private set; }

			private DateTime LastAccess { get; set; }

			public bool IsExpired(TimeSpan expirationTimeout)
			{
				return this.LastAccess + expirationTimeout < TimeProvider.UtcNow;
			}

			public bool Matches(IClientFactory clientFactory, Band[] bands, IList<Guid> nonMovableOrgs, ILogger logger)
			{
				return this.Context.Matches(clientFactory, bands, nonMovableOrgs, logger);
			}

			public void UpdateLastAccessTimestamp()
			{
				this.LastAccess = TimeProvider.UtcNow;
			}
		}
	}
}
