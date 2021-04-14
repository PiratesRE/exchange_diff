using System;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal sealed class SafeNCryptHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal SafeNCryptHandle() : base(true)
		{
		}

		public SafeNCryptHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		protected override bool ReleaseHandle()
		{
			return CngNativeMethods.NCryptFreeObject(this.handle) == 0;
		}
	}
}
