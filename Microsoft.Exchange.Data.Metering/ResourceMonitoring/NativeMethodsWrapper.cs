using System;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal class NativeMethodsWrapper : INativeMethodsWrapper
	{
		public bool GetDiskFreeSpaceEx(string directoryName, out ulong freeBytesAvailable, out ulong totalNumberOfBytes, out ulong totalNumberOfFreeBytes)
		{
			return NativeMethods.GetDiskFreeSpaceEx(directoryName, out freeBytesAvailable, out totalNumberOfBytes, out totalNumberOfFreeBytes);
		}

		public bool GetSystemMemoryUsePercentage(out uint systemMemoryUsage)
		{
			NativeMethods.MemoryStatusEx memoryStatusEx;
			if (NativeMethods.GlobalMemoryStatusEx(out memoryStatusEx))
			{
				systemMemoryUsage = memoryStatusEx.MemoryLoad;
				return true;
			}
			systemMemoryUsage = 0U;
			return false;
		}

		public bool GetTotalSystemMemory(out ulong systemMemory)
		{
			NativeMethods.MemoryStatusEx memoryStatusEx;
			if (NativeMethods.GlobalMemoryStatusEx(out memoryStatusEx))
			{
				systemMemory = memoryStatusEx.TotalPhys;
				return true;
			}
			systemMemory = 0UL;
			return false;
		}

		public bool GetTotalVirtualMemory(out ulong virtualMemory)
		{
			NativeMethods.MemoryStatusEx memoryStatusEx;
			if (NativeMethods.GlobalMemoryStatusEx(out memoryStatusEx))
			{
				virtualMemory = memoryStatusEx.TotalVirtual;
				return true;
			}
			virtualMemory = 0UL;
			return false;
		}

		public bool GetProcessPrivateBytes(out ulong privateBytes)
		{
			bool result;
			using (SafeProcessHandle currentProcess = NativeMethods.GetCurrentProcess())
			{
				NativeMethods.ProcessMemoryCounterEx processMemoryCounterEx;
				if (NativeMethods.GetProcessMemoryInfo(currentProcess, out processMemoryCounterEx, NativeMethods.ProcessMemoryCounterEx.Size))
				{
					privateBytes = processMemoryCounterEx.privateUsage.ToUInt64();
					result = true;
				}
				else
				{
					privateBytes = 0UL;
					result = false;
				}
			}
			return result;
		}
	}
}
