using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum RemoteDomainFlags
	{
		None = 0,
		TrustedMailOutboundEnabled = 1,
		TrustedMailInboundEnabled = 2
	}
}
