using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal class CertificateStore : IDisposable
	{
		public CertificateStore(StoreName storeName, StoreLocation storeLocation)
		{
			this.storeType = StoreType.System;
			this.store = new X509Store(storeName, storeLocation);
		}

		public CertificateStore(StoreType type, string serviceStoreName)
		{
			switch (type)
			{
			case StoreType.System:
				this.store = new X509Store(serviceStoreName, StoreLocation.LocalMachine);
				break;
			case StoreType.Memory:
				this.serviceStoreName = string.Empty;
				break;
			case StoreType.Service:
				if (string.IsNullOrEmpty(serviceStoreName))
				{
					throw new ArgumentNullException("serviceStoreName");
				}
				this.serviceStoreName = serviceStoreName;
				break;
			default:
				throw new ArgumentException(NetException.StoreTypeUnsupported, "type");
			}
			this.storeType = type;
		}

		public static X509Store Open(StoreType type, string path, OpenFlags flags)
		{
			CapiNativeMethods.CertificateStoreOptions certificateStoreOptions = (CapiNativeMethods.CertificateStoreOptions)0U;
			if ((flags & OpenFlags.OpenExistingOnly) == OpenFlags.OpenExistingOnly)
			{
				certificateStoreOptions |= CapiNativeMethods.CertificateStoreOptions.OpenExisting;
			}
			if ((flags & OpenFlags.ReadWrite) != OpenFlags.ReadWrite)
			{
				certificateStoreOptions |= CapiNativeMethods.CertificateStoreOptions.ReadOnly;
			}
			switch (type)
			{
			case StoreType.Memory:
				certificateStoreOptions |= CapiNativeMethods.CertificateStoreOptions.Create;
				return CertificateStore.StoreOpen(CapiNativeMethods.CertificateStoreProvider.Memory, certificateStoreOptions, null);
			case StoreType.Service:
				certificateStoreOptions |= CapiNativeMethods.CertificateStoreOptions.Services;
				return CertificateStore.StoreOpen(CapiNativeMethods.CertificateStoreProvider.System, certificateStoreOptions, path);
			case StoreType.Physical:
				certificateStoreOptions |= CapiNativeMethods.CertificateStoreOptions.LocalMachine;
				return CertificateStore.StoreOpen(CapiNativeMethods.CertificateStoreProvider.Physical, certificateStoreOptions, path);
			default:
				throw new ArgumentException(type.ToString(), "type");
			}
		}

		public void Dispose()
		{
			this.Close();
		}

		public int Bookmark
		{
			get
			{
				if (this.changed != null && this.changed.WaitOne(0, false))
				{
					this.version++;
					IntPtr intPtr = this.changed.SafeWaitHandle.DangerousGetHandle();
					using (SafeCertStoreHandle safeCertStoreHandle = SafeCertStoreHandle.Clone(this.store))
					{
						if (!CapiNativeMethods.CertControlStore(safeCertStoreHandle, 0U, CapiNativeMethods.StoreControl.Resync, ref intPtr))
						{
							int lastWin32Error = Marshal.GetLastWin32Error();
							ExTraceGlobals.CertificateTracer.TraceError<int>(0L, "Attempt to reset event notification on Certificate store failed with 0x{0:X}", lastWin32Error);
						}
					}
				}
				return this.version;
			}
		}

		public X509Store BaseStore
		{
			get
			{
				return this.store;
			}
		}

		public void Open(OpenFlags flags)
		{
			this.changed = new EventWaitHandle(false, EventResetMode.AutoReset);
			if (this.storeType == StoreType.System)
			{
				this.store.Open(flags);
			}
			else
			{
				this.store = CertificateStore.Open(this.storeType, this.serviceStoreName, flags);
			}
			IntPtr intPtr = this.changed.SafeWaitHandle.DangerousGetHandle();
			using (SafeCertStoreHandle safeCertStoreHandle = SafeCertStoreHandle.Clone(this.store))
			{
				if (!CapiNativeMethods.CertControlStore(safeCertStoreHandle, 0U, CapiNativeMethods.StoreControl.NotifiyChange, ref intPtr))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					ExTraceGlobals.CertificateTracer.TraceError<int>(0L, "Attempt to set event notification on Certificate store failed with 0x{0:X}", lastWin32Error);
				}
			}
		}

		public void Close()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing && this.store != null)
			{
				if (this.changed != null)
				{
					IntPtr intPtr = this.changed.SafeWaitHandle.DangerousGetHandle();
					using (SafeCertStoreHandle safeCertStoreHandle = SafeCertStoreHandle.Clone(this.store))
					{
						CapiNativeMethods.CertControlStore(safeCertStoreHandle, 0U, CapiNativeMethods.StoreControl.CancelNotifyChange, ref intPtr);
					}
					this.changed.Close();
					this.changed = null;
				}
				this.store.Close();
				this.store = null;
			}
		}

		private static X509Store StoreOpen(CapiNativeMethods.CertificateStoreProvider provider, CapiNativeMethods.CertificateStoreOptions options, string path)
		{
			X509Store result;
			using (SafeCertStoreHandle safeCertStoreHandle = CapiNativeMethods.CertOpenStore((IntPtr)((int)provider), CapiNativeMethods.EncodeType.X509Asn | CapiNativeMethods.EncodeType.Pkcs7Asn, IntPtr.Zero, options, path))
			{
				if (safeCertStoreHandle.IsInvalid)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (lastWin32Error == 2)
					{
						throw new ArgumentOutOfRangeException(path, "path");
					}
					throw new CryptographicException(lastWin32Error);
				}
				else
				{
					result = new X509Store(safeCertStoreHandle.DangerousGetHandle());
				}
			}
			return result;
		}

		private string serviceStoreName;

		private StoreType storeType;

		private X509Store store;

		private int version;

		private EventWaitHandle changed;
	}
}
