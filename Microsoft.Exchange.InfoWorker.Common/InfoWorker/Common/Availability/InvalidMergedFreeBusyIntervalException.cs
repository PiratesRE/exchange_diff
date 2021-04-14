using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class InvalidMergedFreeBusyIntervalException : AvailabilityInvalidParameterException
	{
		public InvalidMergedFreeBusyIntervalException(int minimumValue, int maximumValue) : base(ErrorConstants.InvalidMergedFreeBusyInterval, Strings.descInvalidMergedFreeBusyInterval(minimumValue.ToString(), maximumValue.ToString()))
		{
		}
	}
}
