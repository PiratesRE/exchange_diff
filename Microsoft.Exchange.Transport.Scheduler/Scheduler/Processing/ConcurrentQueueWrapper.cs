using System;
using System.Collections.Concurrent;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal class ConcurrentQueueWrapper : ISchedulerQueue
	{
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
				return (long)this.queue.Count;
			}
		}

		public void Enqueue(ISchedulableMessage message)
		{
			ArgumentValidator.ThrowIfNull("message", message);
			this.queue.Enqueue(message);
		}

		public bool TryDequeue(out ISchedulableMessage message)
		{
			return this.queue.TryDequeue(out message);
		}

		public bool TryPeek(out ISchedulableMessage message)
		{
			return this.queue.TryPeek(out message);
		}

		private readonly ConcurrentQueue<ISchedulableMessage> queue = new ConcurrentQueue<ISchedulableMessage>();
	}
}
