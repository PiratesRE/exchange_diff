using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class Align
	{
		internal static long RoundUp(long val, long quantum)
		{
			long num = val % quantum;
			if (num > 0L)
			{
				return val + (quantum - num);
			}
			return val;
		}

		internal static uint RoundUp(uint val, uint quantum)
		{
			uint num = val % quantum;
			if (num > 0U)
			{
				return val + (quantum - num);
			}
			return val;
		}
	}
}
