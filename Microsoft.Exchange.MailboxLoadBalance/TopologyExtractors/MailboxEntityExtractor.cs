using System;
using System.Linq;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Data.LoadMetrics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MailboxEntityExtractor : TopologyExtractor
	{
		public MailboxEntityExtractor(DirectoryObject directoryObject, TopologyExtractorFactory extractorFactory, Band[] bands) : base(directoryObject, extractorFactory)
		{
			AnchorUtil.ThrowOnNullArgument(bands, "bands");
			this.bands = bands;
		}

		public override LoadEntity ExtractEntity()
		{
			LoadEntity loadEntity = new LoadEntity(base.DirectoryObject);
			DirectoryMailbox mailbox = (DirectoryMailbox)base.DirectoryObject;
			foreach (LoadMetric loadMetric in this.bands.Union(LoadMetricRepository.DefaultMetrics))
			{
				long unitsForMailbox = loadMetric.GetUnitsForMailbox(mailbox);
				loadEntity.ConsumedLoad[loadMetric] = unitsForMailbox;
			}
			return loadEntity;
		}

		public override LoadContainer ExtractTopology()
		{
			throw new NotSupportedException("Cannot extract topology from a mailbox.");
		}

		private readonly Band[] bands;
	}
}
