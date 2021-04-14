using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography.X509Certificates
{
	internal struct CRYPT_OID_INFO
	{
		internal int cbSize;

		[MarshalAs(UnmanagedType.LPStr)]
		internal string pszOID;

		[MarshalAs(UnmanagedType.LPWStr)]
		internal string pwszName;

		internal OidGroup dwGroupId;

		internal int AlgId;

		internal int cbData;

		internal IntPtr pbData;
	}
}
