using System;

namespace Microsoft.Exchange.Security.Authorization
{
	public enum AccessTokenType
	{
		Windows,
		LiveId,
		LiveIdBasic,
		LiveIdNego2,
		OAuth,
		CompositeIdentity,
		Adfs,
		CertificateSid,
		RemotePowerShellDelegated,
		Anonymous
	}
}
