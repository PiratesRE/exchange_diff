using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.ExSmtpClient
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class SmtpConstants
	{
		internal static readonly string EhloCommand = "EHLO ";

		internal static readonly string XAnonymousTlsCommand = "X-ANONYMOUSTLS";

		internal static readonly string StartTlsCommand = "STARTTLS";

		internal static readonly string AuthCommand = "X-EXPS EXCHANGEAUTH SHA256 ";

		internal static readonly string AuthLoginCommand = "AUTH LOGIN ";

		internal static readonly string MailFromCommand = "MAIL FROM: ";

		internal static readonly string RcptToCommand = "RCPT TO: ";

		internal static readonly string BdatCommand = "BDAT {0} LAST\r\n";

		internal static readonly string RsetCommand = "RSET";

		internal static readonly string QuitCommand = "QUIT";

		internal static readonly string NoNDR = " NOTIFY=NEVER";

		internal static readonly string NDRForFailure = " NOTIFY=FAILURE";

		internal static readonly string XSHADOW = "XSHADOW ";

		internal static readonly string CrLf = "\r\n";

		internal static readonly string Cr = "\r";

		internal static readonly string Lf = "\n";

		internal static readonly string TargetSpn = "smtpsvc/{0}";

		internal static readonly int BufferSize = 16384;

		internal static readonly int NetworkingTimeout = 60000;
	}
}
