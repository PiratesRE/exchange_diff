using System;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal class ConcurrentQueueFactory : IQueueFactory
	{
		public ISchedulerQueue CreateNewQueueInstance()
		{
			return new ConcurrentQueueWrapper();
		}
	}
}
