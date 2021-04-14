using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CalendarsInCalendarGroup : Calendars
	{
		public CalendarsInCalendarGroup(CalendarGroupReference calendarGroup) : base(calendarGroup, calendarGroup, null)
		{
			this.CalendarGroup = calendarGroup;
		}

		public CalendarGroupReference CalendarGroup { get; private set; }
	}
}
