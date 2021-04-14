using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class DiscoverySearchNonFullTextQuery : DiscoverySearchPermanentException
	{
		public DiscoverySearchNonFullTextQuery(SearchType searchType, string query) : base(Strings.SearchAdminRpcInvalidQuery((searchType == SearchType.Preview) ? "Preview" : "Statistics", query))
		{
		}

		protected DiscoverySearchNonFullTextQuery(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
