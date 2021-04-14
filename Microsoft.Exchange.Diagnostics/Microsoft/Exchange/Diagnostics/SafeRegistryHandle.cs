using System;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Diagnostics
{
	internal sealed class SafeRegistryHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public SafeRegistryHandle() : base(true)
		{
		}

		public SafeRegistryHandle(IntPtr preexistingHandle, bool ownsHandle) : base(ownsHandle)
		{
			base.SetHandle(preexistingHandle);
		}

		public static SafeRegistryHandle LocalMachine
		{
			get
			{
				return new SafeRegistryHandle(new IntPtr(-2147483646), false);
			}
		}

		protected override bool ReleaseHandle()
		{
			DiagnosticsNativeMethods.ErrorCode errorCode = DiagnosticsNativeMethods.RegCloseKey(this.handle);
			return errorCode == DiagnosticsNativeMethods.ErrorCode.Success;
		}
	}
}
