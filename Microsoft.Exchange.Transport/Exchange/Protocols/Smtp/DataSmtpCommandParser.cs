using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class DataSmtpCommandParser
	{
		public static ParseResult Parse(CommandContext context, SmtpInSessionState state)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ArgumentValidator.ThrowIfNull("state", state);
			ArgumentValidator.ThrowIfNull("state.TransportMailItem", state.TransportMailItem);
			if (state.TransportMailItem.BodyType == BodyType.BinaryMIME)
			{
				return ParseResult.BadCommandSequence;
			}
			if (context.HasArguments)
			{
				return ParseResult.InvalidArguments;
			}
			return ParseResult.MoreDataRequired;
		}

		public const string CommandKeyword = "DATA";
	}
}
