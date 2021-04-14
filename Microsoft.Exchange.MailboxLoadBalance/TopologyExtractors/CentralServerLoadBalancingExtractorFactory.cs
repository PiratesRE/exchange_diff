using System;
using System.Collections.Generic;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;

namespace Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CentralServerLoadBalancingExtractorFactory : RegularLoadBalancingExtractorFactory
	{
		public CentralServerLoadBalancingExtractorFactory(IClientFactory clientFactory, Band[] bands, IList<Guid> nonMovableOrgs, ILogger logger) : base(bands, nonMovableOrgs, logger)
		{
			this.clientFactory = clientFactory;
		}

		protected override TopologyExtractor CreateDagExtractor(DirectoryDatabaseAvailabilityGroup directoryDag)
		{
			return ParallelParentContainerExtractor.CreateExtractor(directoryDag, this, base.Logger);
		}

		protected override TopologyExtractor CreateServerExtractor(DirectoryServer directoryServer)
		{
			return new RemoteServerTopologyExtractor(directoryServer, this, base.Bands, this.clientFactory);
		}

		private readonly IClientFactory clientFactory;
	}
}
