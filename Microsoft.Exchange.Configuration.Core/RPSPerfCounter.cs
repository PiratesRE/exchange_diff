using System;

namespace Microsoft.Exchange.Configuration.Core
{
	public static class RPSPerfCounter
	{
		public static void UpdateAverageRTCounter(long requestTime)
		{
			if (RPSPerfCounter.currentAvgRT == null)
			{
				RPSPerfCounter.currentAvgRT = new AverageResponseTimeCounter();
			}
			long averageResponseTime = RPSPerfCounter.currentAvgRT.GetAverageResponseTime(requestTime, RPSPerfCounterHelper.CurrentPerfCounter.AverageResponseTime.RawValue);
			if (averageResponseTime != 0L)
			{
				RPSPerfCounterHelper.CurrentPerfCounter.AverageResponseTime.RawValue = averageResponseTime;
			}
		}

		private static AverageResponseTimeCounter currentAvgRT;
	}
}
