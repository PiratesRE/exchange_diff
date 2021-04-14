using System;

namespace Microsoft.Exchange.Rpc
{
	[CLSCompliant(false)]
	public enum AuthenticationService : uint
	{
		None,
		DcePrivate,
		DcePublic,
		DecPublic = 4U,
		Negotiate = 9U,
		Ntlm,
		SecureChannel = 14U,
		Kerberos = 16U,
		Dpa,
		Msn,
		Kernel = 20U,
		Digest,
		NegoExtender = 30U,
		Pku2u,
		Live,
		MicrosoftOnline = 82U,
		MessageQueue = 100U,
		Default = 4294967295U
	}
}
