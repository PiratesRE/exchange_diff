using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal interface IQueueLogProvider
	{
		IEnumerable<QueueLogInfo> FlushLogs(DateTime checkpoint, ISchedulerMetering metering);
	}
}
