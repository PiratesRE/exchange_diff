using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging.MessageTracking;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class ReceiveCommandEventSourceImpl : ReceiveCommandEventSource
	{
		private ReceiveCommandEventSourceImpl(SmtpSession smtpSession) : base(smtpSession)
		{
		}

		public static ReceiveCommandEventSource Create(SmtpSession smtpSession)
		{
			return new ReceiveCommandEventSourceImpl(smtpSession);
		}

		public override void RejectCommand(SmtpResponse response)
		{
			this.RejectCommand(response, null);
		}

		public override void RejectCommand(SmtpResponse response, string trackingContext)
		{
			if (response.Equals(SmtpResponse.Empty))
			{
				throw new ArgumentException("Argument cannot be response.Empty", "response");
			}
			base.SmtpSession.SmtpResponse = response;
			base.SmtpSession.ExecutionControl.HaltExecution();
			StringBuilder stringBuilder = new StringBuilder(base.SmtpSession.ExecutionControl.ExecutingAgentName);
			if (!string.IsNullOrEmpty(trackingContext))
			{
				stringBuilder.AppendFormat("; {0}", trackingContext);
			}
			AckDetails ackDetails = new AckDetails(base.SmtpSession.LocalEndPoint, null, base.SmtpSession.SessionId.ToString("X16", NumberFormatInfo.InvariantInfo), null, base.SmtpSession.RemoteEndPoint.Address);
			MessageTrackingLog.TrackRejectCommand(MessageTrackingSource.SMTP, stringBuilder.ToString(), ackDetails, response);
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
