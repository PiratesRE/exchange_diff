using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public enum SmtpAuthenticationMechanism
	{
		None,
		Login,
		Gssapi,
		Ntlm,
		ExchangeAuth
	}
}
