using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Transport.Sync.Manager.Throttling
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class SyncQueue<T>
	{
		public abstract int Count { get; }

		public abstract void Clear();

		public abstract void Enqueue(T item, WorkType workType, ExDateTime nextPollingTime);

		public abstract void EnqueueAtFront(T item, WorkType workType);

		public abstract T Dequeue(WorkType workType);

		public abstract T Peek(ExDateTime currentTime);

		public abstract bool IsEmpty();

		public abstract ExDateTime NextPollingTime(ExDateTime currentTime);

		public ExDateTime NextPollingTime()
		{
			return this.NextPollingTime(ExDateTime.UtcNow);
		}

		public T Peek()
		{
			return this.Peek(ExDateTime.UtcNow);
		}

		public bool IsWorkDue(ExDateTime currentTime)
		{
			return !this.IsEmpty() && this.NextPollingTime(currentTime) <= currentTime;
		}

		protected void ThrowIfQueueEmpty()
		{
			if (this.IsEmpty())
			{
				throw new InvalidOperationException("Sync Queue is empty.");
			}
		}
	}
}
