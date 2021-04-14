using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class DiscoverySearchAbortedException : DiscoverySearchPermanentException
	{
		public DiscoverySearchAbortedException(Guid queryCorrelationId, Guid mdbGuid, string server) : base(Strings.DiscoverySearchAborted(queryCorrelationId.ToString(), mdbGuid.ToString(), server))
		{
		}
	}
}
