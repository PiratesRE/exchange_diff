using System;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[Flags]
	internal enum AmConfigChangedFlags
	{
		None = 0,
		Role = 1,
		DbState = 2,
		LastError = 4,
		DagConfig = 8,
		DagId = 16,
		MemberServers = 32,
		CurrentPAM = 64,
		Cluster = 128,
		CoreGroup = 256
	}
}
