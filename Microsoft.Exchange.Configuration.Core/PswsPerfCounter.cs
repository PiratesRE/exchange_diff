using System;

namespace Microsoft.Exchange.Configuration.Core
{
	public class PswsPerfCounter
	{
		internal static PswshttpRequestPerformanceCountersInstance Instance
		{
			get
			{
				if (PswsPerfCounter.perfCounter == null)
				{
					PswsPerfCounter.InitializePerfCounter();
				}
				return PswsPerfCounter.perfCounter;
			}
		}

		internal static void UpdatePerfCounter(long totalRequestTime)
		{
			if (PswsPerfCounter.perfCounter == null)
			{
				PswsPerfCounter.InitializePerfCounter();
			}
			long averageResponseTime = PswsPerfCounter.averageRT.GetAverageResponseTime(totalRequestTime, PswsPerfCounter.perfCounter.AverageResponseTime.RawValue);
			if (averageResponseTime != 0L)
			{
				PswsPerfCounter.perfCounter.AverageResponseTime.RawValue = averageResponseTime;
			}
		}

		private static void InitializePerfCounter()
		{
			PswsPerfCounter.perfCounter = PswshttpRequestPerformanceCounters.GetInstance("Psws");
			PswsPerfCounter.averageRT = new AverageResponseTimeCounter();
		}

		private const string PowerShellWebServicePerfCounterInstanceName = "Psws";

		private static PswshttpRequestPerformanceCountersInstance perfCounter;

		private static AverageResponseTimeCounter averageRT;
	}
}
