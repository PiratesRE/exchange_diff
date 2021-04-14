using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class ProxyQueryFailureException : AvailabilityException
	{
		public ProxyQueryFailureException(string serverName, LocalizedString message, ErrorConstants errorCode, ResponseMessage responseMessage, string responseSource) : base(serverName, errorCode, message)
		{
			this.ResponseMessage = responseMessage;
			this.ResponseSource = responseSource;
		}

		public ResponseMessage ResponseMessage { get; private set; }

		public string ResponseSource { get; private set; }
	}
}
