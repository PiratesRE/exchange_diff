using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MailTips
{
	internal static class PerfCounterHelper
	{
		public static void UpdateMailTipsConfigurationResponseTimePerformanceCounter(long newValue)
		{
			PerfCounterHelper.UpdateMovingAveragePerformanceCounter(MailTipsPerfCounters.MailTipsConfigurationAverageResponseTime, newValue, ref PerfCounterHelper.mailTipsConfigurationResponseTimeAverage, PerfCounterHelper.mailTipsConfigurationResponseTimeAverageLock);
		}

		public static void UpdateServiceConfigurationResponseTimePerformanceCounter(long newValue)
		{
			PerfCounterHelper.UpdateMovingAveragePerformanceCounter(MailTipsPerfCounters.ServiceConfigurationAverageResponseTime, newValue, ref PerfCounterHelper.serviceConfigurationResponseTimeAverage, PerfCounterHelper.serviceConfigurationResponseTimeAverageLock);
		}

		public static void UpdateMailTipsResponseTimePerformanceCounter(long newValue)
		{
			PerfCounterHelper.UpdateMovingAveragePerformanceCounter(MailTipsPerfCounters.MailTipsAverageResponseTime, newValue, ref PerfCounterHelper.mailTipsResponseTimeAverage, PerfCounterHelper.mailTipsResponseTimeAverageLock);
		}

		public static void UpdateMailTipsRecipientNumberPerformanceCounter(long newValue)
		{
			PerfCounterHelper.UpdateMovingAveragePerformanceCounter(MailTipsPerfCounters.MailTipsAverageRecipientNumber, newValue, ref PerfCounterHelper.mailTipsRecipientNumberAverage, PerfCounterHelper.mailTipsRecipientNumberAverageLock);
		}

		public static void UpdateMailTipsAverageActiveDirectoryResponseTimePerformanceCounter(long newValue)
		{
			PerfCounterHelper.UpdateMovingAveragePerformanceCounter(MailTipsPerfCounters.MailTipsAverageActiveDirectoryResponseTime, newValue, ref PerfCounterHelper.mailTipsAverageActiveDirectoryResponseTime, PerfCounterHelper.mailTipsAverageActiveDirectoryResponseTimeLock);
		}

		private static void UpdateMovingAveragePerformanceCounter(ExPerformanceCounter performanceCounter, long newValue, ref double averageValue, object lockObject)
		{
			lock (lockObject)
			{
				averageValue = (1.0 - PerfCounterHelper.averageMultiplier) * averageValue + PerfCounterHelper.averageMultiplier * (double)newValue;
				performanceCounter.RawValue = (long)averageValue;
			}
		}

		private static double averageMultiplier = 0.04;

		private static object mailTipsConfigurationResponseTimeAverageLock = new object();

		private static double mailTipsConfigurationResponseTimeAverage;

		private static object serviceConfigurationResponseTimeAverageLock = new object();

		private static double serviceConfigurationResponseTimeAverage;

		private static object mailTipsResponseTimeAverageLock = new object();

		private static double mailTipsResponseTimeAverage;

		private static object mailTipsRecipientNumberAverageLock = new object();

		private static double mailTipsRecipientNumberAverage;

		private static object mailTipsAverageActiveDirectoryResponseTimeLock = new object();

		private static double mailTipsAverageActiveDirectoryResponseTime;
	}
}
