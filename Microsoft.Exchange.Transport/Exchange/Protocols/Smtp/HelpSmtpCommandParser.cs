using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class HelpSmtpCommandParser
	{
		public static ParseResult Parse(CommandContext context, SmtpInSessionState state, out string helpArgs)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ArgumentValidator.ThrowIfNull("state", state);
			context.TrimLeadingWhitespace();
			context.GetCommandArguments(out helpArgs);
			return ParseResult.ParsingComplete;
		}

		public const string CommandKeyword = "HELP";
	}
}
