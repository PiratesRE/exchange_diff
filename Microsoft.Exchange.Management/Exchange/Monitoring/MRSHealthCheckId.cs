using System;

namespace Microsoft.Exchange.Monitoring
{
	public enum MRSHealthCheckId
	{
		None,
		ServiceCheck,
		RPCPingCheck,
		QueueScanCheck,
		MRSProxyPingCheck
	}
}
