using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class TimeoutExpiredException : AvailabilityException
	{
		public TimeoutExpiredException(string requestState) : base(ErrorConstants.TimeoutExpired, Strings.descTimeoutExpired(requestState))
		{
		}
	}
}
