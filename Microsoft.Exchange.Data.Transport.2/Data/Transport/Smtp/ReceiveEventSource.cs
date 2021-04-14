using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public abstract class ReceiveEventSource
	{
		internal ReceiveEventSource(SmtpSession smtpSession)
		{
			this.smtpSession = smtpSession;
		}

		internal SmtpSession SmtpSession
		{
			get
			{
				return this.smtpSession;
			}
		}

		public abstract void Disconnect();

		public abstract CertificateValidationStatus ValidateCertificate();

		public abstract CertificateValidationStatus ValidateCertificate(string domain, out string matchedCertDomain);

		private readonly SmtpSession smtpSession;
	}
}
