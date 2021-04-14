using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal enum ProberState
	{
		Idle,
		Sending,
		Sent,
		Gathering,
		Disposed
	}
}
