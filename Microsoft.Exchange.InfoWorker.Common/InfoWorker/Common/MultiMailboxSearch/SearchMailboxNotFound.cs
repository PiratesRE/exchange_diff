using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SearchMailboxNotFound : DiscoverySearchPermanentException
	{
		public SearchMailboxNotFound(string server, string databaseName, string mailboxGuid) : base(Strings.SearchMailboxNotFound(mailboxGuid, databaseName, server))
		{
		}
	}
}
