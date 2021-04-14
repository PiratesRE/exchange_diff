using System;

namespace Microsoft.Exchange.Data.Transport
{
	public enum CertificateValidationStatus
	{
		Valid,
		ValidSelfSigned,
		EmptyCertificate,
		SubjectMismatch,
		SignatureFailure,
		UntrustedRoot,
		UntrustedTestRoot,
		InternalChainFailure,
		WrongUsage,
		CertificateExpired,
		ValidityPeriodNesting,
		PurposeError,
		BasicConstraintsError,
		WrongRole,
		NoCNMatch,
		Revoked,
		RevocationOffline,
		CertificateRevoked,
		RevocationFailure,
		NoRevocationCheck,
		ExchangeServerAuthCertificate,
		Other
	}
}
