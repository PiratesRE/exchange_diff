using System;

namespace Microsoft.Exchange.Data.Transport
{
	internal enum RequiredTlsAuthLevel
	{
		EncryptionOnly = 1,
		CertificateValidation,
		DomainValidation
	}
}
