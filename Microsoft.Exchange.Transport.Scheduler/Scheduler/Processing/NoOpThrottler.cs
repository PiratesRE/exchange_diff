using System;
using System.Collections.Generic;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal class NoOpThrottler : ISchedulerThrottler
	{
		public bool ShouldThrottle(IEnumerable<IMessageScope> scopes, out IMessageScope throttledScope)
		{
			throttledScope = null;
			return false;
		}

		public bool ShouldThrottle(IMessageScope scope)
		{
			return false;
		}

		public IEnumerable<IMessageScope> GetThrottlingScopes(IEnumerable<IMessageScope> candidateScopes)
		{
			return candidateScopes;
		}
	}
}
