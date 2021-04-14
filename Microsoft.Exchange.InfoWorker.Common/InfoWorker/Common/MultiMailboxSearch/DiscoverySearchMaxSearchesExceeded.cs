using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class DiscoverySearchMaxSearchesExceeded : MultiMailboxSearchException
	{
		public DiscoverySearchMaxSearchesExceeded(Guid databaseId) : base(Strings.SearchAdminRpcCallMaxSearches(databaseId.ToString()))
		{
		}

		protected DiscoverySearchMaxSearchesExceeded(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
