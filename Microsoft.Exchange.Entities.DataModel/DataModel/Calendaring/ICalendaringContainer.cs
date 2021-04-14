using System;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public interface ICalendaringContainer
	{
		IMailboxCalendars Calendars { get; }

		ICalendarGroups CalendarGroups { get; }

		IMeetingRequestMessages MeetingRequestMessages { get; }
	}
}
