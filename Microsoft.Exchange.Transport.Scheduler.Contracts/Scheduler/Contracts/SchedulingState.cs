using System;

namespace Microsoft.Exchange.Transport.Scheduler.Contracts
{
	internal enum SchedulingState
	{
		Received,
		Scoped,
		Unscoped,
		Dispatched
	}
}
