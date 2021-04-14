using System;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal interface IQueueLogWriter
	{
		void Write(QueueLogInfo logInfo);
	}
}
