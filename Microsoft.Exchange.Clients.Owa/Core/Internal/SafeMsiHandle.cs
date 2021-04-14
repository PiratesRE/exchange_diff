using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Clients.Owa.Core.Internal
{
	[ComVisible(false)]
	internal sealed class SafeMsiHandle : SafeHandleZeroOrMinusOneIsInvalid
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
			return SafeNativeMethods.MsiCloseHandle(this.handle) == 0;
		}
	}
}
