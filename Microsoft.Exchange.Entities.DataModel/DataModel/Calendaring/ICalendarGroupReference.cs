using System;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public interface ICalendarGroupReference : IEntityReference<CalendarGroup>
	{
		ICalendars Calendars { get; }
	}
}
