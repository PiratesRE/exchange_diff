using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class DiscoverySearchCIFailure : MultiMailboxSearchException
	{
		public DiscoverySearchCIFailure(Guid databaseId, string server, int errorCode, Exception innerException) : base(Strings.DiscoverySearchCIFailure(databaseId.ToString(), server, errorCode), innerException)
		{
		}

		protected DiscoverySearchCIFailure(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
