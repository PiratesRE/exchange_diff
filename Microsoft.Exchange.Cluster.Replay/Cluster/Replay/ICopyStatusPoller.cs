using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal interface ICopyStatusPoller
	{
		bool TryWaitForInitialization();
	}
}
