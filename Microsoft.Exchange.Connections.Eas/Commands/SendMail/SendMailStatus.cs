using System;

namespace Microsoft.Exchange.Connections.Eas.Commands.SendMail
{
	[Flags]
	public enum SendMailStatus
	{
		Success = 0,
		InvalidMIME = 4203,
		SendQuotaExceeded = 4211,
		MessagePreviouslySent = 4214,
		MessageHasNoRecipient = 4215,
		MailSubmissionFailed = 376
	}
}
