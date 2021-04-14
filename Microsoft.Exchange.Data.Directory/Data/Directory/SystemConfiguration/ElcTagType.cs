using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	public enum ElcTagType
	{
		None = 0,
		[LocDescription(DirectoryStrings.IDs.SystemTag)]
		SystemTag = 1,
		[LocDescription(DirectoryStrings.IDs.MustDisplayComment)]
		MustDisplayComment = 2,
		[LocDescription(DirectoryStrings.IDs.PrimaryDefault)]
		PrimaryDefault = 4,
		[LocDescription(DirectoryStrings.IDs.AutoGroup)]
		AutoGroup = 8,
		[LocDescription(DirectoryStrings.IDs.ModeratedRecipients)]
		ModeratedRecipients = 16
	}
}
