using System;

namespace Microsoft.Exchange.Transport
{
	internal enum PriorityBehaviour
	{
		IgnorePriority,
		RoundRobin,
		Fixed,
		QueuePriority
	}
}
