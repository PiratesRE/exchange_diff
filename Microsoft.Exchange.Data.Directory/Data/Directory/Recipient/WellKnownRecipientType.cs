using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Flags]
	public enum WellKnownRecipientType
	{
		[LocDescription(DirectoryStrings.IDs.WellKnownRecipientTypeNone)]
		None = 0,
		[LocDescription(DirectoryStrings.IDs.WellKnownRecipientTypeMailboxUsers)]
		MailboxUsers = 1,
		[LocDescription(DirectoryStrings.IDs.WellKnownRecipientTypeResources)]
		Resources = 2,
		[LocDescription(DirectoryStrings.IDs.WellKnownRecipientTypeMailContacts)]
		MailContacts = 4,
		[LocDescription(DirectoryStrings.IDs.WellKnownRecipientTypeMailGroups)]
		MailGroups = 8,
		[LocDescription(DirectoryStrings.IDs.WellKnownRecipientTypeMailUsers)]
		MailUsers = 16,
		[LocDescription(DirectoryStrings.IDs.WellKnownRecipientTypeAllRecipients)]
		AllRecipients = -1
	}
}
