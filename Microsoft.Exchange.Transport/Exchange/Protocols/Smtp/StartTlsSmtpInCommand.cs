using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class StartTlsSmtpInCommand : TlsSmtpInCommandBase
	{
		public StartTlsSmtpInCommand(SmtpInSessionState sessionState, AwaitCompletedDelegate awaitCompletedDelegate) : base(sessionState, awaitCompletedDelegate)
		{
			this.tlsReceiveDomainSecureList = this.sessionState.Configuration.TransportConfiguration.TlsReceiveDomainSecureList;
		}

		protected override SecureState Command
		{
			get
			{
				return SecureState.StartTls;
			}
		}

		protected override IX509Certificate2 LocalCertificate
		{
			get
			{
				return this.sessionState.AdvertisedTlsCertificate;
			}
		}

		protected override bool ShouldRequestClientCertificate
		{
			get
			{
				return this.sessionState.RequestClientTlsCertificate || (this.sessionState.ReceiveConnector.DomainSecureEnabled && this.tlsReceiveDomainSecureList.Any<SmtpDomain>()) || this.sessionState.ReceiveConnectorStub.ContainsTlsDomainCapabilities;
			}
		}

		protected override ParseAndProcessResult<SmtpInStateMachineEvents> CommandCompletedResult
		{
			get
			{
				return StartTlsSmtpInCommand.CommandComplete;
			}
		}

		public static readonly ParseAndProcessResult<SmtpInStateMachineEvents> CommandComplete = new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, SmtpResponse.Empty, SmtpInStateMachineEvents.StartTlsProcessed, false);

		private readonly IEnumerable<SmtpDomain> tlsReceiveDomainSecureList;
	}
}
