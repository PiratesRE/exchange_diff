using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ResourceHealth
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class PingerPerfCounterWrapper
	{
		static PingerPerfCounterWrapper()
		{
			string instanceName;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				instanceName = string.Format("{0}({1})", currentProcess.ProcessName, currentProcess.Id);
			}
			PingerPerfCounterWrapper.instance = MSExchangeDatabasePinger.GetInstance(instanceName);
			PingerPerfCounterWrapper.perfCounterUpdateTimer = new Timer(delegate(object state)
			{
				PingerPerfCounterWrapper.instance.PingsPerMinute.RawValue = (long)((ulong)PingerPerfCounterWrapper.pingsPerMinute.GetValue());
				PingerPerfCounterWrapper.instance.CacheSize.RawValue = (long)PingerCache.Singleton.Count;
			}, null, TimeSpan.Zero, TimeSpan.FromMinutes(1.0));
		}

		public static void PingSuccessful()
		{
			PingerPerfCounterWrapper.pingsPerMinute.Add(1U);
			PingerPerfCounterWrapper.instance.PingsPerMinute.RawValue = (long)((ulong)PingerPerfCounterWrapper.pingsPerMinute.GetValue());
		}

		public static void PingFailed()
		{
			PingerPerfCounterWrapper.instance.FailedPings.Increment();
		}

		public static void PingTimedOut()
		{
			PingerPerfCounterWrapper.instance.PingTimeouts.Increment();
		}

		internal static MSExchangeDatabasePingerInstance GetInstanceForTest()
		{
			return PingerPerfCounterWrapper.instance;
		}

		private static MSExchangeDatabasePingerInstance instance;

		private static FixedTimeSum pingsPerMinute = new FixedTimeSum(15000, 4);

		private static Timer perfCounterUpdateTimer;
	}
}
