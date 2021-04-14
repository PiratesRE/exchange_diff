using System;

namespace Microsoft.Exchange.Data.ContentTypes.iCalendar
{
	[Flags]
	public enum RecurrenceProperties
	{
		None = 0,
		Frequency = 1,
		UntilDate = 2,
		Count = 4,
		Interval = 8,
		BySecond = 16,
		ByMinute = 32,
		ByHour = 64,
		ByDay = 128,
		ByMonthDay = 256,
		ByYearDay = 512,
		ByWeek = 1024,
		ByMonth = 2048,
		BySetPosition = 4096,
		WeekStart = 8192,
		UntilDateTime = 16384
	}
}
