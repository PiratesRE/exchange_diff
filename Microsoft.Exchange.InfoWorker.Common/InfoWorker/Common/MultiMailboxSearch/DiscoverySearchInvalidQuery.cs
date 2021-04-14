using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class DiscoverySearchInvalidQuery : DiscoverySearchPermanentException
	{
		public DiscoverySearchInvalidQuery() : base(Strings.InvalidSearchQuery)
		{
		}
	}
}
