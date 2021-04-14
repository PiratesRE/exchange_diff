using System;

namespace Microsoft.Exchange.Entities.HolidayCalendars.Configuration.Exceptions
{
	public class InvalidHolidayCalendarUrlException : HolidayCalendarException
	{
		public InvalidHolidayCalendarUrlException(string message, params object[] args) : base(message, args)
		{
		}
	}
}
