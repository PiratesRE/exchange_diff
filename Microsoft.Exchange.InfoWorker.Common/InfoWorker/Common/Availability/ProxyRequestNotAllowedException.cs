using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class ProxyRequestNotAllowedException : AvailabilityInvalidParameterException
	{
		public ProxyRequestNotAllowedException() : base(ErrorConstants.ProxyRequestNotAllowed, Strings.descProxyRequestNotAllowed)
		{
		}

		public ProxyRequestNotAllowedException(LocalizedString message) : base(ErrorConstants.ProxyRequestNotAllowed, message)
		{
		}

		public ProxyRequestNotAllowedException(LocalizedString message, Exception innerException) : base(ErrorConstants.ProxyRequestNotAllowed, message, innerException)
		{
		}
	}
}
