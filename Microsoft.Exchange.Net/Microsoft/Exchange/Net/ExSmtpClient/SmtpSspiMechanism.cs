using System;

namespace Microsoft.Exchange.Net.ExSmtpClient
{
	[Flags]
	internal enum SmtpSspiMechanism
	{
		Login = 1,
		Ntlm = 2,
		Gssapi = 4,
		TLS = 8,
		None = 16,
		Kerberos = 32
	}
}
