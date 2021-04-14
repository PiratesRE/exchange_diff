using System;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal class QueueLog
	{
		public void RecordEnqueue()
		{
			this.enqueues += 1L;
		}

		public void RecordDequeue()
		{
			this.dequeues += 1L;
		}

		public void Flush(DateTime timestamp, QueueLogInfo info)
		{
			info.Dequeues = this.dequeues;
			info.Enqueues = this.enqueues;
			this.FlushInternal(timestamp, info);
			this.Reset(timestamp);
		}

		protected virtual void FlushInternal(DateTime timestamp, QueueLogInfo info)
		{
		}

		protected virtual void ResetInternal(DateTime timestamp)
		{
		}

		private void Reset(DateTime timestamp)
		{
			this.enqueues = 0L;
			this.dequeues = 0L;
			this.ResetInternal(timestamp);
		}

		private long enqueues;

		private long dequeues;
	}
}
