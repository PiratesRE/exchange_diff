using System;
using System.Collections.Generic;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RegularLoadBalancingExtractorFactory : TopologyExtractorFactory
	{
		public RegularLoadBalancingExtractorFactory(Band[] bands, IList<Guid> nonMovableOrganizations, ILogger logger) : base(logger)
		{
			AnchorUtil.ThrowOnNullArgument(bands, "bands");
			AnchorUtil.ThrowOnNullArgument(nonMovableOrganizations, "nonMovableOrganizations");
			this.Bands = bands;
			this.NonMovableOrganizations = nonMovableOrganizations;
		}

		private protected Band[] Bands { protected get; private set; }

		private protected IList<Guid> NonMovableOrganizations { protected get; private set; }

		protected override TopologyExtractor CreateContainerParentExtractor(DirectoryContainerParent container)
		{
			return new ParentContainerExtractor(container, this, base.Logger);
		}

		protected override TopologyExtractor CreateMailboxExtractor(DirectoryMailbox mailbox)
		{
			return new MailboxEntityExtractor(mailbox, this, this.Bands);
		}

		protected override TopologyExtractor CreateDatabaseExtractor(DirectoryDatabase database)
		{
			return new FullMailboxInfoDatabaseTopologyExtractor(database, this, this.NonMovableOrganizations);
		}

		protected override TopologyExtractor CreateServerExtractor(DirectoryServer directoryServer)
		{
			return ParallelParentContainerExtractor.CreateExtractor(directoryServer, this, base.Logger);
		}
	}
}
