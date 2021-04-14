using System;

namespace Microsoft.Exchange.Transport.RemoteDelivery
{
	internal delegate ILockableQueue GetQueueDelegate(NextHopSolutionKey key);
}
