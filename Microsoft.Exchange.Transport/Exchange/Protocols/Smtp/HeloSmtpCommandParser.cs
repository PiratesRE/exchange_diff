using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class HeloSmtpCommandParser
	{
		public static ParseResult Parse(CommandContext context, SmtpInSessionState sessionState, HeloOrEhlo heloOrEhlo, out HeloEhloParseOutput parseOutput)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			ParseResult result = HeloSmtpCommandParser.ValidateTlsCipherKeySize(sessionState);
			if (result.IsFailed)
			{
				parseOutput = null;
				return result;
			}
			SmtpReceiveCapabilities tlsDomainCapabilities;
			result = HeloSmtpCommandParser.ValidateRemoteTlsCertificate(sessionState, out tlsDomainCapabilities);
			if (result.IsFailed)
			{
				parseOutput = null;
				return result;
			}
			result = HeloSmtpCommandParser.ValidateAuthenticatedViaDirectTrustCertificate(sessionState, heloOrEhlo);
			if (result.IsFailed)
			{
				parseOutput = null;
				return result;
			}
			string heloDomain;
			result = HeloSmtpCommandParser.ValidateArguments(context, sessionState, heloOrEhlo, out heloDomain);
			if (result.IsFailed)
			{
				parseOutput = null;
				return result;
			}
			parseOutput = new HeloEhloParseOutput(heloDomain, tlsDomainCapabilities);
			return ParseResult.ParsingComplete;
		}

		public static bool IsValidHeloDomain(string helloDomain, HeloOrEhlo heloOrEhlo, bool allowUtf8)
		{
			return ((allowUtf8 && heloOrEhlo != HeloOrEhlo.Helo) || !RoutingAddress.IsUTF8Address(helloDomain)) && (RoutingAddress.IsValidDomain(helloDomain) || RoutingAddress.IsDomainIPLiteral(helloDomain) || HeloCommandEventArgs.IsValidIpv6WindowsAddress(helloDomain));
		}

		private static ParseResult ValidateAuthenticatedViaDirectTrustCertificate(SmtpInSessionState sessionState, HeloOrEhlo heloOrEhlo)
		{
			if (heloOrEhlo == HeloOrEhlo.Ehlo && sessionState.SecureState == SecureState.AnonymousTls && sessionState.TlsRemoteCertificateInternal != null && sessionState.RemoteIdentity == SmtpConstants.AnonymousSecurityIdentifier)
			{
				sessionState.DisconnectReason = DisconnectReason.DroppedSession;
				return ParseResult.AuthTempFailureDisconnect;
			}
			return ParseResult.ParsingComplete;
		}

		private static ParseResult ValidateTlsCipherKeySize(SmtpInSessionState sessionState)
		{
			if (sessionState.IsSecureSession && sessionState.NetworkConnection.TlsCipherKeySize < 128)
			{
				sessionState.Tracer.TraceError<int>((long)sessionState.GetHashCode(), "Negotiated TLS cipher strength is too weak at {0} bits.", sessionState.NetworkConnection.TlsCipherKeySize);
				sessionState.DisconnectReason = DisconnectReason.DroppedSession;
				return ParseResult.TlsCipherKeySizeTooShortDisconnect;
			}
			return ParseResult.ParsingComplete;
		}

		public static ParseResult ValidateRemoteTlsCertificate(SmtpInSessionState sessionState, out SmtpReceiveCapabilities tlsDomainCapabilities)
		{
			if (sessionState.SecureState == SecureState.StartTls && !sessionState.TryCalculateTlsDomainCapabilitiesFromRemoteTlsCertificate(out tlsDomainCapabilities))
			{
				sessionState.DisconnectReason = DisconnectReason.DroppedSession;
				return ParseResult.CertificateValidationFailureDisconnect;
			}
			tlsDomainCapabilities = SmtpReceiveCapabilities.None;
			return ParseResult.ParsingComplete;
		}

		private static ParseResult ValidateArguments(CommandContext context, SmtpInSessionState sessionState, HeloOrEhlo heloOrEhlo, out string heloDomain)
		{
			context.TrimLeadingWhitespace();
			if (context.GetCommandArguments(out heloDomain))
			{
				if (!HeloSmtpCommandParser.IsValidHeloDomain(heloDomain, heloOrEhlo, sessionState.ReceiveConnector.SmtpUtf8Enabled))
				{
					heloDomain = string.Empty;
					if (heloOrEhlo != HeloOrEhlo.Helo)
					{
						return ParseResult.InvalidEhloDomain;
					}
					return ParseResult.InvalidHeloDomain;
				}
			}
			else if (sessionState.ReceiveConnector.RequireEHLODomain)
			{
				if (heloOrEhlo != HeloOrEhlo.Helo)
				{
					return ParseResult.EhloDomainRequired;
				}
				return ParseResult.HeloDomainRequired;
			}
			return ParseResult.ParsingComplete;
		}

		public const string HeloCommandKeyword = "HELO";

		public const string EhloCommandKeyword = "EHLO";
	}
}
