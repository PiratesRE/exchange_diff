using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class InvalidFreeBusyViewTypeException : AvailabilityInvalidParameterException
	{
		public InvalidFreeBusyViewTypeException() : base(ErrorConstants.InvalidFreeBusyViewType, Strings.descInvalidFreeBusyViewType)
		{
		}
	}
}
