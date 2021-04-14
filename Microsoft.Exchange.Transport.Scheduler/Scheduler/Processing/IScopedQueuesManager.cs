using System;
using System.Collections.Generic;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal interface IScopedQueuesManager
	{
		IDictionary<IMessageScope, ScopedQueue> ScopedQueue { get; }

		void Add(ISchedulableMessage message, IMessageScope throttledScope);

		bool IsAlreadyScoped(IEnumerable<IMessageScope> scopes, out IMessageScope throttledScope);

		bool TryGetNext(out ISchedulableMessage message);
	}
}
