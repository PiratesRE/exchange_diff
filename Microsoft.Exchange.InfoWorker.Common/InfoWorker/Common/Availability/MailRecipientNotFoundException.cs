using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class MailRecipientNotFoundException : AvailabilityException
	{
		public MailRecipientNotFoundException(LocalizedString message, uint locationIdentifier) : base(ErrorConstants.MailRecipientNotFound, message, locationIdentifier)
		{
		}

		public MailRecipientNotFoundException(Exception innerException, uint locationIdentifier) : base(ErrorConstants.MailRecipientNotFound, Strings.descMailRecipientNotFound, innerException, locationIdentifier)
		{
		}
	}
}
