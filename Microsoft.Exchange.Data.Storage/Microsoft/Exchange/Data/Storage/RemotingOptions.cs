using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum RemotingOptions
	{
		LocalConnectionsOnly = 0,
		AllowCrossSite = 1,
		AllowCrossPremise = 2,
		AllowHybridAccess = 4
	}
}
