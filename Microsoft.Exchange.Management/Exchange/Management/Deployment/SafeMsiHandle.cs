using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Management.Deployment
{
	internal class SafeMsiHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private SafeMsiHandle() : base(true)
		{
		}

		internal SafeMsiHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		protected override bool ReleaseHandle()
		{
			return SafeMsiHandle.CloseHandle(this.handle) == 0U;
		}

		[DllImport("msi", EntryPoint = "MsiCloseHandle")]
		private static extern uint CloseHandle(IntPtr any);
	}
}
