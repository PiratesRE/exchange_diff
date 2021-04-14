using System;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	[Flags]
	internal enum TrustStatus : uint
	{
		Valid = 0U,
		IsNotTimeValid = 1U,
		IsNotTimeNested = 2U,
		IsRevoked = 4U,
		IsNotSignatureValid = 8U,
		IsNotValidForUsage = 16U,
		IsUntrustedRoot = 32U,
		X509RevocationStatusUnknown = 64U,
		IsCyclic = 128U,
		InvalidExtension = 256U,
		InvalidPolicyConstraints = 512U,
		InvalidBasicConstraints = 1024U,
		InvalidNameConstraints = 2048U,
		HasNotSupportedNameConstraint = 4096U,
		HasNotDefinedNameConstraint = 8192U,
		HasNotPermittedNameConstraint = 16384U,
		HasExcludedNameConstraint = 32768U,
		IsOfflineRevocation = 16777216U,
		NoIssuanceChainPolicy = 33554432U,
		IsPartialChain = 65536U,
		CTLIsNotTimeValid = 131072U,
		CTLIsNotSignatureValid = 262144U,
		CTLIsNotValidForUsage = 524288U
	}
}
