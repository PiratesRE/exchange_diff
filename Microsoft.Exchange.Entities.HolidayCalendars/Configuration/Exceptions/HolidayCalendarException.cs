using System;

namespace Microsoft.Exchange.Entities.HolidayCalendars.Configuration.Exceptions
{
	public abstract class HolidayCalendarException : Exception
	{
		protected HolidayCalendarException(string message, params object[] args) : base(string.Format(message, args))
		{
		}

		protected HolidayCalendarException(Exception innerException, string message, params object[] args) : base(string.Format(message, args), innerException)
		{
		}
	}
}
