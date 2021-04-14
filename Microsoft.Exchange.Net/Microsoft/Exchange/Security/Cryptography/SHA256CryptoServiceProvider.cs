using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Microsoft.Exchange.Security.Cryptography
{
	[ComVisible(false)]
	internal sealed class SHA256CryptoServiceProvider : SHA256
	{
		public SHA256CryptoServiceProvider()
		{
			this.handle = SafeHashHandle.Create(HashUtilities.StaticAESHandle, CapiNativeMethods.AlgorithmId.Sha256);
		}

		public override void Initialize()
		{
			if (this.handle != null && !this.handle.IsClosed)
			{
				this.handle.Dispose();
			}
			this.handle = SafeHashHandle.Create(HashUtilities.StaticAESHandle, CapiNativeMethods.AlgorithmId.Sha256);
		}

		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			this.handle.HashData(array, ibStart, cbSize);
		}

		protected override byte[] HashFinal()
		{
			return this.handle.HashFinal();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && this.handle != null && !this.handle.IsClosed)
			{
				this.handle.Dispose();
			}
		}

		private SafeHashHandle handle;
	}
}
