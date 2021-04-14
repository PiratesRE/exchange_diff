using System;

namespace Microsoft.Exchange.Data
{
	public enum MailRecipientType
	{
		[LocDescription(DataStrings.IDs.MailRecipientTypeUnknown)]
		Unknown,
		[LocDescription(DataStrings.IDs.MailRecipientTypeDistributionGroup)]
		DistributionGroup,
		[LocDescription(DataStrings.IDs.MailRecipientTypeExternal)]
		External,
		[LocDescription(DataStrings.IDs.MailRecipientTypeMailbox)]
		Mailbox
	}
}
