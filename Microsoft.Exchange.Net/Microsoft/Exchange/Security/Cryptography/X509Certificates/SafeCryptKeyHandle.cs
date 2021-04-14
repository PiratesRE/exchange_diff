using System;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal sealed class SafeCryptKeyHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal SafeCryptKeyHandle() : base(true)
		{
		}

		public SafeCryptKeyHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		protected override bool ReleaseHandle()
		{
			return CapiNativeMethods.CryptDestroyKey(this.handle);
		}
	}
}
