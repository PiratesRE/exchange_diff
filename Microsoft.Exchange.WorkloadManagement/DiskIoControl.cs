using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal static class DiskIoControl
	{
		public static int GetDiskReadLatency(string volumeName, ref DiskPerformanceStructure lastDiskPerf, out DateTime lastUpdatedTime)
		{
			DiskPerformanceStructure diskPerformance = DiskIoControl.GetDiskPerformance(volumeName);
			long num = diskPerformance.ReadTime - lastDiskPerf.ReadTime;
			int num2 = diskPerformance.ReadCount - lastDiskPerf.ReadCount;
			int result = 0;
			if (num < 0L || num2 < 0)
			{
				result = int.MaxValue;
				ExTraceGlobals.ResourceHealthManagerTracer.TraceError<string, long, int>(0L, "[DiskIoControl.GetDiskReadLatency] Volume: {0}. Set readLatency to int.MaxValue. Reported readTime: {1}, reported readCount: {2}", volumeName, num, num2);
			}
			else if (num2 != 0)
			{
				try
				{
					long value = num / checked(unchecked((long)num2) * 10000L);
					result = Convert.ToInt32(value);
				}
				catch (OverflowException arg)
				{
					result = int.MaxValue;
					ExTraceGlobals.ResourceHealthManagerTracer.TraceError<string, OverflowException>(0L, "[DiskIoControl.GetDiskReadLatency] Volume: {0}. Set readLatency to int.MaxValue. Error: {1}", volumeName, arg);
				}
			}
			lastDiskPerf = diskPerformance;
			lastUpdatedTime = DateTime.UtcNow;
			return result;
		}

		public static DiskPerformanceStructure GetDiskPerformance(string volumeName)
		{
			return DiskIoControl.GetDeviceIoControlReturnedStructures<DiskPerformanceStructure>("\\\\.\\PhysicalDrive" + DiskIoControl.GetDeviceIoControlReturnedStructures<DiskExtents>(volumeName.TrimEnd(new char[]
			{
				'\\'
			}), 5636096U).Extents.DiskNumber, 458784U);
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern uint DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize, IntPtr lpOutBuffer, int nOutBufferSize, ref uint lpBytesReturned, IntPtr lpOverlapped);

		private static T GetDeviceIoControlReturnedStructures<T>(string pathToDevice, uint ioControlCode) where T : struct
		{
			T result;
			using (SafeFileHandle safeFileHandle = DiskIoControl.CreateFile(pathToDevice, 0U, 3U, 0, 3U, 0U, 0))
			{
				if (safeFileHandle == null || safeFileHandle.IsInvalid)
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
				int num = Marshal.SizeOf(typeof(T));
				IntPtr intPtr = Marshal.AllocHGlobal(num);
				uint num2 = 0U;
				if (DiskIoControl.DeviceIoControl(safeFileHandle, ioControlCode, IntPtr.Zero, 0U, intPtr, num, ref num2, IntPtr.Zero) == 0U)
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
				result = (T)((object)Marshal.PtrToStructure(intPtr, typeof(T)));
				Marshal.FreeHGlobal(intPtr);
			}
			return result;
		}

		private const int IoctlDiskPerformance = 458784;

		private const int IoctlVolumeGetVolumeDiskExtents = 5636096;

		private const uint DriveDesiredAccessNoAccess = 0U;

		private const uint DriveShareModeReadWrite = 3U;

		private const uint DriveDispositionOpenExisting = 3U;

		private const uint DriveFlagsAndAttributesNone = 0U;

		private const string DriveNameTemplate = "\\\\.\\PhysicalDrive";
	}
}
