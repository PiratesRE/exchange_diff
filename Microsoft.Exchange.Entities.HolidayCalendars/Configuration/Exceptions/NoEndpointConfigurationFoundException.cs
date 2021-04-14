using System;

namespace Microsoft.Exchange.Entities.HolidayCalendars.Configuration.Exceptions
{
	public class NoEndpointConfigurationFoundException : HolidayCalendarException
	{
		public NoEndpointConfigurationFoundException(string message, params object[] args) : base(message, args)
		{
		}
	}
}
