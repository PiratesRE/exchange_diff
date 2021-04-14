using System;

namespace Microsoft.Exchange.Data.ContentTypes.iCalendar
{
	[Flags]
	public enum ComponentId
	{
		None = 0,
		Unknown = 1,
		VCalendar = 2,
		VEvent = 4,
		VTodo = 8,
		VJournal = 16,
		VFreeBusy = 32,
		VTimeZone = 64,
		VAlarm = 128,
		Standard = 256,
		Daylight = 512
	}
}
