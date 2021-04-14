using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal enum ReseedState
	{
		Unknown,
		Resume,
		AssignSpare,
		InPlaceReseed,
		Completed
	}
}
