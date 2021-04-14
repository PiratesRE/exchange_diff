using System;
using System.Collections.Generic;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal interface ISchedulerThrottler
	{
		bool ShouldThrottle(IEnumerable<IMessageScope> scopes, out IMessageScope throttledScope);

		bool ShouldThrottle(IMessageScope scope);

		IEnumerable<IMessageScope> GetThrottlingScopes(IEnumerable<IMessageScope> candidateScopes);
	}
}
