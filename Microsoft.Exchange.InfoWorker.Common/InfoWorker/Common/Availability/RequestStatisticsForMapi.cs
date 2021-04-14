using System;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class RequestStatisticsForMapi
	{
		public static RequestStatisticsForMapi Begin()
		{
			PerformanceContext performanceContext;
			NativeMethods.GetTLSPerformanceContext(out performanceContext);
			return new RequestStatisticsForMapi
			{
				beginRpcCount = performanceContext.rpcCount,
				beginRpcLatency = performanceContext.rpcLatency
			};
		}

		public RequestStatistics End(RequestStatisticsType tag)
		{
			return this.End(tag, null);
		}

		public RequestStatistics End(RequestStatisticsType tag, string destination)
		{
			long timeTaken = 0L;
			int requestCount = 0;
			PerformanceContext performanceContext;
			if (NativeMethods.GetTLSPerformanceContext(out performanceContext))
			{
				timeTaken = (long)((performanceContext.rpcLatency - this.beginRpcLatency) / 100000UL);
				requestCount = (int)(performanceContext.rpcCount - this.beginRpcCount);
			}
			if (destination == null)
			{
				return RequestStatistics.Create(tag, timeTaken, requestCount);
			}
			return RequestStatistics.Create(tag, timeTaken, requestCount, destination);
		}

		private RequestStatisticsForMapi()
		{
		}

		private uint beginRpcCount;

		private ulong beginRpcLatency;
	}
}
