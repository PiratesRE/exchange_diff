using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Configuration.Core
{
	internal static class CPUMemoryLogger
	{
		static CPUMemoryLogger()
		{
			CPUMemoryLogger.ProcessorCount = Environment.ProcessorCount;
		}

		internal static void Log()
		{
			if (TickDiffer.Elapsed(CPUMemoryLogger.lastLogTime).TotalMinutes > (double)AppSettings.Current.LogCPUMemoryIntervalInMinutes)
			{
				CPUMemoryLogger.lastLogTime = Environment.TickCount;
				HttpLogger.SafeSetLogger(ConfigurationCoreMetadata.CPU, (long)CPUMemoryLogger.ProcessCpuPerfCounter.GetValue() + "% * " + CPUMemoryLogger.ProcessorCount);
				HttpLogger.SafeSetLogger(ConfigurationCoreMetadata.Memory, CPUMemoryLogger.GetMemory());
			}
		}

		private static string GetMemory()
		{
			string result;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				result = string.Concat(new object[]
				{
					currentProcess.WorkingSet64 / 1000000L,
					"M/",
					currentProcess.PrivateMemorySize64 / 1000000L,
					"M"
				});
			}
			return result;
		}

		private static readonly ProcessCPURunningAveragePerfCounterReader ProcessCpuPerfCounter = new ProcessCPURunningAveragePerfCounterReader();

		private static readonly int ProcessorCount;

		private static int lastLogTime = TickDiffer.Subtract(Environment.TickCount, AppSettings.Current.LogCPUMemoryIntervalInMinutes * 60 * 1000);
	}
}
