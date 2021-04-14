using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum AutoAttendantDisambiguationFieldEnum
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
		PromptForAlias,
		[LocDescription(DirectoryStrings.IDs.InheritFromDialPlan)]
		InheritFromDialPlan
	}
}
