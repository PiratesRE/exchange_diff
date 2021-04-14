using System;
using Microsoft.Exchange.Diagnostics.LatencyDetection;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class MServeLatencyContext : PerformanceDataProvider
	{
		internal MServeLatencyContext() : base("MServe Requests")
		{
		}

		internal static MServeLatencyContext Current
		{
			get
			{
				if (MServeLatencyContext.current == null)
				{
					MServeLatencyContext.current = new MServeLatencyContext();
				}
				return MServeLatencyContext.current;
			}
		}

		internal static void UpdateContext(uint requestCount, int requestLatency)
		{
			MServeLatencyContext mserveLatencyContext = MServeLatencyContext.Current;
			mserveLatencyContext.RequestCount += requestCount;
			mserveLatencyContext.Latency += TimeSpan.FromMilliseconds((double)requestLatency);
		}

		[ThreadStatic]
		private static MServeLatencyContext current;
	}
}
