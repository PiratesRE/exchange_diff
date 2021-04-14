using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class DiscoverySearchTimedOut : MultiMailboxSearchException
	{
		public DiscoverySearchTimedOut(int mailboxesCount, Guid databaseId, string aqs) : base(Strings.SearchAdminRpcSearchCallTimedout("preview", mailboxesCount, databaseId.ToString(), aqs))
		{
		}

		protected DiscoverySearchTimedOut(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
