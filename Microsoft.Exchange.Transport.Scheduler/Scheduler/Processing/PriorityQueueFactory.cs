using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal class PriorityQueueFactory : IQueueFactory
	{
		public PriorityQueueFactory(IDictionary<DeliveryPriority, int> distributions)
		{
			ArgumentValidator.ThrowIfNull("distributions", distributions);
			this.distributions = distributions;
		}

		public ISchedulerQueue CreateNewQueueInstance()
		{
			return new PriorityDistributedQueue(this.distributions);
		}

		private readonly IDictionary<DeliveryPriority, int> distributions;
	}
}
