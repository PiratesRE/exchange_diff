using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class EhloSmtpInCommand : HeloSmtpInCommandBase
	{
		public EhloSmtpInCommand(SmtpInSessionState sessionState, AwaitCompletedDelegate awaitCompletedDelegate) : base(sessionState, awaitCompletedDelegate)
		{
			this.role = sessionState.Configuration.TransportConfiguration.ProcessTransportRole;
		}

		protected override void OnParseComplete(out string agentEventTopic, out ReceiveCommandEventArgs agentEventArgs)
		{
			EhloCommandEventArgs ehloCommandEventArgs = new EhloCommandEventArgs(this.sessionState);
			if (!string.IsNullOrEmpty(this.parseOutput.HeloDomain))
			{
				this.sessionState.HelloDomain = this.parseOutput.HeloDomain;
				ehloCommandEventArgs.Domain = this.parseOutput.HeloDomain;
			}
			agentEventTopic = "OnEhloCommand";
			agentEventArgs = ehloCommandEventArgs;
		}

		protected override Task<ParseAndProcessResult<SmtpInStateMachineEvents>> ProcessAsyncInternal(CancellationToken cancellationToken)
		{
			this.sessionState.AdvertisedEhloOptions.AddAuthenticationMechanism("AUTH LOGIN", this.ShouldAuthLoginBeAdvertised);
			this.sessionState.AdvertisedEhloOptions.AddAuthenticationMechanism("AUTH GSSAPI", this.ShouldAuthGssApiBeAdvertised);
			this.sessionState.AdvertisedEhloOptions.AddAuthenticationMechanism("AUTH NTLM", this.ShouldAuthNtlmBeAdvertised);
			this.sessionState.AdvertisedEhloOptions.AddAuthenticationMechanism("X-EXPS GSSAPI", this.ShouldExpsGssApiBeAdvertised);
			this.sessionState.AdvertisedEhloOptions.AddAuthenticationMechanism("X-EXPS EXCHANGEAUTH", this.ShouldExpsExchangeAuthBeAdvertised);
			this.sessionState.AdvertisedEhloOptions.AddAuthenticationMechanism("X-EXPS NTLM", this.ShouldExpsNtlmBeAdvertised);
			this.sessionState.AdvertisedEhloOptions.SetFlags(EhloOptionsFlags.StartTls, this.ShouldStartTlsBeAdvertised);
			this.sessionState.AdvertisedEhloOptions.SetFlags(EhloOptionsFlags.AnonymousTls, this.ShouldAnonymousTlsBeAdvertised);
			this.sessionState.AdvertisedEhloOptions.SetFlags(EhloOptionsFlags.Xoorg, this.ShouldXoorgBeAdvertised);
			this.sessionState.AdvertisedEhloOptions.SetFlags(EhloOptionsFlags.Xproxy, this.ShouldXproxyBeAdvertised);
			this.sessionState.AdvertisedEhloOptions.SetFlags(EhloOptionsFlags.XproxyFrom, this.ShouldXproxyFromBeAdvertised);
			this.sessionState.AdvertisedEhloOptions.SetFlags(EhloOptionsFlags.XproxyTo, this.ShouldXproxyToBeAdvertised);
			this.sessionState.AdvertisedEhloOptions.SetFlags(EhloOptionsFlags.XrsetProxyTo, this.ShouldXrsetProxyToBeAdvertised);
			this.sessionState.AdvertisedEhloOptions.SetFlags(EhloOptionsFlags.XSessionMdbGuid, this.ShouldXSessionMdbGuidBeAdvertised);
			this.sessionState.AdvertisedEhloOptions.SetFlags(EhloOptionsFlags.XAttr, this.ShouldXAttrBeAdvertised);
			this.sessionState.AdvertisedEhloOptions.SetFlags(EhloOptionsFlags.XSysProbe, this.ShouldXSysProbeBeAdvertised);
			this.sessionState.AdvertisedEhloOptions.SetFlags(EhloOptionsFlags.XExProps, this.ShouldExtendedPropertiesBeAdvertised);
			this.sessionState.AdvertisedEhloOptions.SetFlags(EhloOptionsFlags.XAdrc, this.ShouldADRecipientCacheBeAdvertised);
			this.sessionState.AdvertisedEhloOptions.SetFlags(EhloOptionsFlags.XFastIndex, this.ShouldFastIndexBeAdvertised);
			this.sessionState.AdvertisedEhloOptions.SetFlags(EhloOptionsFlags.XSessionType, this.ShouldXSessionTypeBeAdvertised);
			return Task.FromResult<ParseAndProcessResult<SmtpInStateMachineEvents>>(new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, this.sessionState.AdvertisedEhloOptions.CreateSmtpResponse(this.sessionState.MessageContextBlob.AdrcSmtpMessageContextBlob, this.sessionState.MessageContextBlob.ExtendedPropertiesSmtpMessageContextBlob, this.sessionState.MessageContextBlob.FastIndexSmtpMessageContextBlob), SmtpInStateMachineEvents.EhloProcessed, false));
		}

		protected override HeloOrEhlo Command
		{
			get
			{
				return HeloOrEhlo.Ehlo;
			}
		}

		protected virtual bool ShouldAuthLoginBeAdvertised
		{
			get
			{
				return SmtpInSessionUtils.ShouldAuthLoginBeAdvertised(this.sessionState.ReceiveConnector.AuthMechanism, this.sessionState.SecureState);
			}
		}

		protected virtual bool ShouldAuthGssApiBeAdvertised
		{
			get
			{
				return SmtpInSessionUtils.ShouldAuthGssApiBeAdvertised(this.sessionState.IsIntegratedAuthSupported, this.sessionState.ReceiveConnector.EnableAuthGSSAPI);
			}
		}

		protected virtual bool ShouldAuthNtlmBeAdvertised
		{
			get
			{
				return SmtpInSessionUtils.ShouldAuthNtlmBeAdvertised(this.sessionState.IsIntegratedAuthSupported);
			}
		}

		protected virtual bool ShouldExpsGssApiBeAdvertised
		{
			get
			{
				return SmtpInSessionUtils.ShouldExpsGssApiBeAdvertised(this.sessionState.ReceiveConnector.AuthMechanism, this.role);
			}
		}

		protected virtual bool ShouldExpsExchangeAuthBeAdvertised
		{
			get
			{
				return SmtpInSessionUtils.ShouldExpsExchangeAuthBeAdvertised(this.sessionState.ReceiveConnector.AuthMechanism, this.sessionState.SecureState, this.role);
			}
		}

		protected virtual bool ShouldExpsNtlmBeAdvertised
		{
			get
			{
				return SmtpInSessionUtils.ShouldExpsNtlmBeAdvertised(this.sessionState.ReceiveConnector.AuthMechanism);
			}
		}

		protected virtual bool ShouldStartTlsBeAdvertised
		{
			get
			{
				return SmtpInSessionUtils.ShouldStartTlsBeAdvertised(this.sessionState.ReceiveConnector.AuthMechanism, this.sessionState.SecureState, this.sessionState.IsStartTlsSupported);
			}
		}

		protected virtual bool ShouldAnonymousTlsBeAdvertised
		{
			get
			{
				return SmtpInSessionUtils.ShouldAnonymousTlsBeAdvertised(this.sessionState.ReceiveConnector.AuthMechanism, this.sessionState.SecureState, this.sessionState.IsAnonymousTlsSupported);
			}
		}

		protected virtual bool ShouldXoorgBeAdvertised
		{
			get
			{
				return SmtpInSessionUtils.ShouldXoorgBeAdvertised(this.sessionState.Capabilities);
			}
		}

		protected virtual bool ShouldXproxyBeAdvertised
		{
			get
			{
				return SmtpInSessionUtils.ShouldXproxyBeAdvertised(this.role, this.sessionState.Capabilities, this.sessionState.SecureState);
			}
		}

		protected virtual bool ShouldXproxyFromBeAdvertised
		{
			get
			{
				return SmtpInSessionUtils.ShouldXproxyFromBeAdvertised(this.role, this.sessionState.Capabilities, this.sessionState.SecureState);
			}
		}

		protected virtual bool ShouldXproxyToBeAdvertised
		{
			get
			{
				return SmtpInSessionUtils.ShouldXproxyToBeAdvertised(this.role, this.sessionState.Capabilities, this.sessionState.SecureState);
			}
		}

		protected virtual bool ShouldXrsetProxyToBeAdvertised
		{
			get
			{
				return SmtpInSessionUtils.ShouldXrsetProxyToBeAdvertised(this.role, this.sessionState.Capabilities, this.sessionState.SecureState);
			}
		}

		protected virtual bool ShouldXSessionMdbGuidBeAdvertised
		{
			get
			{
				return SmtpInSessionUtils.ShouldXSessionMdbGuidBeAdvertised(this.role, this.sessionState.SecureState);
			}
		}

		protected virtual bool ShouldXAttrBeAdvertised
		{
			get
			{
				return SmtpInSessionUtils.ShouldXAttrBeAdvertised(this.sessionState.Capabilities, this.sessionState.SecureState);
			}
		}

		protected virtual bool ShouldXSysProbeBeAdvertised
		{
			get
			{
				return SmtpInSessionUtils.ShouldXSysProbeBeAdvertised(this.sessionState.Capabilities, this.sessionState.SecureState);
			}
		}

		protected virtual bool ShouldExtendedPropertiesBeAdvertised
		{
			get
			{
				return SmtpInSessionUtils.ShouldExtendedPropertiesBeAdvertised(this.role, this.sessionState.SecureState, this.sessionState.Configuration.TransportConfiguration.AdvertiseExtendedProperties);
			}
		}

		protected virtual bool ShouldADRecipientCacheBeAdvertised
		{
			get
			{
				return SmtpInSessionUtils.ShouldADRecipientCacheBeAdvertised(this.role, this.sessionState.SecureState, this.sessionState.Configuration.TransportConfiguration.AdvertiseADRecipientCache);
			}
		}

		protected virtual bool ShouldFastIndexBeAdvertised
		{
			get
			{
				return SmtpInSessionUtils.ShouldFastIndexBeAdvertised(this.role, this.sessionState.SecureState, this.sessionState.Configuration.TransportConfiguration.AdvertiseFastIndex);
			}
		}

		protected virtual bool ShouldXSessionTypeBeAdvertised
		{
			get
			{
				return SmtpInSessionUtils.ShouldXSessionTypeBeAdvertised(this.role, this.sessionState.SecureState);
			}
		}

		private readonly ProcessTransportRole role;
	}
}
