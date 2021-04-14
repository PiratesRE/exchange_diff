using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class ProxyWebRequestProcessingException : AvailabilityException
	{
		public ProxyWebRequestProcessingException(LocalizedString message) : base(ErrorConstants.ProxyRequestProcessingFailed, message)
		{
		}

		public ProxyWebRequestProcessingException(LocalizedString message, Exception innerException) : base(ErrorConstants.ProxyRequestProcessingFailed, message, innerException)
		{
		}
	}
}
