using System;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal enum AmNetInterfaceState
	{
		Unknown = -1,
		Unavailable,
		Failed,
		Unreachable,
		Up
	}
}
