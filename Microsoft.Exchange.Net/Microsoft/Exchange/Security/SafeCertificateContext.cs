using System;

namespace Microsoft.Exchange.Security
{
	internal sealed class SafeCertificateContext : DebugSafeHandle
	{
		internal unsafe SafeCertificateContext(byte[] memory)
		{
			fixed (IntPtr* ptr = memory)
			{
				IntPtr value = new IntPtr((void*)ptr);
				this.handle = *(IntPtr*)((void*)value);
			}
		}

		protected override bool ReleaseHandle()
		{
			return SspiNativeMethods.CertFreeCertificateContext(this.handle);
		}
	}
}
