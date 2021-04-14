using System;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	[Flags]
	internal enum CertificateSelectionOption
	{
		None = 0,
		WildcardAllowed = 1,
		PreferedNonSelfSigned = 2
	}
}
