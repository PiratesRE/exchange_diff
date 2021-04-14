using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal class LoggingQueueWrapper : ISchedulerQueue
	{
		public LoggingQueueWrapper(ISchedulerQueue queue)
		{
			ArgumentValidator.ThrowIfNull("queue", queue);
			this.queue = queue;
		}

		public bool IsEmpty
		{
			get
			{
				return this.queue.IsEmpty;
			}
		}

		public long Count
		{
			get
			{
				return this.queue.Count;
			}
		}

		public void Enqueue(ISchedulableMessage message)
		{
			this.queue.Enqueue(message);
			this.queueLog.RecordEnqueue();
		}

		public bool TryDequeue(out ISchedulableMessage message)
		{
			if (this.queue.TryDequeue(out message))
			{
				this.queueLog.RecordDequeue();
				return true;
			}
			return false;
		}

		public bool TryPeek(out ISchedulableMessage message)
		{
			return this.queue.TryPeek(out message);
		}

		public void Flush(DateTime timestamp, QueueLogInfo info)
		{
			info.Count = this.Count;
			this.queueLog.Flush(timestamp, info);
		}

		private readonly ISchedulerQueue queue;

		private readonly QueueLog queueLog = new QueueLog();
	}
}
