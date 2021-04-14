using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.UM.NativeMethods
{
	internal static class Win32NativeMethods
	{
		[DllImport("kernel32.dll")]
		public static extern IntPtr GetProcessHeap();

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr CreateIoCompletionPort(IntPtr FileHandle, IntPtr ExistingCompletionPort, IntPtr CompletionKey, uint NumberOfConcurrentThreads);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetQueuedCompletionStatus(IntPtr CompletionPort, ref uint lpNumberOfBytes, ref IntPtr lpCompletionKey, ref IntPtr lpOverlapped, int dwMilliseconds);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool PostQueuedCompletionStatus(IntPtr CompletionPort, uint dwNumberOfBytesTransferred, IntPtr lpCompletionKey, IntPtr lpOverlapped);

		public static bool HeapFree(IntPtr lpMem)
		{
			return !(lpMem != IntPtr.Zero) || Win32NativeMethods.HeapFree(Win32NativeMethods.GetProcessHeap(), 0U, lpMem);
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool HeapFree(IntPtr hHeap, uint dwFlags, IntPtr lpMem);
	}
}
