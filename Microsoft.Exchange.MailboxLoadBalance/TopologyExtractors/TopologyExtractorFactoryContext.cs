using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;

namespace Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors
{
	internal class TopologyExtractorFactoryContext
	{
		public TopologyExtractorFactoryContext(IClientFactory clientFactory, Band[] bands, IList<Guid> nonMovableOrgs, ILogger logger)
		{
			this.clientFactory = clientFactory;
			this.bands = (bands ?? Array<Band>.Empty);
			this.nonMovableOrgs = nonMovableOrgs;
			this.Logger = logger;
			this.localFactory = new Lazy<TopologyExtractorFactory>(new Func<TopologyExtractorFactory>(this.CreateLocalLoadBalancingFactory));
		}

		private protected ILogger Logger { protected get; private set; }

		public virtual TopologyExtractorFactory GetEntitySelectorFactory()
		{
			return this.CreateEntitySelectorFactory();
		}

		public virtual TopologyExtractorFactory GetLoadBalancingCentralFactory()
		{
			return new CentralServerLoadBalancingExtractorFactory(this.clientFactory, this.bands, this.nonMovableOrgs, this.Logger);
		}

		public virtual TopologyExtractorFactory GetLoadBalancingLocalFactory(bool ignoreCache = false)
		{
			if (LoadBalanceADSettings.Instance.Value.LocalCacheRefreshPeriod == TimeSpan.Zero || ignoreCache)
			{
				return this.CreateRegularLoadBalancingLocalFactory();
			}
			return this.localFactory.Value;
		}

		public bool Matches(IClientFactory requestedClientFactory, IEnumerable<Band> requestedBands, IEnumerable<Guid> requestedNonMovableOrgs, ILogger requestedLogger)
		{
			if (!object.Equals(this.clientFactory, requestedClientFactory))
			{
				return false;
			}
			Band[] array = (requestedBands == null) ? Array<Band>.Empty : ((requestedBands as Band[]) ?? requestedBands.ToArray<Band>());
			if (this.bands.IsNullOrEmpty<Band>() && !array.IsNullOrEmpty<Band>())
			{
				return false;
			}
			if (!array.IsNullOrEmpty<Band>())
			{
				if (!(from band in this.bands
				orderby band.Name
				select band).SequenceEqual(from band in array
				orderby band.Name
				select band))
				{
					return false;
				}
			}
			return (from guid in this.nonMovableOrgs
			orderby guid
			select guid).SequenceEqual(from guid in requestedNonMovableOrgs
			orderby guid
			select guid) && object.Equals(this.Logger, requestedLogger);
		}

		protected virtual TopologyExtractorFactory CreateEntitySelectorFactory()
		{
			return new LocalLoadBalancingWithEntitiesExtractorFactory(this.bands, this.nonMovableOrgs, this.Logger);
		}

		protected virtual TopologyExtractorFactory CreateLocalLoadBalancingFactory()
		{
			return new CachingTopologyExtractorFactory(this.Logger, this.CreateRegularLoadBalancingLocalFactory());
		}

		protected virtual TopologyExtractorFactory CreateRegularLoadBalancingLocalFactory()
		{
			return new RegularLoadBalancingExtractorFactory(this.bands, this.nonMovableOrgs, this.Logger);
		}

		private readonly Band[] bands;

		private readonly IClientFactory clientFactory;

		private readonly Lazy<TopologyExtractorFactory> localFactory;

		private readonly IList<Guid> nonMovableOrgs;
	}
}
