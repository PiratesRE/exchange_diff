using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.BITS
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("F1BD1079-9F01-4BDC-8036-F09B70095066")]
	[ComImport]
	internal interface IBackgroundCopyJobHttpOptions
	{
		void SetClientCertificateByID(BG_CERT_STORE_LOCATION StoreLocation, [MarshalAs(UnmanagedType.LPWStr)] [In] string StoreName, [In] IntPtr pCertHashBlob);

		void SetClientCertificateByName(BG_CERT_STORE_LOCATION StoreLocation, [MarshalAs(UnmanagedType.LPWStr)] [In] string StoreName, [MarshalAs(UnmanagedType.LPWStr)] [In] string SubjectName);

		void RemoveClientCertificate();

		void GetClientCertificate(out BG_CERT_STORE_LOCATION pStoreLocation, [MarshalAs(UnmanagedType.LPWStr)] out string pStoreName, out IntPtr ppCertHashBlob, [MarshalAs(UnmanagedType.LPWStr)] out string pSubjectName);

		void SetCustomHeaders([MarshalAs(UnmanagedType.LPWStr)] [In] string RequestHeaders);

		void GetCustomHeaders([MarshalAs(UnmanagedType.LPWStr)] out string pRequestHeaders);

		void SetSecurityFlags(ulong Flags);

		void GetSecurityFlags(out ulong pFlags);
	}
}
