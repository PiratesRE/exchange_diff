using System;
using System.Collections.Generic;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LocalLoadBalancingWithEntitiesExtractorFactory : RegularLoadBalancingExtractorFactory
	{
		public LocalLoadBalancingWithEntitiesExtractorFactory(Band[] bands, IList<Guid> nonMovableOrgs, ILogger logger) : base(bands, nonMovableOrgs, logger)
		{
		}

		protected override TopologyExtractor CreateServerExtractor(DirectoryServer directoryServer)
		{
			return ParallelParentContainerExtractor.CreateExtractor(directoryServer, this, base.Logger);
		}

		protected override TopologyExtractor CreateDatabaseExtractor(DirectoryDatabase database)
		{
			return new DetailedMailboxInfoDatabaseExtractor(database, this, base.NonMovableOrganizations);
		}
	}
}
