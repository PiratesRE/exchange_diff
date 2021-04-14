using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class InvalidTimeIntervalException : AvailabilityInvalidParameterException
	{
		public InvalidTimeIntervalException(string propertyName) : base(ErrorConstants.InvalidTimeInterval, Strings.descInvalidTimeInterval(propertyName))
		{
		}
	}
}
