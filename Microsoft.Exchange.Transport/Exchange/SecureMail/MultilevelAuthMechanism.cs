using System;

namespace Microsoft.Exchange.SecureMail
{
	internal enum MultilevelAuthMechanism
	{
		None,
		SenderId,
		MutualTLS,
		MapiSubmit,
		SecureMapiSubmit,
		SecureInternalSubmit,
		TLSAuthLogin,
		Login,
		Pickup,
		NTLM = 10,
		GSSAPI,
		MUTUALGSSAPI,
		Custom,
		SecureExternalSubmit = 16,
		DirectTrustTLS,
		Replay
	}
}
