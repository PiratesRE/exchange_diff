using System;

namespace Microsoft.Exchange.Monitoring
{
	public enum LiveIdAuthenticationError
	{
		None,
		OverThreshold,
		CommunicationException,
		InvalidOperationException,
		LoginFailure,
		OtherException = 9
	}
}
