using System;

namespace Microsoft.Exchange.Collections
{
	internal class FixedTimeQuota : FixedTimeSumBase
	{
		public FixedTimeQuota(ushort windowBucketLength, ushort numberOfBuckets, uint limit) : base((uint)windowBucketLength, numberOfBuckets, new uint?(limit))
		{
		}

		internal new bool TryAdd(uint addend)
		{
			return base.TryAdd(addend);
		}
	}
}
