using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal enum MailboxTransportSmtpState
	{
		WaitingForGreeting,
		GreetingReceived,
		WaitingForSecureGreeting,
		SecureGreetingReceived,
		MailTransactionStarted,
		WaitingForMoreRecipientsOrData,
		ReceivingBdatChunks,
		End
	}
}
