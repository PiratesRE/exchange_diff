using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal sealed class SafeMsiHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal SafeMsiHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		private SafeMsiHandle() : base(true)
		{
		}

		protected override bool ReleaseHandle()
		{
			return SafeMsiHandle.CloseHandle(this.handle) == 0U;
		}

		[DllImport("msi", EntryPoint = "MsiCloseHandle")]
		private static extern uint CloseHandle(IntPtr any);
	}
}
