using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal class SafeChainEngineHandle : SafeHandleMinusOneIsInvalid
	{
		internal SafeChainEngineHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		private SafeChainEngineHandle() : base(true)
		{
		}

		public static SafeChainEngineHandle DefaultEngine
		{
			get
			{
				return new SafeChainEngineHandle(IntPtr.Zero);
			}
		}

		public bool IsDefaultEngine
		{
			get
			{
				return this.handle == IntPtr.Zero;
			}
		}

		public static SafeChainEngineHandle Create(ChainEnginePool.ChainEngineConfig configuration)
		{
			SafeChainEngineHandle result;
			if (!CapiNativeMethods.CertCreateCertificateChainEngine(ref configuration, out result))
			{
				throw new CryptographicException(Marshal.GetLastWin32Error());
			}
			return result;
		}

		protected override bool ReleaseHandle()
		{
			if (!this.IsDefaultEngine)
			{
				CapiNativeMethods.CertFreeCertificateChainEngine(this.handle);
			}
			return true;
		}
	}
}
