using System;

namespace System.Security.Cryptography.X509Certificates
{
	internal enum OidGroup
	{
		AllGroups,
		HashAlgorithm,
		EncryptionAlgorithm,
		PublicKeyAlgorithm,
		SignatureAlgorithm,
		Attribute,
		ExtensionOrAttribute,
		EnhancedKeyUsage,
		Policy,
		Template,
		KeyDerivationFunction,
		DisableSearchDS = -2147483648
	}
}
