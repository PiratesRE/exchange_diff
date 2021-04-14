using System;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	[Flags]
	internal enum SSLPolicyAuthorizationOptions : uint
	{
		None = 0U,
		IgnoreRevocation = 128U,
		IgnoreUnknownCA = 256U,
		IgnoreWrongUsage = 512U,
		IgnoreCertCNInvalid = 4096U,
		IgnoreCertDateInvalid = 8192U
	}
}
