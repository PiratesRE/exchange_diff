using System;

namespace Microsoft.Exchange.Data
{
	public enum TlsAuthLevel
	{
		[LocDescription(DataStrings.IDs.TlsAuthLevelEncryptionOnly)]
		EncryptionOnly = 1,
		[LocDescription(DataStrings.IDs.TlsAuthLevelCertificateValidation)]
		CertificateValidation,
		[LocDescription(DataStrings.IDs.TlsAuthLevelDomainValidation)]
		DomainValidation = 4
	}
}
