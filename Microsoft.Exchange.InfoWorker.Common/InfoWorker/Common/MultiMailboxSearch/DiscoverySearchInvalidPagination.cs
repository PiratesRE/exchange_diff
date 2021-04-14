using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class DiscoverySearchInvalidPagination : DiscoverySearchPermanentException
	{
		public DiscoverySearchInvalidPagination() : base(Strings.SearchInvalidPagination)
		{
		}

		protected DiscoverySearchInvalidPagination(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
