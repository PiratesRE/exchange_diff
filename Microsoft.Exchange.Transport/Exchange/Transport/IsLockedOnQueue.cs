using System;

namespace Microsoft.Exchange.Transport
{
	internal delegate bool IsLockedOnQueue(WaitCondition condition, NextHopSolutionKey queue);
}
