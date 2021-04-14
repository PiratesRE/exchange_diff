using System;

namespace Microsoft.Exchange.Monitoring
{
	public enum TopologyServiceError
	{
		None,
		OverThreshold,
		CommunicationException,
		InvalidOperationException,
		OtherException = 10
	}
}
