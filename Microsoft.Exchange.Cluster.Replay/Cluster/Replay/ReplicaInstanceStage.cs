using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal enum ReplicaInstanceStage
	{
		Unknown,
		Initializing,
		Resynchronizing,
		Running
	}
}
