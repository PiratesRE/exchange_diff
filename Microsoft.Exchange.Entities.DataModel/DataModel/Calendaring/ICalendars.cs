using System;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public interface ICalendars : IEntitySet<Calendar>
	{
		ICalendarReference this[string calendarId]
		{
			get;
		}

		Calendar Create(string name);
	}
}
