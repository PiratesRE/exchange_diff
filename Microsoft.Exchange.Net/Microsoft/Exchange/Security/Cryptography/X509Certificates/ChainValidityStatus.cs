using System;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal enum ChainValidityStatus : uint
	{
		Valid,
		ValidSelfSigned,
		EmptyCertificate,
		SubjectMismatch,
		SignatureFailure = 2148098052U,
		UntrustedRoot = 2148204809U,
		UntrustedTestRoot = 2148204813U,
		InternalChainFailure = 2148204810U,
		WrongUsage = 2148204816U,
		CertificateExpired = 2148204801U,
		ValidityPeriodNesting,
		PurposeError = 2148204806U,
		BasicConstraintsError = 2148098073U,
		WrongRole = 2148204803U,
		NoCNMatch = 2148204815U,
		Revoked = 2148081680U,
		RevocationOffline = 2148081683U,
		CertificateRevoked = 2148204812U,
		RevocationFailure = 2148204814U,
		NoRevocationCheck = 2148081682U
	}
}
