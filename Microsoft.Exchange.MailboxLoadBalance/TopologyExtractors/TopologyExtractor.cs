using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class TopologyExtractor
	{
		protected TopologyExtractor(DirectoryObject directoryObject, TopologyExtractorFactory extractorFactory)
		{
			AnchorUtil.ThrowOnNullArgument(directoryObject, "directoryObject");
			AnchorUtil.ThrowOnNullArgument(extractorFactory, "extractorFactory");
			this.DirectoryObject = directoryObject;
			this.ExtractorFactory = extractorFactory;
		}

		private protected DirectoryObject DirectoryObject { protected get; private set; }

		private protected TopologyExtractorFactory ExtractorFactory { protected get; private set; }

		public abstract LoadContainer ExtractTopology();

		public virtual LoadEntity ExtractEntity()
		{
			return this.ExtractTopology();
		}
	}
}
