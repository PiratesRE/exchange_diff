using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	public enum UMIPGatewayFlags2
	{
		None = 0,
		MessageWaitingIndicatorAllowed = 1,
		IPv4Enabled = 2,
		All = -1
	}
}
