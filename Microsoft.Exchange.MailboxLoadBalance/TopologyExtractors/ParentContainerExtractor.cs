using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ParentContainerExtractor : TopologyExtractor
	{
		public ParentContainerExtractor(DirectoryContainerParent directoryObject, TopologyExtractorFactory extractorFactory, ILogger logger) : base(directoryObject, extractorFactory)
		{
			this.logger = logger;
		}

		public override LoadContainer ExtractTopology()
		{
			this.logger.LogInformation("Retrieving topology for parent object {0}", new object[]
			{
				base.DirectoryObject.Identity
			});
			LoadContainer loadContainer = new LoadContainer(base.DirectoryObject, ContainerType.Generic);
			foreach (DirectoryObject directoryObject in ((DirectoryContainerParent)base.DirectoryObject).Children)
			{
				new ChildContainerExtractor(directoryObject, loadContainer, this.logger, base.ExtractorFactory.GetExtractor(directoryObject)).ExtractContainer();
			}
			return loadContainer;
		}

		private readonly ILogger logger;
	}
}
