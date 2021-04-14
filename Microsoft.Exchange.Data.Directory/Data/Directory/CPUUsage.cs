using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class CPUUsage
	{
		internal static bool CalculateCPUUsagePercentage(ref DateTime lastUpdatedTime, ref long lastCPU, out float cpuUsage)
		{
			return CPUUsage.InternalCalculateCPUUsagePercentage(IntPtr.Zero, ref lastUpdatedTime, ref lastCPU, out cpuUsage);
		}

		internal static bool CalculateCPUUsagePercentage(IntPtr process, ref DateTime lastUpdatedTime, ref long lastCPU, out float cpuUsage)
		{
			if (process == IntPtr.Zero)
			{
				throw new ArgumentNullException("process");
			}
			return CPUUsage.InternalCalculateCPUUsagePercentage(process, ref lastUpdatedTime, ref lastCPU, out cpuUsage);
		}

		internal static bool GetCurrentCPU(ref long cpuTime)
		{
			return CPUUsage.InternalGetCurrentCPU(IntPtr.Zero, ref cpuTime);
		}

		internal static bool GetCurrentCPU(IntPtr process, ref long cpuTime)
		{
			if (process == IntPtr.Zero)
			{
				throw new ArgumentNullException("process");
			}
			return CPUUsage.InternalGetCurrentCPU(process, ref cpuTime);
		}

		private static bool InternalCalculateCPUUsagePercentage(IntPtr process, ref DateTime lastUpdatedTime, ref long lastCPU, out float cpuUsage)
		{
			cpuUsage = 0f;
			long num = 0L;
			if (!CPUUsage.InternalGetCurrentCPU(process, ref num))
			{
				return false;
			}
			DateTime utcNow = DateTime.UtcNow;
			long num2 = num - lastCPU;
			double totalSeconds = (utcNow - lastUpdatedTime).TotalSeconds;
			if (totalSeconds > 0.0)
			{
				cpuUsage = (float)((double)num2 * 1E-05 / (totalSeconds * (double)CPUUsage.processorCount));
				if (cpuUsage > 100f)
				{
					cpuUsage = 100f;
				}
				lastUpdatedTime = utcNow;
				lastCPU = num;
			}
			return true;
		}

		private static bool InternalGetCurrentCPU(IntPtr process, ref long cpuTime)
		{
			bool result = false;
			long num3;
			long num4;
			long num5;
			if (process != IntPtr.Zero)
			{
				long num;
				long num2;
				if (NativeMethods.GetProcessTimes(process, out num, out num2, out num3, out num4))
				{
					result = true;
					cpuTime = num3 + num4;
				}
				else
				{
					CPUUsage.Tracer.TraceError<uint>(0L, "[CPUUsage::InternalGetCurrentCPU] Calling GetProcessTimes failed with error code '{0}'.", NativeMethods.GetLastError());
				}
			}
			else if (NativeMethods.GetSystemTimes(out num5, out num3, out num4))
			{
				result = true;
				cpuTime = num3 + num4 - num5;
			}
			else
			{
				CPUUsage.Tracer.TraceError<uint>(0L, "[CPUUsage::InternalGetCurrentCPU] Calling GetSystemTimes failed with error code '{0}'.", NativeMethods.GetLastError());
			}
			return result;
		}

		private static readonly Trace Tracer = ExTraceGlobals.ResourceHealthManagerTracer;

		private static readonly int processorCount = Environment.ProcessorCount;
	}
}
