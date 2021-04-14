using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum TrustDirectionFlag
	{
		None = 0,
		Inbound = 1,
		Outbound = 2,
		Bidirectional = 3
	}
}
