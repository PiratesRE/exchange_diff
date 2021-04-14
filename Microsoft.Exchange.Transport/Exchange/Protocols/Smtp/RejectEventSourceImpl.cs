using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class RejectEventSourceImpl : RejectEventSource
	{
		private RejectEventSourceImpl(SmtpSession smtpSession) : base(smtpSession)
		{
		}

		public static RejectEventSource Create(SmtpSession smtpSession)
		{
			return new RejectEventSourceImpl(smtpSession);
		}

		public override void Disconnect()
		{
			base.SmtpSession.Disconnect();
		}

		public override CertificateValidationStatus ValidateCertificate()
		{
			return base.SmtpSession.ValidateCertificate();
		}

		public override CertificateValidationStatus ValidateCertificate(string domain, out string matchedCertDomain)
		{
			return base.SmtpSession.ValidateCertificate(domain, out matchedCertDomain);
		}
	}
}
