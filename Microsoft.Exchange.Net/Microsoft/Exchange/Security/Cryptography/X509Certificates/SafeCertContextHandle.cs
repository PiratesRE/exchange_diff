using System;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal sealed class SafeCertContextHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal SafeCertContextHandle() : base(true)
		{
		}

		public SafeCertContextHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		public static SafeCertContextHandle Clone(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				return new SafeCertContextHandle();
			}
			return CapiNativeMethods.CertDuplicateCertificateContext(handle);
		}

		protected override bool ReleaseHandle()
		{
			return CapiNativeMethods.CertFreeCertificateContext(this.handle);
		}
	}
}
