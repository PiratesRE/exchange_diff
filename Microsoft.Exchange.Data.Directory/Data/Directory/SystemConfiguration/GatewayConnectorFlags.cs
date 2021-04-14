using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum GatewayConnectorFlags
	{
		None = 0,
		RelayDsnRequired = 1,
		Disabled = 2
	}
}
