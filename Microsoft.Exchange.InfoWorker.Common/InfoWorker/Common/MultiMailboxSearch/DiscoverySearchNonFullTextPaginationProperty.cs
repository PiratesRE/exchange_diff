using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class DiscoverySearchNonFullTextPaginationProperty : DiscoverySearchPermanentException
	{
		public DiscoverySearchNonFullTextPaginationProperty(string paginationClause) : base(Strings.SearchNonFullTextPaginationProperty(paginationClause))
		{
		}

		protected DiscoverySearchNonFullTextPaginationProperty(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
