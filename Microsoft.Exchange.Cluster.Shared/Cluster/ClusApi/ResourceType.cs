using System;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal enum ResourceType
	{
		Unknown = -1,
		IPv4Address,
		NetName,
		SA,
		Store,
		DAV,
		Database,
		Disk,
		IPv6Address,
		FileShareWitness,
		MajorityNodeSet,
		FileServer,
		Maximum
	}
}
