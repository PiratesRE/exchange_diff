using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class RequestStatisticsForAD
	{
		public static RequestStatisticsForAD Begin()
		{
			return new RequestStatisticsForAD
			{
				begin = new PerformanceContext(PerformanceContext.Current)
			};
		}

		public RequestStatistics End(RequestStatisticsType tag)
		{
			return this.End(tag, null);
		}

		public RequestStatistics End(RequestStatisticsType tag, string destination)
		{
			PerformanceContext performanceContext = PerformanceContext.Current;
			long timeTaken = 0L;
			int requestCount = 0;
			if (this.begin != null && performanceContext != null)
			{
				timeTaken = (long)(performanceContext.RequestLatency - this.begin.RequestLatency);
				requestCount = (int)(performanceContext.RequestCount - this.begin.RequestCount);
			}
			if (destination == null)
			{
				return RequestStatistics.Create(tag, timeTaken, requestCount);
			}
			return RequestStatistics.Create(tag, timeTaken, requestCount, destination);
		}

		private PerformanceContext begin;
	}
}
