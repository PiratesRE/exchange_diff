using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class DiscoveryKeywordStatsSearchTimedOut : MultiMailboxSearchException
	{
		public DiscoveryKeywordStatsSearchTimedOut(int mailboxesCount, Guid databaseId, string aqs) : base(Strings.SearchAdminRpcSearchCallTimedout("keyword stats", mailboxesCount, databaseId.ToString(), aqs))
		{
		}

		protected DiscoveryKeywordStatsSearchTimedOut(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
