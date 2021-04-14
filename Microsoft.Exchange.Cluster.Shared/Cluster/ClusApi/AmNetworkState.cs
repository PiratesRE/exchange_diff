using System;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	public enum AmNetworkState
	{
		StateUnknown = -1,
		Unavailable,
		Down,
		Partitioned,
		Up
	}
}
