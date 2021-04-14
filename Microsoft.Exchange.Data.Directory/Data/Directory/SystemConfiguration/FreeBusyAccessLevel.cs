using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum FreeBusyAccessLevel
	{
		[LocDescription(DirectoryStrings.IDs.CalendarSharingFreeBusyNone)]
		None,
		[LocDescription(DirectoryStrings.IDs.CalendarSharingFreeBusyAvailabilityOnly)]
		AvailabilityOnly,
		[LocDescription(DirectoryStrings.IDs.CalendarSharingFreeBusyLimitedDetails)]
		LimitedDetails
	}
}
