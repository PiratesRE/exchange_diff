using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class DiscoverySearchInvalidKeywordStatsRequest : DiscoverySearchPermanentException
	{
		public DiscoverySearchInvalidKeywordStatsRequest(Guid mdbGuid, string server, Exception innerException) : base(Strings.InvalidKeywordStatsRequest(mdbGuid.ToString(), server), innerException)
		{
		}
	}
}
