using System;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal sealed class SafeCryptProvHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public SafeCryptProvHandle() : base(true)
		{
		}

		internal SafeCryptProvHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		public static SafeCryptProvHandle InvalidHandle
		{
			get
			{
				return new SafeCryptProvHandle(IntPtr.Zero);
			}
		}

		protected override bool ReleaseHandle()
		{
			return CapiNativeMethods.CryptReleaseContext(this.handle, 0U);
		}
	}
}
