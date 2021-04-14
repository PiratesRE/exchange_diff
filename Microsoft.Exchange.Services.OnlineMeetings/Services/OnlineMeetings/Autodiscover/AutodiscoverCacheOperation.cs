using System;

namespace Microsoft.Exchange.Services.OnlineMeetings.Autodiscover
{
	internal enum AutodiscoverCacheOperation
	{
		None,
		InvalidateUser,
		InvalidateDomain,
		IncrementFailureCounter
	}
}
