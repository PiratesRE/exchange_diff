using System;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal enum WellKnownChainPolicy
	{
		Base = 1,
		Authenticode,
		AuthenticodeTS,
		SSL,
		BasicConstraints,
		NTAuthorization,
		MicrosoftRoot
	}
}
