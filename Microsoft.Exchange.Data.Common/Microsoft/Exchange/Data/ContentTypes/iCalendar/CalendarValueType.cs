using System;

namespace Microsoft.Exchange.Data.ContentTypes.iCalendar
{
	[Flags]
	public enum CalendarValueType
	{
		Unknown = 1,
		Binary = 2,
		Boolean = 4,
		CalAddress = 8,
		Date = 16,
		DateTime = 32,
		Duration = 64,
		Float = 128,
		Integer = 256,
		Period = 512,
		Recurrence = 1024,
		Text = 2048,
		Time = 4096,
		Uri = 8192,
		UtcOffset = 16384
	}
}
