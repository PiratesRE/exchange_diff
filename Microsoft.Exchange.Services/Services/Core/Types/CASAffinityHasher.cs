using System;
using System.Security.Principal;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal static class CASAffinityHasher
	{
		internal static long ComputeIndex(string value, int numberOfBuckets)
		{
			return (long)(Math.Abs(value.ToUpperInvariant().GetHashCode()) % numberOfBuckets);
		}

		internal static long ComputeIndex(SecurityIdentifier sid, int numberOfBuckets)
		{
			return CASAffinityHasher.ComputeIndex(sid.Value, numberOfBuckets);
		}

		internal static long ComputeIndex(Guid guid, int numberOfBuckets)
		{
			return CASAffinityHasher.ComputeIndex(guid.ToString("D"), numberOfBuckets);
		}
	}
}
