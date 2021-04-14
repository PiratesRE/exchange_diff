using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Win32
{
	internal abstract class SafeHGlobalHandleBase : SafeHandleZeroIsInvalid
	{
		protected SafeHGlobalHandleBase()
		{
		}

		protected SafeHGlobalHandleBase(IntPtr handle)
		{
			base.SetHandle(handle);
		}

		protected SafeHGlobalHandleBase(IntPtr handle, bool ownsHandle) : base(handle, ownsHandle)
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected override bool ReleaseHandle()
		{
			return SafeHGlobalHandleBase.LocalFree(this.handle) == IntPtr.Zero;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("KERNEL32.DLL", ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr LocalFree([In] IntPtr hMem);
	}
}
