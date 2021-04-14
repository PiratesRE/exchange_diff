using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class StartTlsSmtpCommandParser
	{
		public static ParseResult Parse(CommandContext context, SmtpInSessionState state, SecureState startTlsOrAnonymousTls)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ArgumentValidator.ThrowIfNull("state", state);
			if (context.HasArguments)
			{
				return StartTlsSmtpCommandParser.InvalidArguments;
			}
			if ((startTlsOrAnonymousTls == SecureState.StartTls && !state.IsStartTlsSupported) || (startTlsOrAnonymousTls == SecureState.AnonymousTls && !state.IsAnonymousTlsSupported))
			{
				return StartTlsSmtpCommandParser.UnrecognizedCommand;
			}
			return StartTlsSmtpCommandParser.ParsingComplete;
		}

		public const string AnonymousTlsCommandKeyword = "X-ANONYMOUSTLS";

		public const string StartTlsCommandKeyword = "STARTTLS";

		public static readonly ParseResult ParsingComplete = new ParseResult(ParsingStatus.Complete, SmtpResponse.Empty, false);

		public static readonly ParseResult InvalidArguments = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.InvalidArguments, false);

		public static readonly ParseResult UnrecognizedCommand = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.UnrecognizedCommand, false);
	}
}
