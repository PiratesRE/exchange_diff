using System;

namespace Microsoft.Exchange.Data
{
	[Flags]
	public enum SharingPolicyAction
	{
		[LocDescription(DataStrings.IDs.CalendarSharingFreeBusySimple)]
		CalendarSharingFreeBusySimple = 1,
		[LocDescription(DataStrings.IDs.CalendarSharingFreeBusyDetail)]
		CalendarSharingFreeBusyDetail = 2,
		[LocDescription(DataStrings.IDs.CalendarSharingFreeBusyReviewer)]
		CalendarSharingFreeBusyReviewer = 4,
		[LocDescription(DataStrings.IDs.ContactsSharing)]
		ContactsSharing = 8,
		[LocDescription(DataStrings.IDs.MeetingLimitedDetails)]
		MeetingLimitedDetails = 16,
		[LocDescription(DataStrings.IDs.MeetingFullDetails)]
		MeetingFullDetails = 32,
		[LocDescription(DataStrings.IDs.MeetingFullDetailsWithAttendees)]
		MeetingFullDetailsWithAttendees = 64
	}
}
