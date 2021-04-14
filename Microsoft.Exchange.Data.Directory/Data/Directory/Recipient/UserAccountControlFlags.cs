using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Flags]
	public enum UserAccountControlFlags
	{
		None = 0,
		Script = 1,
		AccountDisabled = 2,
		HomeDirectoryRequired = 8,
		Lockout = 16,
		PasswordNotRequired = 32,
		CannotChangePassowrd = 64,
		EncryptedTextPasswordAllowed = 128,
		TemporaryDuplicateAccount = 256,
		NormalAccount = 512,
		InterDomainTrustAccount = 2048,
		WorkstationTrustAccount = 4096,
		ServerTrustAccount = 8192,
		DoNotExpirePassword = 65536,
		MnsLogonAccount = 131072,
		SmartCardRequired = 262144,
		TrustedForDelegation = 524288,
		NotDelegated = 1048576,
		UseDesKeyOnly = 2097152,
		DoNotRequirePreauthentication = 4194304,
		PasswordExpired = 8388608,
		TrustedToAuthenticateForDelegation = 16777216
	}
}
