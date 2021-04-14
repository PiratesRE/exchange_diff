using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class ReceiveMessageEventSourceImpl : ReceiveMessageEventSource
	{
		private ReceiveMessageEventSourceImpl(SmtpSession smtpSession, MailItem mailItem) : base(smtpSession)
		{
			this.mailItem = mailItem;
		}

		public static ReceiveMessageEventSource Create(SmtpSession smtpSession, MailItem mailItem)
		{
			ArgumentValidator.ThrowIfNull("mailItem", mailItem);
			return new ReceiveMessageEventSourceImpl(smtpSession, mailItem);
		}

		public override void RejectMessage(SmtpResponse response)
		{
			this.RejectMessage(response, null);
		}

		public override void RejectMessage(SmtpResponse response, string context)
		{
			if (response.Equals(SmtpResponse.Empty))
			{
				throw new ArgumentException("Argument cannot be response.Empty", "response");
			}
			base.SmtpSession.RejectMessage(response, context);
		}

		public override void DiscardMessage(SmtpResponse response, string context)
		{
			base.SmtpSession.DiscardMessage(response, context);
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

		public override void Quarantine(IEnumerable<EnvelopeRecipient> recipients, string quarantineReason)
		{
			HeaderList headers = this.mailItem.Message.MimeDocument.RootPart.Headers;
			if (headers.FindFirst("X-MS-Exchange-Organization-Quarantine") != null)
			{
				return;
			}
			if (recipients == null)
			{
				recipients = this.mailItem.Recipients;
			}
			SmtpResponse smtpResponse = new SmtpResponse("550", "5.2.1", new string[]
			{
				quarantineReason
			});
			IList<EnvelopeRecipient> list = (recipients as IList<EnvelopeRecipient>) ?? new List<EnvelopeRecipient>(recipients);
			for (int i = list.Count - 1; i >= 0; i--)
			{
				EnvelopeRecipient envelopeRecipient = list[i];
				MailRecipient mailRecipient = ((MailRecipientWrapper)envelopeRecipient).MailRecipient;
				mailRecipient.Ack(AckStatus.Quarantine, smtpResponse);
			}
		}

		private readonly MailItem mailItem;
	}
}
