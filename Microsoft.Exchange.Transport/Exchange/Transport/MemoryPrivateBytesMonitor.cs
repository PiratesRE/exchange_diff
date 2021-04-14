using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Transport
{
	internal sealed class MemoryPrivateBytesMonitor : ResourceMonitor
	{
		public MemoryPrivateBytesMonitor(ResourceManagerConfiguration.ResourceMonitorConfiguration configuration) : base(Strings.PrivateBytesResource, configuration)
		{
		}

		internal static ulong TotalPhysicalMemory
		{
			get
			{
				if (MemoryPrivateBytesMonitor.totalPhysicalMemory == 0UL)
				{
					NativeMethods.MemoryStatusEx memoryStatusEx;
					if (NativeMethods.GlobalMemoryStatusEx(out memoryStatusEx))
					{
						MemoryPrivateBytesMonitor.totalPhysicalMemory = memoryStatusEx.TotalPhys;
					}
					else
					{
						int lastWin32Error = Marshal.GetLastWin32Error();
						ExTraceGlobals.ResourceManagerTracer.TraceError<int>(0L, "Call to GlobalMemoryStatusEx failed with 0x{0:X}", lastWin32Error);
					}
				}
				return MemoryPrivateBytesMonitor.totalPhysicalMemory;
			}
		}

		private static ulong DefaultPrivateBytesLimit
		{
			get
			{
				if (MemoryPrivateBytesMonitor.defaultPrivateBytesLimit == 0UL)
				{
					bool flag = UIntPtr.Size >= 8;
					NativeMethods.MemoryStatusEx memoryStatusEx;
					if (NativeMethods.GlobalMemoryStatusEx(out memoryStatusEx))
					{
						MemoryPrivateBytesMonitor.totalPhysicalMemory = memoryStatusEx.TotalPhys;
						ulong val;
						if (flag)
						{
							val = 1099511627776UL;
						}
						else if (memoryStatusEx.TotalVirtual > (ulong)-2147483648)
						{
							val = 1887436800UL;
						}
						else
						{
							val = 838860800UL;
						}
						MemoryPrivateBytesMonitor.defaultPrivateBytesLimit = Math.Min(memoryStatusEx.TotalPhys * 75UL / 100UL, val);
					}
					else
					{
						int lastWin32Error = Marshal.GetLastWin32Error();
						ExTraceGlobals.ResourceManagerTracer.TraceError<int>(0L, "Call to GlobalMemoryStatusEx failed with 0x{0:X}", lastWin32Error);
						MemoryPrivateBytesMonitor.defaultPrivateBytesLimit = (flag ? 1099511627776UL : 838860800UL);
					}
				}
				return MemoryPrivateBytesMonitor.defaultPrivateBytesLimit;
			}
		}

		public override void UpdateConfig()
		{
			if (this.Configuration.HighThreshold == 0 && MemoryPrivateBytesMonitor.TotalPhysicalMemory > 0UL)
			{
				base.HighPressureLimit = (int)(MemoryPrivateBytesMonitor.DefaultPrivateBytesLimit * 100UL / MemoryPrivateBytesMonitor.TotalPhysicalMemory);
				base.MediumPressureLimit = base.HighPressureLimit - 2;
				base.LowPressureLimit = base.MediumPressureLimit - 2;
				return;
			}
			base.UpdateConfig();
		}

		protected override bool GetCurrentReading(out int currentReading)
		{
			currentReading = 0;
			if (MemoryPrivateBytesMonitor.TotalPhysicalMemory > 0UL)
			{
				using (SafeProcessHandle currentProcess = NativeMethods.GetCurrentProcess())
				{
					NativeMethods.ProcessMemoryCounterEx processMemoryCounterEx;
					if (NativeMethods.GetProcessMemoryInfo(currentProcess, out processMemoryCounterEx, NativeMethods.ProcessMemoryCounterEx.Size))
					{
						currentReading = (int)(processMemoryCounterEx.privateUsage.ToUInt64() * 100UL / MemoryPrivateBytesMonitor.TotalPhysicalMemory);
					}
					else
					{
						ExTraceGlobals.ResourceManagerTracer.TraceError<int>(0L, "Failed to GetProcessMemoryInfo Error: {0}", Marshal.GetLastWin32Error());
						currentReading = 0;
					}
				}
				return true;
			}
			return false;
		}

		internal const ulong TERABYTE = 1099511627776UL;

		internal const ulong GIGABYTE = 1073741824UL;

		internal const ulong MEGABYTE = 1048576UL;

		private const ulong PrivateBytesLimit2GB = 838860800UL;

		private const ulong PrivateBytesLimit3GB = 1887436800UL;

		private const ulong PrivateBytesLimit64Bit = 1099511627776UL;

		private static ulong totalPhysicalMemory;

		private static ulong defaultPrivateBytesLimit;
	}
}
