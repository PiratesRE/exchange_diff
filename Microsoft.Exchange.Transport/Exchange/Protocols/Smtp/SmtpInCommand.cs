using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal enum SmtpInCommand
	{
		UNKNOWN,
		AUTH,
		BDAT,
		DATA,
		EHLO,
		EXPN,
		HELO,
		HELP,
		MAIL,
		NOOP,
		QUIT,
		RCPT,
		RSET,
		STARTTLS,
		VRFY,
		XANONYMOUSTLS,
		XEXCH50,
		XEXPS,
		XPROXY,
		XPROXYFROM,
		XPROXYTO,
		XQDISCARD,
		XSESSIONPARAMS,
		XSHADOW,
		XSHADOWREQUEST,
		RCPT2
	}
}
