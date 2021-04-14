using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DetailedMailboxInfoDatabaseExtractor : FullMailboxInfoDatabaseTopologyExtractor
	{
		public DetailedMailboxInfoDatabaseExtractor(DirectoryDatabase directoryObject, TopologyExtractorFactory extractorFactory, IList<Guid> nonMovableOrgs) : base(directoryObject, extractorFactory, nonMovableOrgs)
		{
		}

		protected override void AddEntityToContainer(LoadContainer databaseContainer, LoadEntity extractedEntity)
		{
			databaseContainer.AddChild(extractedEntity);
			base.AddEntityToContainer(databaseContainer, extractedEntity);
		}
	}
}
