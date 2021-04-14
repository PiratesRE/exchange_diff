using System;

namespace Microsoft.Exchange.Monitoring
{
	internal enum WacConnectivityErrors
	{
		Undefined,
		WacUrlNetworkIssue,
		WacInvalidResponse,
		WopiEndpointIssue,
		WacExchangePipelineIssue
	}
}
