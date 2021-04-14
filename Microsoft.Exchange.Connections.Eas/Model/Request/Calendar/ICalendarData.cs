using System;
using System.Collections.Generic;
using Microsoft.Exchange.Connections.Eas.Model.Common.Email;
using Microsoft.Exchange.Connections.Eas.Model.Request.AirSyncBase;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.Calendar
{
	public interface ICalendarData
	{
		Body Body { get; set; }

		byte? AllDayEvent { get; set; }

		byte? BusyStatus { get; set; }

		string DtStamp { get; set; }

		string EndTime { get; set; }

		string Location { get; set; }

		uint? Reminder { get; set; }

		byte? Sensitivity { get; set; }

		string CalendarSubject { get; set; }

		string StartTime { get; set; }

		List<Attendee> Attendees { get; set; }

		List<Category> CalendarCategories { get; set; }
	}
}
