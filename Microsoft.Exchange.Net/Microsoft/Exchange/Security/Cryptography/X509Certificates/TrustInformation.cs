using System;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	[Flags]
	internal enum TrustInformation : uint
	{
		None = 0U,
		HasExactMatchIssuer = 1U,
		HasKeyMatchIssuer = 2U,
		HasNameMatchIssuer = 4U,
		IsSelfSigned = 8U,
		HasPreferredIssuer = 256U,
		HasIssuanceChainPolicy = 512U,
		HasValidNameConstraints = 1024U,
		IsComplexChain = 65536U
	}
}
