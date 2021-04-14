using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class ProxyNoResultException : AvailabilityException
	{
		public ProxyNoResultException(LocalizedString message, uint locationIdentifier) : base(ErrorConstants.ProxyRequestProcessingFailed, message, locationIdentifier)
		{
		}
	}
}
