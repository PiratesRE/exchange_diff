using System;

namespace Microsoft.Exchange.Security.Authentication
{
	internal enum AuthenticationMechanism
	{
		None,
		Login,
		Negotiate,
		Ntlm,
		Kerberos,
		Certificate,
		Gssapi,
		Plain
	}
}
