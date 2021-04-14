using System;

namespace Microsoft.Exchange.Monitoring
{
	public enum SmtpConnectivityStatusCode
	{
		NotTested,
		Success = 1000,
		Error,
		UnableToComplete
	}
}
