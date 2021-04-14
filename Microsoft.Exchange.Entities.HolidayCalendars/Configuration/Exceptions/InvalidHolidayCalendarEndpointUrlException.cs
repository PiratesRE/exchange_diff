using System;

namespace Microsoft.Exchange.Entities.HolidayCalendars.Configuration.Exceptions
{
	public class InvalidHolidayCalendarEndpointUrlException : HolidayCalendarException
	{
		public InvalidHolidayCalendarEndpointUrlException(string message, params object[] args) : base(message, args)
		{
		}
	}
}
