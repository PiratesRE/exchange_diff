using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxLoadBalance.Band
{
	internal interface IBatchSizeReducer
	{
		IEnumerable<BandMailboxRebalanceData> ReduceBatchSize(IEnumerable<BandMailboxRebalanceData> results);
	}
}
