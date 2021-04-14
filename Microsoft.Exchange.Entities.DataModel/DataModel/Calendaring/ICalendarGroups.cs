using System;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public interface ICalendarGroups : IEntitySet<CalendarGroup>
	{
		ICalendarGroupReference MyCalendars { get; }

		ICalendarGroupReference OtherCalendars { get; }

		ICalendarGroupReference this[string calendarGroupId]
		{
			get;
		}

		CalendarGroup Create(string name);
	}
}
