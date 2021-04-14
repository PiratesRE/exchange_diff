using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.LatencyDetection;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RpcDataProvider : PerformanceDataProvider
	{
		private RpcDataProvider() : base("Remote Procedure Calls", true)
		{
		}

		public static RpcDataProvider Instance
		{
			get
			{
				return RpcDataProvider.singletonInstance;
			}
		}

		public override PerformanceData TakeSnapshot(bool begin)
		{
			PerformanceContext performanceContext;
			if (NativeMethods.GetTLSPerformanceContext(out performanceContext))
			{
				base.Latency = TimeSpan.FromMilliseconds((double)((int)(performanceContext.rpcLatency / 10000UL)));
				base.RequestCount = performanceContext.rpcCount;
			}
			else
			{
				base.Latency = TimeSpan.Zero;
				base.RequestCount = 0U;
			}
			base.TakeSnapshot(begin);
			return new PerformanceData(base.Latency, base.RequestCount, (int)performanceContext.currentActiveConnections, (int)performanceContext.currentConnectionPoolSize, (int)performanceContext.failedConnections);
		}

		private const int MillisecondsFactor = 10000;

		private static RpcDataProvider singletonInstance = new RpcDataProvider();
	}
}
