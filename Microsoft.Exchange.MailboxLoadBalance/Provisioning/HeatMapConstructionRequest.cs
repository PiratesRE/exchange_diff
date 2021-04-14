using System;
using System.Linq;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.QueueProcessing;
using Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors;

namespace Microsoft.Exchange.MailboxLoadBalance.Provisioning
{
	internal abstract class HeatMapConstructionRequest : BaseRequest
	{
		protected HeatMapConstructionRequest(LoadBalanceAnchorContext context)
		{
			this.ServiceContext = context;
		}

		public LoadContainer Topology { get; private set; }

		private protected LoadBalanceAnchorContext ServiceContext { protected get; private set; }

		public void UpdateBands(Band[] newBands)
		{
			if (this.NeedToRebuildForBands(newBands))
			{
				this.Topology = null;
			}
		}

		protected abstract LoadContainer BuildTopology(TopologyExtractorFactoryContext topologyExtractorContext);

		protected bool NeedToRebuildForBands(Band[] newBands)
		{
			return this.bands != null && newBands.Any((Band band) => !this.bands.Contains(band));
		}

		protected override void ProcessRequest()
		{
			this.bands = this.ServiceContext.GetActiveBands();
			LoadContainer loadContainer = this.BuildTopology(this.GetTopologyExtractorContext(this.bands));
			if (loadContainer != null)
			{
				this.ServiceContext.Logger.LogVerbose("Refreshed topology for {0}, new timestamp is {1}.", new object[]
				{
					loadContainer.DirectoryObjectIdentity,
					loadContainer.DataRetrievedTimestampUtc
				});
			}
			this.Topology = loadContainer;
		}

		protected TopologyExtractorFactoryContext GetTopologyExtractorContext(Band[] bandsToUse)
		{
			return this.ServiceContext.TopologyExtractorFactoryContextPool.GetContext(this.ServiceContext.ClientFactory, bandsToUse, LoadBalanceUtils.GetNonMovableOrgsList(this.ServiceContext.Settings), this.ServiceContext.Logger);
		}

		private Band[] bands;
	}
}
