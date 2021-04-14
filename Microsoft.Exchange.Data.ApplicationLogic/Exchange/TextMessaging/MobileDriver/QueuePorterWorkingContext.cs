using System;
using System.Threading;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class QueuePorterWorkingContext<T>
	{
		public QueuePorterWorkingContext(ThreadSafeQueue<T> queue, QueueDataAvailableEventHandler<T> handler, int outstandingWorkersLimitHint)
		{
			if (queue == null)
			{
				throw new ArgumentNullException("queue");
			}
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			if (1 > outstandingWorkersLimitHint)
			{
				throw new ArgumentOutOfRangeException("outstandingWorkersLimitHint");
			}
			this.Queue = queue;
			this.QueueDataAvailableEventHandler = handler;
			this.OutstandingWorkersLimitHint = outstandingWorkersLimitHint;
		}

		public ThreadSafeQueue<T> Queue { get; private set; }

		public int OutstandingWorkersLimitHint { get; private set; }

		private QueueDataAvailableEventHandler<T> QueueDataAvailableEventHandler { get; set; }

		public void OnDataAvailable(QueueDataAvailableEventSource<T> src, QueueDataAvailableEventArgs<T> e)
		{
			try
			{
				if (Interlocked.Increment(ref this.countOfOutstandingWorkers) == (long)this.OutstandingWorkersLimitHint)
				{
					this.Queue.PauseEvent();
				}
				this.QueueDataAvailableEventHandler(src, e);
			}
			finally
			{
				if (Interlocked.Decrement(ref this.countOfOutstandingWorkers) == (long)(this.OutstandingWorkersLimitHint - 1))
				{
					this.Queue.ResumeEvent();
				}
			}
		}

		private long countOfOutstandingWorkers;

		public QueueDataAvailableEventHandler<T> onDataAvailable;
	}
}
