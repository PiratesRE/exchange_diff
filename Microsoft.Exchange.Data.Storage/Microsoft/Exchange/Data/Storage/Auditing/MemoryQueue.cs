using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MemoryQueue<T> : DisposeTrackableBase, IQueue<T>, IDisposable
	{
		internal MemoryQueue()
		{
			this.queue = new BlockingCollection<QueueMessage<T>>();
		}

		public void Send(T data)
		{
			base.CheckDisposed();
			QueueMessage<T> item = new QueueMessage<T>
			{
				Data = data
			};
			this.queue.Add(item);
		}

		public IQueueMessage<T> GetNext(int timeoutInMilliseconds, CancellationToken cancel)
		{
			base.CheckDisposed();
			QueueMessage<T> result = null;
			try
			{
				this.queue.TryTake(out result, timeoutInMilliseconds, cancel);
			}
			catch (OperationCanceledException)
			{
			}
			return result;
		}

		public void Commit(IQueueMessage<T> message)
		{
			base.CheckDisposed();
		}

		public void Rollback(IQueueMessage<T> message)
		{
			base.CheckDisposed();
			this.Send(message.Data);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing)
			{
				this.queue.Dispose();
			}
			this.queue = null;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MemoryQueue<T>>(this);
		}

		private BlockingCollection<QueueMessage<T>> queue;
	}
}
