using System;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal enum ExchangeCertificateValidity
	{
		Valid,
		PrivateKeyMissing,
		KeyAlgorithmUnsupported,
		SigningNotSupported,
		PkixKpServerAuthNotFoundInEnhancedKeyUsage,
		PrivateKeyNotAccessible,
		PrivateKeyUnsupportedAlgorithm,
		CspKeyContainerInfoProtected,
		CspKeyContainerInfoRemovableDevice,
		CspKeyContainerInfoNotAccessible,
		CspKeyContainerInfoUnknownKeyNumber,
		PublicKeyUnsupportedSize,
		KeyUsageCorrupted,
		EnhancedKeyUsageCorrupted
	}
}
