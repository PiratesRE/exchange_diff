using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class MailboxFailoverException : AvailabilityException
	{
		public MailboxFailoverException() : base(ErrorConstants.MailboxFailover, Strings.descMailboxFailover)
		{
		}

		public MailboxFailoverException(Exception innerException) : base(ErrorConstants.MailboxFailover, Strings.descMailboxFailover, innerException)
		{
		}
	}
}
