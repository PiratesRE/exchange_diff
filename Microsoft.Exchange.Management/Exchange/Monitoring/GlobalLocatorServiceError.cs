using System;

namespace Microsoft.Exchange.Monitoring
{
	public enum GlobalLocatorServiceError
	{
		None,
		OverThreshold,
		CommunicationException,
		InvalidOperationException,
		OtherException = 10
	}
}
