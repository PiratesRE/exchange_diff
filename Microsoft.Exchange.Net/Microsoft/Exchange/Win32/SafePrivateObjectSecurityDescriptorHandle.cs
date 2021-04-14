using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Win32
{
	internal class SafePrivateObjectSecurityDescriptorHandle : SafeHandleZeroIsInvalid
	{
		protected override bool ReleaseHandle()
		{
			return SafePrivateObjectSecurityDescriptorHandle.DestroyPrivateObjectSecurity(ref this.handle);
		}

		[DllImport("AdvApi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool DestroyPrivateObjectSecurity([In] ref IntPtr toDestroy);
	}
}
