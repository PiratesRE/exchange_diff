using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class QuitSmtpCommandParser
	{
		public static ParseResult Parse(CommandContext context, SmtpInSessionState state)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ArgumentValidator.ThrowIfNull("state", state);
			if (context.HasArguments)
			{
				return ParseResult.InvalidArguments;
			}
			return ParseResult.ParsingComplete;
		}

		public const string CommandKeyword = "QUIT";
	}
}
