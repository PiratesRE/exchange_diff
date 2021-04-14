using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class AutoDiscoverFailedException : AvailabilityException
	{
		public AutoDiscoverFailedException(LocalizedString message) : base(ErrorConstants.AutoDiscoverFailed, message)
		{
		}

		public AutoDiscoverFailedException(LocalizedString message, uint locationIdentifier) : base(ErrorConstants.AutoDiscoverFailed, message, locationIdentifier)
		{
		}

		public AutoDiscoverFailedException(LocalizedString message, Exception innerException) : base(ErrorConstants.AutoDiscoverFailed, message, innerException)
		{
		}

		public AutoDiscoverFailedException(LocalizedString message, Exception innerException, uint locationIdentifier) : base(ErrorConstants.AutoDiscoverFailed, message, innerException, locationIdentifier)
		{
		}
	}
}
