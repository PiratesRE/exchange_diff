using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class AnonymousTlsSmtpInCommand : TlsSmtpInCommandBase
	{
		public AnonymousTlsSmtpInCommand(SmtpInSessionState sessionState, AwaitCompletedDelegate awaitCompletedDelegate) : base(sessionState, awaitCompletedDelegate)
		{
		}

		protected override ParseAndProcessResult<SmtpInStateMachineEvents> CommandCompletedResult
		{
			get
			{
				return AnonymousTlsSmtpInCommand.CommandComplete;
			}
		}

		protected override void OnClientCertificateReceived(IX509Certificate2 remoteCertificate)
		{
			this.sessionState.UpdateIdentityBasedOnClientTlsCertificate(remoteCertificate);
		}

		protected override SecureState Command
		{
			get
			{
				return SecureState.AnonymousTls;
			}
		}

		protected override IX509Certificate2 LocalCertificate
		{
			get
			{
				return this.sessionState.InternalTransportCertificate;
			}
		}

		protected override bool ShouldRequestClientCertificate
		{
			get
			{
				return true;
			}
		}

		public static readonly ParseAndProcessResult<SmtpInStateMachineEvents> CommandComplete = new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, SmtpResponse.Empty, SmtpInStateMachineEvents.XAnonymousTlsProcessed, false);
	}
}
