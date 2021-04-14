using System;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal enum AmNodeState
	{
		Unknown = -1,
		Up,
		Down,
		Paused,
		Joining
	}
}
