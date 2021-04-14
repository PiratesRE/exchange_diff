using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ParallelParentContainerExtractor : TopologyExtractor
	{
		private ParallelParentContainerExtractor(DirectoryContainerParent directoryContainerObject, TopologyExtractorFactory extractorFactory, ILogger logger) : base(directoryContainerObject, extractorFactory)
		{
			this.logger = logger;
		}

		public static TopologyExtractor CreateExtractor(DirectoryContainerParent directoryContainerObject, TopologyExtractorFactory extractorFactory, ILogger logger)
		{
			if (LoadBalanceADSettings.Instance.Value.UseParallelDiscovery)
			{
				return new ParallelParentContainerExtractor(directoryContainerObject, extractorFactory, logger);
			}
			return new ParentContainerExtractor(directoryContainerObject, extractorFactory, logger);
		}

		public override LoadContainer ExtractTopology()
		{
			DirectoryContainerParent directoryContainerParent = (DirectoryContainerParent)base.DirectoryObject;
			this.logger.LogInformation("Retrieving topology for parent object {0} with {1} children.", new object[]
			{
				base.DirectoryObject.Identity,
				directoryContainerParent.Children.Count<DirectoryObject>()
			});
			LoadContainer container = new LoadContainer(base.DirectoryObject, ContainerType.Generic);
			IEnumerable<DirectoryObject> children = directoryContainerParent.Children;
			IEnumerable<ChildContainerExtractor> source = from child in children
			select new ChildContainerExtractor(child, container, this.logger, this.ExtractorFactory.GetExtractor(child));
			Parallel.ForEach<ChildContainerExtractor>(source, new Action<ChildContainerExtractor, ParallelLoopState>(this.ExtractChild));
			return container;
		}

		private void ExtractChild(ChildContainerExtractor childExtractor, ParallelLoopState state)
		{
			childExtractor.ExtractContainer();
		}

		private readonly ILogger logger;
	}
}
