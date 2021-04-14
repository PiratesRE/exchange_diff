using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class DiscoverySearchInvalidSearchRequest : DiscoverySearchPermanentException
	{
		public DiscoverySearchInvalidSearchRequest(Guid mdbGuid, string server, Exception innerException) : base(Strings.InvalidSearchRequest(mdbGuid.ToString(), server), innerException)
		{
		}
	}
}
