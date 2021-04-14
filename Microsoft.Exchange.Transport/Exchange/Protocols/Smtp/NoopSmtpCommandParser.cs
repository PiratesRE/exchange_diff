using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class NoopSmtpCommandParser
	{
		public static ParseResult Parse(CommandContext context, SmtpInSessionState state)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ArgumentValidator.ThrowIfNull("state", state);
			return ParseResult.ParsingComplete;
		}

		public const string CommandKeyword = "NOOP";
	}
}
