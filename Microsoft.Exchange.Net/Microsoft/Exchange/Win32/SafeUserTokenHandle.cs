using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Win32
{
	internal sealed class SafeUserTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public SafeUserTokenHandle() : base(true)
		{
		}

		public SafeUserTokenHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected override bool ReleaseHandle()
		{
			return SafeUserTokenHandle.CloseHandle(this.handle);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("KERNEL32.DLL", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseHandle(IntPtr handle);

		internal enum LogonType
		{
			LogonService = 5
		}

		internal enum LogonProvider
		{
			Default
		}
	}
}
