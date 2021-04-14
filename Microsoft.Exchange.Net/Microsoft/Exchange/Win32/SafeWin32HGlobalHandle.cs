using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Win32
{
	internal sealed class SafeWin32HGlobalHandle : SafeHandleZeroIsInvalid
	{
		private SafeWin32HGlobalHandle()
		{
		}

		internal SafeWin32HGlobalHandle(IntPtr handle)
		{
			base.SetHandle(handle);
		}

		private SafeWin32HGlobalHandle(IntPtr handle, bool ownsHandle) : base(handle, ownsHandle)
		{
		}

		public static SafeWin32HGlobalHandle InvalidHandle
		{
			get
			{
				return new SafeWin32HGlobalHandle(IntPtr.Zero, false);
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected override bool ReleaseHandle()
		{
			return SafeWin32HGlobalHandle.GlobalFree(this.handle) == IntPtr.Zero;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("KERNEL32.DLL", ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr GlobalFree([In] IntPtr hMem);
	}
}
