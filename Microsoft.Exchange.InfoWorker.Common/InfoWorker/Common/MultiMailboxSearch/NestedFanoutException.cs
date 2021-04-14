using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class NestedFanoutException : DiscoverySearchPermanentException
	{
		public NestedFanoutException(string mailboxName) : base(Strings.NestedFanout(mailboxName))
		{
		}
	}
}
