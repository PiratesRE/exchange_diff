using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ILoadBalance
	{
		IEnumerable<BandMailboxRebalanceData> BalanceForest(LoadContainer forest);
	}
}
