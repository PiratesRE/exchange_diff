using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class NoCalendarException : AvailabilityException
	{
		public NoCalendarException() : base(ErrorConstants.NoCalendar, Strings.descNoCalendar)
		{
		}
	}
}
