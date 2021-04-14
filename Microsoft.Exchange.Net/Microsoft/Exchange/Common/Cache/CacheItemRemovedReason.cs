using System;

namespace Microsoft.Exchange.Common.Cache
{
	internal enum CacheItemRemovedReason
	{
		Removed,
		Expired,
		Scavenged,
		OverWritten,
		Clear
	}
}
