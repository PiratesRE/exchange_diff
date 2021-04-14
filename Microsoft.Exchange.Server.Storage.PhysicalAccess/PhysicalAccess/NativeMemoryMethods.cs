using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	internal static class NativeMemoryMethods
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GlobalMemoryStatusEx(ref NativeMemoryMethods.MemoryStatusEx memoryStatusEx);

		private const string Kernel32 = "kernel32.dll";

		private const string PSAPI = "psapi.dll";

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct MemoryStatusEx
		{
			public void Init()
			{
				this.Length = Marshal.SizeOf(typeof(NativeMemoryMethods.MemoryStatusEx));
			}

			public int Length;

			public int MemoryLoad;

			public ulong TotalPhys;

			public ulong AvailPhys;

			public ulong TotalPageFile;

			public ulong AvailPageFile;

			public ulong TotalVirtual;

			public ulong AvailVirtual;

			public ulong AvailExtendedVirtual;
		}
	}
}
