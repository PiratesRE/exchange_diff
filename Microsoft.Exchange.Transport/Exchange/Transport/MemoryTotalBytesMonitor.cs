using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Transport
{
	internal sealed class MemoryTotalBytesMonitor : ResourceMonitor
	{
		public MemoryTotalBytesMonitor(ResourceManagerConfiguration.ResourceMonitorConfiguration configuration) : base(string.Empty, configuration)
		{
		}

		public override string ToString(ResourceUses resourceUses, int currentPressure)
		{
			return Strings.PhysicalMemoryUses(currentPressure, base.HighPressureLimit);
		}

		protected override bool GetCurrentReading(out int currentReading)
		{
			NativeMethods.MemoryStatusEx memoryStatusEx;
			if (NativeMethods.GlobalMemoryStatusEx(out memoryStatusEx))
			{
				currentReading = (int)memoryStatusEx.MemoryLoad;
			}
			else
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				ExTraceGlobals.ResourceManagerTracer.TraceError<int>(0L, "Call to GlobalMemoryStatusEx failed with 0x{0:X}", lastWin32Error);
				currentReading = 0;
			}
			return true;
		}
	}
}
