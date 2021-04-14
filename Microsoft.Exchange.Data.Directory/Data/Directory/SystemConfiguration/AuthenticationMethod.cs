using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum AuthenticationMethod
	{
		Basic,
		Digest,
		Ntlm,
		Fba,
		WindowsIntegrated,
		LiveIdFba,
		LiveIdBasic,
		WSSecurity,
		Certificate,
		NegoEx,
		OAuth,
		Adfs,
		Kerberos,
		Negotiate,
		LiveIdNegotiate,
		Misconfigured = 1000
	}
}
