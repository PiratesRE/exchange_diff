using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum AutoAttendantFlagBits
	{
		[LocDescription(DirectoryStrings.IDs.None)]
		None = 0,
		[LocDescription(DirectoryStrings.IDs.Enabled)]
		Enabled = 1,
		[LocDescription(DirectoryStrings.IDs.NameLookupEnabled)]
		NameLookupEnabled = 2,
		[LocDescription(DirectoryStrings.IDs.InfoAnnouncementEnabled)]
		InfoAnnouncementEnabled = 4,
		[LocDescription(DirectoryStrings.IDs.ForwardCallsToDefaultMailbox)]
		ForwardCallsToDefaultMailbox = 8,
		[LocDescription(DirectoryStrings.IDs.StarOutToDialPlanEnabled)]
		StarOutToDialPlanEnabled = 16
	}
}
