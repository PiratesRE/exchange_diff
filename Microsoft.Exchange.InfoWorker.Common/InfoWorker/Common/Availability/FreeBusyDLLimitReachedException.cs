using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class FreeBusyDLLimitReachedException : AvailabilityException
	{
		public FreeBusyDLLimitReachedException(int allowedSize) : base(ErrorConstants.FreeBusyDLLimitReached, Strings.descFreeBusyDLLimitReached(allowedSize))
		{
		}
	}
}
