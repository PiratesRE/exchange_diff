using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	internal class NativeMethods
	{
		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool QueryPerformanceFrequency(out long freq);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool QueryPerformanceCounter(out long count);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool QueryThreadCycleTime(SafeThreadHandle handle, out ulong ticks);

		[DllImport("kernel32.dll")]
		internal static extern SafeThreadHandle GetCurrentThread();

		[DllImport("kernel32.dll")]
		internal static extern uint GetCurrentProcessorNumber();

		[DllImport("powrprof.dll", SetLastError = true)]
		internal static extern NativeMethods.NTSTATUS CallNtPowerInformation(int InformationLevel, IntPtr lpInputBuffer, uint nInputBufferSize, IntPtr lpOutputBuffer, uint nOutputBufferSize);

		internal enum NTSTATUS : uint
		{
			STATUS_SUCCESS,
			STATUS_BUFFER_TOO_SMALL = 3221225507U,
			STATUS_ACCESS_DENIED = 3221225506U
		}
	}
}
