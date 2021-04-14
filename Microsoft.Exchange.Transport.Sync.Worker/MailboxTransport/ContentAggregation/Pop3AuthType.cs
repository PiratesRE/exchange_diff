using System;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
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
