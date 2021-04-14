using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography.X509Certificates
{
	[ComVisible(true)]
	public enum X509ContentType
	{
		Unknown,
		Cert,
		SerializedCert,
		Pfx,
		Pkcs12 = 3,
		SerializedStore,
		Pkcs7,
		Authenticode
	}
}
