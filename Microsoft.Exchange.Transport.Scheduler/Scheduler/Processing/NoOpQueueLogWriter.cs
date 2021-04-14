using System;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal class NoOpQueueLogWriter : IQueueLogWriter
	{
		public void Write(QueueLogInfo logInfo)
		{
		}
	}
}
