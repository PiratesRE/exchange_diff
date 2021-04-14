using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Win32;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Exchange.Services.Diagnostics
{
	public class StorePerformanceCountersCapture
	{
		internal static StorePerformanceCountersCapture Start(StoreSession storeSession)
		{
			StorePerformanceCountersCapture storePerformanceCountersCapture = new StorePerformanceCountersCapture
			{
				storeSession = storeSession
			};
			storePerformanceCountersCapture.Restart();
			return storePerformanceCountersCapture;
		}

		internal void Restart()
		{
			NativeMethods.GetTLSPerformanceContext(out this.beginPerformanceContext);
			this.beginCumulativeRPCPerformanceStatistics = this.storeSession.GetStoreCumulativeRPCStats();
			this.stopwatch = Stopwatch.StartNew();
			this.beginThreadTimes = ThreadTimes.GetFromCurrentThread();
		}

		internal StorePerformanceCounters Stop()
		{
			ThreadTimes fromCurrentThread = ThreadTimes.GetFromCurrentThread();
			long elapsedMilliseconds = this.stopwatch.ElapsedMilliseconds;
			PerformanceContext performanceContext;
			NativeMethods.GetTLSPerformanceContext(out performanceContext);
			CumulativeRPCPerformanceStatistics storeCumulativeRPCStats = this.storeSession.GetStoreCumulativeRPCStats();
			return new StorePerformanceCounters
			{
				ElapsedMilliseconds = elapsedMilliseconds,
				Cpu = (fromCurrentThread.Kernel - this.beginThreadTimes.Kernel + (fromCurrentThread.User - this.beginThreadTimes.User)).TotalMilliseconds,
				RpcLatency = TimeSpan.FromTicks((long)(performanceContext.rpcLatency - this.beginPerformanceContext.rpcLatency)).TotalMilliseconds,
				RpcCount = (int)(performanceContext.rpcCount - this.beginPerformanceContext.rpcCount),
				RpcLatencyOnStore = (storeCumulativeRPCStats.timeInServer - this.beginCumulativeRPCPerformanceStatistics.timeInServer).TotalMilliseconds
			};
		}

		private StoreSession storeSession;

		private Stopwatch stopwatch;

		private PerformanceContext beginPerformanceContext;

		private ThreadTimes beginThreadTimes;

		private CumulativeRPCPerformanceStatistics beginCumulativeRPCPerformanceStatistics;
	}
}
