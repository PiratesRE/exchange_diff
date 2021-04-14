using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Transport
{
	internal class DiskSpaceMonitor : ResourceMonitor
	{
		public DiskSpaceMonitor(string displayName, string path, ResourceManagerConfiguration.ResourceMonitorConfiguration configuration, ulong minimumFreeSpace = 0UL) : base(displayName, configuration)
		{
			this.path = Path.GetDirectoryName(path);
			this.minimumFreeSpace = minimumFreeSpace;
			ulong num;
			this.TryGetDiskUsage(out num, out this.diskSize);
		}

		public override void UpdateConfig()
		{
			if (this.minimumFreeSpace != 0UL && this.diskSize > 0UL)
			{
				base.HighPressureLimit = Math.Min((int)((this.diskSize - this.minimumFreeSpace) * 100UL / this.diskSize), this.Configuration.HighThreshold);
				base.MediumPressureLimit = Math.Min(base.HighPressureLimit - 2, this.Configuration.MediumThreshold);
				base.LowPressureLimit = Math.Min(base.MediumPressureLimit - 2, this.Configuration.NormalThreshold);
				return;
			}
			base.UpdateConfig();
		}

		protected override bool GetCurrentReading(out int currentReading)
		{
			ulong num;
			ulong num2;
			if (this.TryGetDiskUsage(out num, out num2) && num2 > 0UL)
			{
				num += this.GetFreeBytesAvailableOffset();
				currentReading = (int)((num2 - num) * 100UL / num2);
				return true;
			}
			currentReading = 0;
			return false;
		}

		protected bool TryGetDiskUsage(out ulong freeBytesAvailable, out ulong totalNumberOfBytes)
		{
			ulong num;
			if (!NativeMethods.GetDiskFreeSpaceEx(this.path, out freeBytesAvailable, out totalNumberOfBytes, out num))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				ExTraceGlobals.ResourceManagerTracer.TraceError<int>(0L, "Call to GetDiskFreeInfoEx failed with 0x{0:X}", lastWin32Error);
				return false;
			}
			return true;
		}

		protected virtual ulong GetFreeBytesAvailableOffset()
		{
			return 0UL;
		}

		private readonly string path;

		private readonly ulong minimumFreeSpace;

		private readonly ulong diskSize;
	}
}
