using System;

namespace Microsoft.Exchange.Data
{
	public enum EncryptionType
	{
		[LocDescription(DataStrings.IDs.EncryptionTypeSSL)]
		SSL,
		[LocDescription(DataStrings.IDs.EncryptionTypeTLS)]
		TLS
	}
}
