using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class CPUBasedSleeper
	{
		static CPUBasedSleeper()
		{
			if ((long)CPUBasedSleeper.processCpuSlowDownThresholdEntry.Value < 25L || (long)CPUBasedSleeper.processCpuSlowDownThresholdEntry.Value > 100L)
			{
				CPUBasedSleeper.ProcessCpuSlowDownThreshold = 75U;
				return;
			}
			CPUBasedSleeper.ProcessCpuSlowDownThreshold = (uint)CPUBasedSleeper.processCpuSlowDownThresholdEntry.Value;
		}

		internal static IPerfCounterReader ProcessCPUCounter { get; set; } = new ProcessCPURunningAveragePerfCounterReader();

		public static bool SleepIfNecessary(out int sleepTime, out float cpuPercent)
		{
			return CPUBasedSleeper.SleepIfNecessary(CPUBasedSleeper.ProcessCpuSlowDownThreshold, out sleepTime, out cpuPercent);
		}

		public static bool SleepIfNecessary(uint cpuStartPercent, out int sleepTime, out float cpuPercent)
		{
			sleepTime = -1;
			cpuPercent = -1f;
			if (cpuStartPercent >= 100U || CPUBasedSleeper.ProcessCPUCounter == null)
			{
				return false;
			}
			cpuPercent = CPUBasedSleeper.ProcessCPUCounter.GetValue();
			if (cpuPercent >= cpuStartPercent)
			{
				int num = (int)(100U - cpuStartPercent);
				if (num > 0)
				{
					float num2 = 500f / (float)num;
					sleepTime = (int)((cpuPercent - cpuStartPercent) * num2);
				}
				if (sleepTime > 0)
				{
					Thread.Sleep(sleepTime);
				}
				else
				{
					sleepTime = -1;
				}
			}
			ThrottlingPerfCounterWrapper.UpdateAverageThreadSleepTime((long)Math.Max(sleepTime, 0));
			return sleepTime >= 0;
		}

		private const string AppSettingProcessCpuSlowDownThreshold = "ProcessCpuSlowDownThreshold";

		private const uint DefaultProcessCpuThreshold = 75U;

		private const uint MinimumCpuThreshold = 25U;

		private const uint MaximumCpuThreshold = 100U;

		private const int MaxSleepThrottleTime = 500;

		public static readonly uint ProcessCpuSlowDownThreshold;

		private static readonly IntAppSettingsEntry processCpuSlowDownThresholdEntry = new IntAppSettingsEntry("ProcessCpuSlowDownThreshold", 75, ExTraceGlobals.ClientThrottlingTracer);
	}
}
