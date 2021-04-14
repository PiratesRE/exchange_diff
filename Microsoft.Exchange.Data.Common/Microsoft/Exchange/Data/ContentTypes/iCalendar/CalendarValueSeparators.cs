using System;

namespace Microsoft.Exchange.Data.ContentTypes.iCalendar
{
	[Flags]
	internal enum CalendarValueSeparators
	{
		None = 0,
		Comma = 1,
		Semicolon = 2
	}
}
