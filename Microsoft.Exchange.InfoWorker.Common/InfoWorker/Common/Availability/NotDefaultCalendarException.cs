using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class NotDefaultCalendarException : AvailabilityException
	{
		public NotDefaultCalendarException() : base(ErrorConstants.NotDefaultCalendar, Strings.descNotDefaultCalendar)
		{
		}
	}
}
