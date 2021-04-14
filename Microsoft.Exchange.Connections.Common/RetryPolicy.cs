using System;

namespace Microsoft.Exchange.Connections.Common
{
	[Serializable]
	internal enum RetryPolicy
	{
		Backoff,
		Immediate
	}
}
