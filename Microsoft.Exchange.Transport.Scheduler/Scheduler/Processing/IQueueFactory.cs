using System;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal interface IQueueFactory
	{
		ISchedulerQueue CreateNewQueueInstance();
	}
}
