using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.EntitySets;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MailboxCalendars : Calendars, IMailboxCalendars, ICalendars, IEntitySet<Calendar>
	{
		public MailboxCalendars(IStorageEntitySetScope<IMailboxSession> scope, CalendarGroupReference calendarGroupForNewCalendars) : base(scope, calendarGroupForNewCalendars, null)
		{
		}

		public CalendarReference Default
		{
			get
			{
				CalendarReference result;
				if ((result = this.defaultCalendar) == null)
				{
					result = (this.defaultCalendar = new DefaultCalendarReference(this));
				}
				return result;
			}
		}

		ICalendarReference IMailboxCalendars.Default
		{
			get
			{
				return this.Default;
			}
		}

		private CalendarReference defaultCalendar;
	}
}
