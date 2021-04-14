using System;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class QueueDataAvailableEventSource<T>
	{
		public QueueDataAvailableEventSource(ThreadSafeQueue<T> queue)
		{
			if (queue == null)
			{
				throw new ArgumentNullException("queue");
			}
			this.Queue = queue;
		}

		public ThreadSafeQueue<T> Queue { get; private set; }
	}
}
