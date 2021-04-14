using System;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	[Flags]
	internal enum CertificateCreationOption
	{
		None = 0,
		Default = 0,
		RSAProvider = 1,
		DSSProvider = 2,
		Exportable = 4,
		Archivable = 8
	}
}
