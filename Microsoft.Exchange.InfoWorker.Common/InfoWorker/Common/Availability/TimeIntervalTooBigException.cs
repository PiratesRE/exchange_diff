using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class TimeIntervalTooBigException : AvailabilityInvalidParameterException
	{
		public TimeIntervalTooBigException(string propertyName, int allowedTimeSpanInDays, int actualTimeSpanInDays) : base(ErrorConstants.TimeIntervalTooBig, Strings.descTimeIntervalTooBig(propertyName, allowedTimeSpanInDays.ToString(), actualTimeSpanInDays.ToString()))
		{
		}
	}
}
