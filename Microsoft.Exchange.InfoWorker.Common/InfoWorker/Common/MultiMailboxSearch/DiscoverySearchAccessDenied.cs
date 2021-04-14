using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class DiscoverySearchAccessDenied : MultiMailboxSearchException
	{
		public DiscoverySearchAccessDenied(string displayName, Guid databaseId) : base(Strings.SearchAdminRpcCallAccessDenied(displayName, databaseId.ToString()))
		{
		}

		protected DiscoverySearchAccessDenied(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
