using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class DiscoverySearchInvalidSortSpecification : DiscoverySearchPermanentException
	{
		public DiscoverySearchInvalidSortSpecification(string sortBy) : base(Strings.SearchInvalidSortSpecification(sortBy))
		{
		}

		protected DiscoverySearchInvalidSortSpecification(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
