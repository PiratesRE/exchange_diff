using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Win32.SafeHandles
{
	[SecurityCritical]
	internal sealed class SafePEFileHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private SafePEFileHandle() : base(true)
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void ReleaseSafePEFileHandle(IntPtr peFile);

		[SecurityCritical]
		protected override bool ReleaseHandle()
		{
			SafePEFileHandle.ReleaseSafePEFileHandle(this.handle);
			return true;
		}
	}
}
