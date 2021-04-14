using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	public enum AuthMechanisms
	{
		[LocDescription(DirectoryStrings.IDs.ReceiveAuthMechanismNone)]
		None = 0,
		[LocDescription(DirectoryStrings.IDs.ReceiveAuthMechanismTls)]
		Tls = 1,
		[LocDescription(DirectoryStrings.IDs.ReceiveAuthMechanismIntegrated)]
		Integrated = 2,
		[LocDescription(DirectoryStrings.IDs.ReceiveAuthMechanismBasicAuth)]
		BasicAuth = 4,
		[LocDescription(DirectoryStrings.IDs.ReceiveAuthMechanismBasicAuthPlusTls)]
		BasicAuthRequireTLS = 8,
		[LocDescription(DirectoryStrings.IDs.ReceiveAuthMechanismExchangeServer)]
		ExchangeServer = 16,
		[LocDescription(DirectoryStrings.IDs.ReceiveAuthMechanismExternalAuthoritative)]
		ExternalAuthoritative = 32
	}
}
