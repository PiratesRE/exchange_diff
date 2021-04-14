using System;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal enum LatencyBucket
	{
		Healthy,
		Delayed = 3,
		Unhealthy
	}
}
