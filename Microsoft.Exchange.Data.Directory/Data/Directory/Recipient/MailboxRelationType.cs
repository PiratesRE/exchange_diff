using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	public enum MailboxRelationType
	{
		[LocDescription(DirectoryStrings.IDs.NoneMailboxRelationType)]
		None,
		[LocDescription(DirectoryStrings.IDs.PrimaryMailboxRelationType)]
		Primary,
		[LocDescription(DirectoryStrings.IDs.SecondaryMailboxRelationType)]
		Secondary
	}
}
