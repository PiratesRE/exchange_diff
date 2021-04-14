using System;

namespace Microsoft.Exchange.Rpc
{
	[CLSCompliant(false)]
	public enum HttpAuthenticationScheme : uint
	{
		Undefined,
		Basic,
		Ntlm,
		Passport = 4U,
		Digest = 8U,
		Negotiate = 16U,
		Certificate = 65536U
	}
}
