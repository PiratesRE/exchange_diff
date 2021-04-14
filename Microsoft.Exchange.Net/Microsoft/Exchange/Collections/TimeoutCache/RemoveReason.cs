using System;

namespace Microsoft.Exchange.Collections.TimeoutCache
{
	internal enum RemoveReason
	{
		Expired,
		Removed,
		PreemptivelyExpired,
		Cleanup
	}
}
