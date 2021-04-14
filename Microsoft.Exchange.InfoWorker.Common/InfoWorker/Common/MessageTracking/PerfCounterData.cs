using System;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class PerfCounterData
	{
		public static SlidingWindowResultCounter ResultCounter = new SlidingWindowResultCounter(10);
	}
}
