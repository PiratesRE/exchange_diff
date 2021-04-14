using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class NoFreeBusyAccessException : AvailabilityException
	{
		public NoFreeBusyAccessException(uint locationIdentifier) : base(ErrorConstants.NoFreeBusyAccess, Strings.descNoFreeBusyAccess, locationIdentifier)
		{
		}
	}
}
