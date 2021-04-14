using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum AuthenticationMethodFlags
	{
		None = 0,
		Basic = 1,
		Ntlm = 2,
		Fba = 4,
		Digest = 8,
		WindowsIntegrated = 16,
		LiveIdFba = 32,
		LiveIdBasic = 64,
		WSSecurity = 128,
		Certificate = 256,
		NegoEx = 512,
		OAuth = 1024,
		Adfs = 2048,
		Kerberos = 4096,
		Negotiate = 8192,
		LiveIdNegotiate = 16384
	}
}
