using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class IndividualMailboxLimitReachedException : AvailabilityException
	{
		public IndividualMailboxLimitReachedException(LocalizedString message, uint locationIdentifier) : base(ErrorConstants.IndividualMailboxLimitReached, message, locationIdentifier)
		{
		}
	}
}
