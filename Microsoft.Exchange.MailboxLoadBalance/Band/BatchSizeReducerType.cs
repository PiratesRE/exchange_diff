using System;

namespace Microsoft.Exchange.MailboxLoadBalance.Band
{
	public enum BatchSizeReducerType
	{
		FactorBased,
		DropLargest,
		DropSmallest
	}
}
