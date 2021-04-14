using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Flags]
	internal enum FrontendTransportFlags
	{
		None = 0,
		AntispamAgentsEnabled = 1,
		ConnectivityLogEnabled = 2,
		ExternalDNSAdapterDisabled = 4,
		InternalDNSAdapterDisabled = 8
	}
}
