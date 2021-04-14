using System;

namespace Microsoft.Exchange.Net.ExSmtpClient
{
	internal enum SmtpCommandType
	{
		Connect,
		Ehlo,
		Mail,
		Recipient,
		BDAT,
		Quit,
		XAnonymousTls,
		Custom,
		UnInit,
		XSHADOW,
		STARTTLS
	}
}
