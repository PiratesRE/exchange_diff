using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SearchNonFullTextSortSpecification : DiscoverySearchPermanentException
	{
		public SearchNonFullTextSortSpecification(string sortBy) : base(Strings.SearchNonFullTextSortByProperty(sortBy))
		{
		}

		protected SearchNonFullTextSortSpecification(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
