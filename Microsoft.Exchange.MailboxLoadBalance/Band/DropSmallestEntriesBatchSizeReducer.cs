using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Band
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DropSmallestEntriesBatchSizeReducer : DropEntriesBatchSizeReducer
	{
		public DropSmallestEntriesBatchSizeReducer(ByteQuantifiedSize targetSize, ILogger logger) : base(targetSize, logger)
		{
		}

		protected override IEnumerable<BandMailboxRebalanceData> SortResults(IEnumerable<BandMailboxRebalanceData> results)
		{
			return results.OrderByDescending(new Func<BandMailboxRebalanceData, ByteQuantifiedSize>(base.GetRebalanceDatumSize));
		}
	}
}
