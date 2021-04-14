using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	public enum RecipientType
	{
		[LocDescription(DirectoryStrings.IDs.InvalidRecipientType)]
		Invalid,
		[LocDescription(DirectoryStrings.IDs.UserRecipientType)]
		User,
		[LocDescription(DirectoryStrings.IDs.MailboxUserRecipientType)]
		UserMailbox,
		[LocDescription(DirectoryStrings.IDs.MailEnabledUserRecipientType)]
		MailUser,
		[LocDescription(DirectoryStrings.IDs.ContactRecipientType)]
		Contact,
		[LocDescription(DirectoryStrings.IDs.MailEnabledContactRecipientType)]
		MailContact,
		[LocDescription(DirectoryStrings.IDs.GroupRecipientType)]
		Group,
		[LocDescription(DirectoryStrings.IDs.MailEnabledUniversalDistributionGroupRecipientType)]
		MailUniversalDistributionGroup,
		[LocDescription(DirectoryStrings.IDs.MailEnabledUniversalSecurityGroupRecipientType)]
		MailUniversalSecurityGroup,
		[LocDescription(DirectoryStrings.IDs.MailEnabledNonUniversalGroupRecipientType)]
		MailNonUniversalGroup,
		[LocDescription(DirectoryStrings.IDs.DynamicDLRecipientType)]
		DynamicDistributionGroup,
		[LocDescription(DirectoryStrings.IDs.PublicFolderRecipientType)]
		PublicFolder,
		[LocDescription(DirectoryStrings.IDs.PublicDatabaseRecipientType)]
		PublicDatabase,
		[LocDescription(DirectoryStrings.IDs.SystemAttendantMailboxRecipientType)]
		SystemAttendantMailbox,
		[LocDescription(DirectoryStrings.IDs.SystemMailboxRecipientType)]
		SystemMailbox,
		[LocDescription(DirectoryStrings.IDs.MicrosoftExchangeRecipientType)]
		MicrosoftExchange,
		[LocDescription(DirectoryStrings.IDs.ComputerRecipientType)]
		Computer
	}
}
