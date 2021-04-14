using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class MailboxLogonFailedException : AvailabilityException
	{
		public MailboxLogonFailedException() : base(ErrorConstants.MailboxLogonFailed, Strings.descMailboxLogonFailed)
		{
		}

		public MailboxLogonFailedException(Exception innerException) : base(ErrorConstants.MailboxLogonFailed, Strings.descMailboxLogonFailed, innerException)
		{
		}
	}
}
