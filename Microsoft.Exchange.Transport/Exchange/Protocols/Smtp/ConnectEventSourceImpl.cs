using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class ConnectEventSourceImpl : ConnectEventSource
	{
		private ConnectEventSourceImpl(SmtpSession smtpSession) : base(smtpSession)
		{
		}

		public static ConnectEventSource Create(SmtpSession smtpSession)
		{
			return new ConnectEventSourceImpl(smtpSession);
		}

		public override void RejectConnection(SmtpResponse response)
		{
			if (response.Equals(SmtpResponse.Empty))
			{
				throw new ArgumentException("Argument cannot be response.Empty", "response");
			}
			base.SmtpSession.SmtpResponse = response;
			this.Disconnect();
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
