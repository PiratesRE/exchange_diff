using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Shared.Smtp
{
	internal interface ISmtpMailItemSenderNotifications
	{
		void AckConnection(AckStatus status, SmtpResponse smtpResponse);

		void AckMessage(AckStatus status, SmtpResponse smtpResponse);

		void AckRecipient(AckStatus status, SmtpResponse smtpResponse, MailRecipient recipient);
	}
}
