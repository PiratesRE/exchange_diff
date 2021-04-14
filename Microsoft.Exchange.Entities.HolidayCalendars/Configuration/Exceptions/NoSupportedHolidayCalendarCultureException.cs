using System;

namespace Microsoft.Exchange.Entities.HolidayCalendars.Configuration.Exceptions
{
	public class NoSupportedHolidayCalendarCultureException : HolidayCalendarException
	{
		public NoSupportedHolidayCalendarCultureException(string message, params object[] args) : base(message, args)
		{
		}
	}
}
