using System;

namespace Microsoft.Exchange.Connections.Pop
{
	internal enum Pop3AuthType
	{
		Basic,
		Ntlm,
		SSL = 16,
		NtlmOverSSL,
		TLS = 32,
		NtlmOverTLS
	}
}
