using System;
using Microsoft.Exchange.Diagnostics.LatencyDetection;

namespace Microsoft.Exchange.Data.Directory
{
	internal sealed class PerformanceContext : PerformanceDataProvider
	{
		internal PerformanceContext() : base("LDAP Requests")
		{
		}

		internal PerformanceContext(PerformanceContext performanceContext) : this()
		{
			base.RequestCount = performanceContext.RequestCount;
			base.Latency = performanceContext.Latency;
		}

		public int RequestLatency
		{
			get
			{
				return (int)base.Latency.TotalMilliseconds;
			}
		}

		internal static PerformanceContext Current
		{
			get
			{
				if (PerformanceContext.current == null)
				{
					PerformanceContext.current = new PerformanceContext();
				}
				return PerformanceContext.current;
			}
		}

		internal static void UpdateContext(uint requestCount, int requestLatency)
		{
			PerformanceContext performanceContext = PerformanceContext.Current;
			performanceContext.RequestCount += requestCount;
			performanceContext.Latency += TimeSpan.FromMilliseconds((double)requestLatency);
		}

		[ThreadStatic]
		private static PerformanceContext current;
	}
}
