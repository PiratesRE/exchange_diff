using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class DiscoverySearchFailed : MultiMailboxSearchException
	{
		public DiscoverySearchFailed(Guid databaseId, int errorCode) : base(Strings.SearchAdminRpcCallFailed(databaseId.ToString(), errorCode))
		{
		}

		public DiscoverySearchFailed(Guid databaseId, int errorCode, Exception innerException) : base(Strings.SearchAdminRpcCallFailed(databaseId.ToString(), errorCode), innerException)
		{
		}

		protected DiscoverySearchFailed(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
