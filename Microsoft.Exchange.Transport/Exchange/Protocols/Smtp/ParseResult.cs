using System;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal struct ParseResult : IEquatable<ParseResult>
	{
		public ParseResult(ParsingStatus parsingStatus, SmtpResponse smtpResponse, bool disconnectClient = false)
		{
			this = default(ParseResult);
			this.ParsingStatus = parsingStatus;
			this.SmtpResponse = smtpResponse;
			this.DisconnectClient = disconnectClient;
		}

		public ParsingStatus ParsingStatus { get; private set; }

		public SmtpResponse SmtpResponse { get; private set; }

		public bool DisconnectClient { get; private set; }

		public bool IsFailed
		{
			get
			{
				return this.ParsingStatus == ParsingStatus.Error || this.ParsingStatus == ParsingStatus.ProtocolError || this.ParsingStatus == ParsingStatus.IgnorableProtocolError;
			}
		}

		public bool IsIgnorableFailure
		{
			get
			{
				return this.ParsingStatus == ParsingStatus.IgnorableProtocolError;
			}
		}

		public override string ToString()
		{
			return string.Format("{0}, {1}, {2}", this.ParsingStatus, this.SmtpResponse, this.DisconnectClient);
		}

		public override bool Equals(object other)
		{
			return other is ParseResult && this.Equals((ParseResult)other);
		}

		public override int GetHashCode()
		{
			return 17 + 31 * this.SmtpResponse.GetHashCode() + 31 * this.ParsingStatus.GetHashCode() + 31 * this.DisconnectClient.GetHashCode();
		}

		public static bool operator ==(ParseResult lhs, ParseResult rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(ParseResult lhs, ParseResult rhs)
		{
			return !lhs.Equals(rhs);
		}

		public bool Equals(ParseResult other)
		{
			return this.ParsingStatus.Equals(other.ParsingStatus) && this.SmtpResponse.Equals(other.SmtpResponse) && this.DisconnectClient.Equals(other.DisconnectClient);
		}

		public static readonly ParseResult ParsingComplete = new ParseResult(ParsingStatus.Complete, SmtpResponse.Empty, false);

		public static readonly ParseResult MoreDataRequired = new ParseResult(ParsingStatus.MoreDataRequired, SmtpResponse.Empty, false);

		public static readonly ParseResult InvalidArguments = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.InvalidArguments, false);

		public static readonly ParseResult UnrecognizedParameter = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.UnrecognizedParameter, false);

		public static readonly ParseResult RequiredArgumentsNotPresent = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.RequiredArgumentsNotPresent, false);

		public static readonly ParseResult CommandNotImplemented = new ParseResult(ParsingStatus.Complete, SmtpResponse.CommandNotImplemented, false);

		public static readonly ParseResult CommandNotImplementedProtocolError = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.CommandNotImplemented, false);

		public static readonly ParseResult NotAuthorized = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.NotAuthorized, false);

		public static readonly ParseResult BadCommandSequence = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.BadCommandSequence, false);

		public static readonly ParseResult InvalidHeloDomain = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.InvalidHeloDomain, false);

		public static readonly ParseResult HeloDomainRequired = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.HeloDomainRequired, false);

		public static readonly ParseResult InvalidEhloDomain = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.InvalidEhloDomain, false);

		public static readonly ParseResult EhloDomainRequired = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.EhloDomainRequired, false);

		public static readonly ParseResult AuthTempFailure = new ParseResult(ParsingStatus.Error, SmtpResponse.AuthTempFailure, false);

		public static readonly ParseResult AuthTempFailure2 = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.AuthTempFailure, false);

		public static readonly ParseResult AuthUnrecognized = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.AuthUnrecognized, false);

		public static readonly ParseResult AuthUnsuccessful = new ParseResult(ParsingStatus.Error, SmtpResponse.AuthUnsuccessful, false);

		public static readonly ParseResult AuthAlreadySpecified = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.AuthAlreadySpecified, false);

		public static readonly ParseResult MailFromAlreadySpecified = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.MailFromAlreadySpecified, false);

		public static readonly ParseResult MessageRateLimitExceeded = new ParseResult(ParsingStatus.Error, SmtpResponse.MessageRateLimitExceeded, false);

		public static readonly ParseResult MessageTooLarge = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.MessageTooLarge, false);

		public static readonly ParseResult UnsupportedBodyType = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.UnsupportedBodyType, false);

		public static readonly ParseResult InvalidAddress = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.InvalidAddress, false);

		public static readonly ParseResult LongAddress = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.LongAddress, false);

		public static readonly ParseResult Utf8Address = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.Utf8Address, false);

		public static readonly ParseResult TooManyRecipients = new ParseResult(ParsingStatus.Error, SmtpResponse.TooManyRecipients, false);

		public static readonly ParseResult TooManyRelatedErrors = new ParseResult(ParsingStatus.Error, SmtpResponse.TooManyRelatedErrors, false);

		public static readonly ParseResult TooManyAuthenticationErrors = new ParseResult(ParsingStatus.Error, SmtpResponse.TooManyAuthenticationErrors, false);

		public static readonly ParseResult RcptRelayNotPermitted = new ParseResult(ParsingStatus.Error, SmtpResponse.RcptRelayNotPermitted, false);

		public static readonly ParseResult RequireTlsToSendMail = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.RequireTLSToSendMail, false);

		public static readonly ParseResult OrarNotAuthorized = new ParseResult(ParsingStatus.Error, SmtpResponse.OrarNotAuthorized, false);

		public static readonly ParseResult RDstNotAuthorized = new ParseResult(ParsingStatus.Error, SmtpResponse.RDstNotAuthorized, false);

		public static readonly ParseResult UserLookupFailed = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.UserLookupFailed, false);

		public static readonly ParseResult UnableToObtainIdentity = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.UnableToObtainIdentity, false);

		public static readonly ParseResult XProxyAcceptedAuthenticated = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.XProxyAcceptedAuthenticated, false);

		public static readonly ParseResult XProxyAccepted = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.XProxyAccepted, false);

		public static readonly ParseResult ProxyHopCountExceeded = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.ProxyHopCountExceeded, false);

		public static readonly ParseResult InvalidSenderAddress = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.InvalidSenderAddress, false);

		public static readonly ParseResult UnableToAcceptAnonymousSession = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.UnableToAcceptAnonymousSession, false);

		public static readonly ParseResult SubmitDenied = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.SubmitDenied, false);

		public static readonly ParseResult InsufficientResource = new ParseResult(ParsingStatus.Error, SmtpResponse.InsufficientResource, false);

		public static readonly ParseResult DomainSecureDisabled = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.DomainSecureDisabled, false);

		public static readonly ParseResult NotAuthenticated = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.NotAuthenticated, false);

		public static readonly ParseResult CertificateValidationFailure = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.CertificateValidationFailure, false);

		public static readonly ParseResult XMessageEPropNotFoundInMailCommand = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.XMessageEPropNotFoundInMailCommand, false);

		public static readonly ParseResult LongSenderAddress = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.LongSenderAddress, false);

		public static readonly ParseResult Utf8SenderAddress = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.Utf8SenderAddress, false);

		public static readonly ParseResult SmtpUtf8ArgumentNotProvided = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.SmtpUtf8ArgumentNotProvided, false);

		public static readonly ParseResult OrgQueueQuotaExceeded = new ParseResult(ParsingStatus.Error, SmtpResponse.OrgQueueQuotaExceeded, false);

		public static readonly ParseResult ServiceInactiveDisconnect = new ParseResult(ParsingStatus.Error, SmtpResponse.ServiceInactive, true);

		public static readonly ParseResult CertificateValidationFailureDisconnect = new ParseResult(ParsingStatus.Error, SmtpResponse.CertificateValidationFailure, true);

		public static readonly ParseResult TlsCipherKeySizeTooShortDisconnect = new ParseResult(ParsingStatus.Error, SmtpResponse.AuthTempFailureTLSCipherTooWeak, true);

		public static readonly ParseResult AuthTempFailureDisconnect = new ParseResult(ParsingStatus.Error, SmtpResponse.AuthTempFailure, true);

		public static readonly ParseResult DataTransactionFailed = new ParseResult(ParsingStatus.Error, SmtpResponse.DataTransactionFailed, false);

		public static readonly ParseResult InvalidLastChunk = new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.InvalidArguments, true);

		public static readonly ParseResult IgnorableRcpt2InvalidArguments = new ParseResult(ParsingStatus.IgnorableProtocolError, SmtpResponse.Rcpt2ToOkButInvalidArguments, false);

		public static readonly ParseResult IgnorableRcpt2InvalidAddress = new ParseResult(ParsingStatus.IgnorableProtocolError, SmtpResponse.Rcpt2ToOkButInvalidAddress, false);

		public static readonly ParseResult IgnorableValidRcpt2ButDifferentFromRcptAddress = new ParseResult(ParsingStatus.IgnorableProtocolError, SmtpResponse.Rcpt2ToOkButRcpt2AddressDifferentFromRcptAddress, false);
	}
}
