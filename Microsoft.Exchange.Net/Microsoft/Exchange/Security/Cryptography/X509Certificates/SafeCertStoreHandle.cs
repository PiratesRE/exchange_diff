using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal sealed class SafeCertStoreHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public SafeCertStoreHandle() : base(true)
		{
		}

		public SafeCertStoreHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		public static SafeCertStoreHandle InvalidHandle
		{
			get
			{
				return new SafeCertStoreHandle(IntPtr.Zero);
			}
		}

		public static SafeCertStoreHandle Clone(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				return new SafeCertStoreHandle();
			}
			return CapiNativeMethods.CertDuplicateStore(handle);
		}

		public static SafeCertStoreHandle Clone(X509Store store)
		{
			if (store == null || store.StoreHandle == IntPtr.Zero)
			{
				return new SafeCertStoreHandle();
			}
			return CapiNativeMethods.CertDuplicateStore(store.StoreHandle);
		}

		protected override bool ReleaseHandle()
		{
			return CapiNativeMethods.CertCloseStore(this.handle, CapiNativeMethods.CertCloseStoreFlag.CertCloseStoreCheckFlag);
		}
	}
}
