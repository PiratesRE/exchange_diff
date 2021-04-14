using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Transport.Logging;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class AuthSmtpCommandParser
	{
		public static ParseResult ParseAuthMechanism(CommandContext context, SmtpInSessionState sessionState, AuthCommand command, out AuthParseOutput parseOutput)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			Offset offset;
			if (!context.GetNextArgumentOffset(out offset))
			{
				context.LogReceivedCommand(sessionState.ProtocolLogSession);
				sessionState.Tracer.TraceError(0L, "No auth mechanism specified after auth or x-exps verb");
				parseOutput = null;
				return ParseResult.AuthUnrecognized;
			}
			byte[] array = new byte[offset.End - context.OriginalOffset];
			Buffer.BlockCopy(context.Command, context.OriginalOffset, array, 0, array.Length);
			sessionState.ProtocolLogSession.LogReceive(array);
			if (command == AuthCommand.XExps && !sessionState.ReceiveConnector.HasExchangeServerAuthMechanism)
			{
				sessionState.Tracer.TraceError(0L, "X-EXPS command can only appear if Exchange Server auth mechanisms is set");
				parseOutput = null;
				return ParseResult.AuthUnrecognized;
			}
			if (SmtpInSessionUtils.IsAuthenticated(sessionState.RemoteIdentity))
			{
				switch (sessionState.AuthMethod)
				{
				case MultilevelAuthMechanism.TLSAuthLogin:
				case MultilevelAuthMechanism.Login:
				case MultilevelAuthMechanism.NTLM:
				case MultilevelAuthMechanism.GSSAPI:
				case MultilevelAuthMechanism.MUTUALGSSAPI:
					parseOutput = null;
					return ParseResult.AuthAlreadySpecified;
				}
			}
			if (AuthSmtpCommandParser.IsAuthMechanism(context, offset, AuthSmtpCommandParser.Login))
			{
				return AuthSmtpCommandParser.AuthLoginDetected(context, sessionState.ReceiveConnector, sessionState.SecureState, command, sessionState.ProtocolLogSession, out parseOutput);
			}
			if (AuthSmtpCommandParser.IsAuthMechanism(context, offset, AuthSmtpCommandParser.ExchangeAuth))
			{
				return AuthSmtpCommandParser.ExchangeAuthDetected(context, sessionState, command, out parseOutput);
			}
			if (AuthSmtpCommandParser.IsAuthMechanism(context, offset, AuthSmtpCommandParser.GSSAPI))
			{
				return AuthSmtpCommandParser.GssapiAuthDetected(context, sessionState, command, out parseOutput);
			}
			if (AuthSmtpCommandParser.IsAuthMechanism(context, offset, AuthSmtpCommandParser.NTLM))
			{
				return AuthSmtpCommandParser.NtlmAuthDetected(context, sessionState, command, out parseOutput);
			}
			sessionState.Tracer.TraceError(0L, "Auth mechanism not supported");
			parseOutput = null;
			return ParseResult.AuthUnrecognized;
		}

		private static bool IsAuthMechanism(CommandContext context, Offset nextArgumentOffset, byte[] mechanism)
		{
			return BufferParser.CompareArg(mechanism, context.Command, nextArgumentOffset.Start, nextArgumentOffset.Length);
		}

		private static ParseResult AuthLoginDetected(CommandContext context, ReceiveConnector receiveConnector, SecureState secureState, AuthCommand command, IProtocolLogSession protocolLogSession, out AuthParseOutput parseOutput)
		{
			if (command == AuthCommand.XExps || !receiveConnector.HasBasicAuthAuthMechanism)
			{
				parseOutput = null;
				return ParseResult.AuthUnrecognized;
			}
			bool flag = secureState == SecureState.StartTls || secureState == SecureState.AnonymousTls;
			bool hasBasicAuthRequireTlsAuthMechanism = receiveConnector.HasBasicAuthRequireTlsAuthMechanism;
			MultilevelAuthMechanism multilevelAuthMechanism;
			if (flag)
			{
				multilevelAuthMechanism = MultilevelAuthMechanism.TLSAuthLogin;
			}
			else
			{
				if (hasBasicAuthRequireTlsAuthMechanism)
				{
					protocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Inbound ExchangeAuth negotiation failed because connection is not TLS");
					parseOutput = null;
					return ParseResult.AuthUnrecognized;
				}
				multilevelAuthMechanism = MultilevelAuthMechanism.Login;
			}
			context.TrimLeadingWhitespace();
			parseOutput = new AuthParseOutput(SmtpAuthenticationMechanism.Login, multilevelAuthMechanism, context, null);
			return ParseResult.ParsingComplete;
		}

		private static ParseResult ExchangeAuthDetected(CommandContext context, SmtpInSessionState sessionState, AuthCommand command, out AuthParseOutput parseOutput)
		{
			if (command == AuthCommand.Auth || !SmtpInSessionUtils.ShouldExpsExchangeAuthBeAdvertised(sessionState.ReceiveConnector.AuthMechanism, sessionState.SecureState, sessionState.Configuration.TransportConfiguration.ProcessTransportRole))
			{
				parseOutput = null;
				return ParseResult.AuthUnrecognized;
			}
			string exchangeAuthHashAlgorithm;
			if (!context.GetNextArgument(out exchangeAuthHashAlgorithm))
			{
				parseOutput = null;
				sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Inbound ExchangeAuth negotiation failed because client did not specify hash algorithm");
				return ParseResult.AuthUnrecognized;
			}
			context.TrimLeadingWhitespace();
			if (context.Length == 0)
			{
				parseOutput = null;
				return ParseResult.AuthUnrecognized;
			}
			parseOutput = new AuthParseOutput(SmtpAuthenticationMechanism.ExchangeAuth, MultilevelAuthMechanism.MUTUALGSSAPI, context, exchangeAuthHashAlgorithm);
			return ParseResult.ParsingComplete;
		}

		private static ParseResult GssapiAuthDetected(CommandContext context, SmtpInSessionState sessionState, AuthCommand command, out AuthParseOutput parseOutput)
		{
			if (command == AuthCommand.XExps)
			{
				if (!SmtpInSessionUtils.DoesRoleSupportInboundXExpsGssapi(sessionState.Configuration.TransportConfiguration.ProcessTransportRole))
				{
					parseOutput = null;
					return ParseResult.AuthUnrecognized;
				}
			}
			else if (command == AuthCommand.Auth && (!sessionState.ReceiveConnector.HasIntegratedAuthMechanism || !sessionState.ReceiveConnector.EnableAuthGSSAPI))
			{
				parseOutput = null;
				return ParseResult.AuthUnrecognized;
			}
			context.TrimLeadingWhitespace();
			parseOutput = new AuthParseOutput(SmtpAuthenticationMechanism.Gssapi, MultilevelAuthMechanism.GSSAPI, context, null);
			return ParseResult.ParsingComplete;
		}

		private static ParseResult NtlmAuthDetected(CommandContext context, SmtpInSessionState sessionState, AuthCommand command, out AuthParseOutput parseOutput)
		{
			if (command == AuthCommand.Auth && !sessionState.IsIntegratedAuthSupported)
			{
				parseOutput = null;
				return ParseResult.AuthUnrecognized;
			}
			context.TrimLeadingWhitespace();
			parseOutput = new AuthParseOutput(SmtpAuthenticationMechanism.Ntlm, MultilevelAuthMechanism.NTLM, context, null);
			return ParseResult.ParsingComplete;
		}

		public const string ExpsCommandKeyword = "X-EXPS";

		public const string AuthCommandKeyword = "AUTH";

		public const string LoginKeyword = "login";

		public const string ExchangeAuthKeyword = "exchangeauth";

		public const string GSSAPIKeyword = "gssapi";

		public const string NTLMKeyword = "ntlm";

		public static readonly byte[] Login = Util.AsciiStringToBytes("login");

		public static readonly byte[] ExchangeAuth = Util.AsciiStringToBytes("exchangeauth");

		public static readonly byte[] GSSAPI = Util.AsciiStringToBytes("gssapi");

		public static readonly byte[] NTLM = Util.AsciiStringToBytes("ntlm");
	}
}
