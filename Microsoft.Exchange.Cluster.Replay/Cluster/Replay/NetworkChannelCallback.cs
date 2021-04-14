using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal delegate void NetworkChannelCallback(object asyncState, int bytesAvailable, bool completedSynchronously, Exception e);
}
