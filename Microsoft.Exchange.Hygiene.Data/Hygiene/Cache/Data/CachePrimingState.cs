using System;

namespace Microsoft.Exchange.Hygiene.Cache.Data
{
	internal enum CachePrimingState
	{
		Unavailable,
		Priming,
		Stale,
		Unhealthy,
		Healthy,
		Unknown,
		PrimingWithFile
	}
}
