using System;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal sealed class SafeChainContextHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal SafeChainContextHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		private SafeChainContextHandle() : base(true)
		{
		}

		protected override bool ReleaseHandle()
		{
			CapiNativeMethods.CertFreeCertificateChain(this.handle);
			return true;
		}
	}
}
