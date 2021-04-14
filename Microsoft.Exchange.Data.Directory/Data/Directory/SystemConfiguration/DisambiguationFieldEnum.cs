using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum DisambiguationFieldEnum
	{
		[LocDescription(DirectoryStrings.IDs.Title)]
		Title,
		[LocDescription(DirectoryStrings.IDs.Department)]
		Department,
		[LocDescription(DirectoryStrings.IDs.Location)]
		Location,
		[LocDescription(DirectoryStrings.IDs.None)]
		None,
		[LocDescription(DirectoryStrings.IDs.PromptForAlias)]
		PromptForAlias
	}
}
