using System;

namespace Microsoft.Exchange.Collections
{
	internal class FixedTimeSum : FixedTimeSumBase
	{
		public FixedTimeSum(ushort windowBucketLength, ushort numberOfBuckets) : base((uint)windowBucketLength, numberOfBuckets, null)
		{
		}

		internal void Add(uint value)
		{
			base.TryAdd(value);
		}
	}
}
