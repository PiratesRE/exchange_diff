using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class ServiceDiscoveryFailedException : AvailabilityException
	{
		public ServiceDiscoveryFailedException(LocalizedString message, Exception innerException) : base(ErrorConstants.ServiceDiscoveryFailed, message, innerException)
		{
		}
	}
}
