using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Exchange.LogUploaderProxy;

namespace Microsoft.Exchange.LogUploader
{
	internal class ThreadSafeQueue<T> where T : class
	{
		public ThreadSafeQueue() : this(1000)
		{
		}

		public ThreadSafeQueue(int capacity)
		{
			if (capacity <= 0)
			{
				throw new ArgumentOutOfRangeException("capacity should be greater than 0.");
			}
			this.capacity = capacity;
			this.queue = new ConcurrentQueue<T>();
			this.consumerSemaphore = new Semaphore(0, this.capacity);
			this.producerSemaphore = new Semaphore(this.capacity, this.capacity);
			this.ConsumerSemaphoreCount = 0;
			this.ProducerSemaphoreCount = this.capacity;
		}

		public int Count
		{
			get
			{
				return this.queue.Count;
			}
		}

		public int Capacity
		{
			get
			{
				return this.capacity;
			}
			private set
			{
				this.capacity = value;
			}
		}

		public bool IsFull
		{
			get
			{
				return this.capacity == this.Count;
			}
		}

		public int ConsumerSemaphoreCount { get; set; }

		public int ProducerSemaphoreCount { get; set; }

		public void Enqueue(T item, CancellationContext cancellationContext)
		{
			if (cancellationContext == null)
			{
				this.producerSemaphore.WaitOne();
			}
			else if (WaitHandle.WaitAny(new WaitHandle[]
			{
				cancellationContext.StopToken.WaitHandle,
				this.producerSemaphore
			}) != 1)
			{
				ServiceLogger.LogDebug(ServiceLogger.Component.LogReader, LogUploaderEventLogConstants.Message.LogBatchEnqueue, string.Format("Enqueue: called : Type={0}, thread={1}, queueSize={2}, capacity={3} ?---InProducerFullSkip ", new object[]
				{
					typeof(T),
					Thread.CurrentThread.ManagedThreadId,
					this.queue.Count,
					this.Capacity
				}), "", "");
				return;
			}
			this.Enqueue(item);
		}

		public T Dequeue(CancellationContext cancellationContext)
		{
			if (cancellationContext == null)
			{
				this.consumerSemaphore.WaitOne();
			}
			else if (WaitHandle.WaitAny(new WaitHandle[]
			{
				cancellationContext.StopToken.WaitHandle,
				this.consumerSemaphore
			}) != 1)
			{
				ServiceLogger.LogDebug(ServiceLogger.Component.LogReader, LogUploaderEventLogConstants.Message.LogBatchEnqueue, string.Format("Enqueue: called : Type={0}, thread={1}, queueSize={2}, capacity={3} ?---InDequeueEmptySkip ", new object[]
				{
					typeof(T),
					Thread.CurrentThread.ManagedThreadId,
					this.queue.Count,
					this.Capacity
				}), "", "");
				return default(T);
			}
			return this.Dequeue();
		}

		public void Close()
		{
			this.consumerSemaphore.Close();
			this.producerSemaphore.Close();
		}

		private T Dequeue()
		{
			T result = default(T);
			if (!this.queue.TryDequeue(out result))
			{
				ServiceLogger.LogDebug(ServiceLogger.Component.LogReader, LogUploaderEventLogConstants.Message.LogBatchEnqueue, string.Format("Enqueue: called : Type={0}, thread={1}, queueSize={2}, capacity={3} ?---InDequeuExp ", new object[]
				{
					typeof(T),
					Thread.CurrentThread.ManagedThreadId,
					this.queue.Count,
					this.Capacity
				}), "", "");
				throw new Exception("TryDequeue should always return true");
			}
			int num = this.producerSemaphore.Release();
			this.ProducerSemaphoreCount = num + 1;
			ServiceLogger.LogDebug(ServiceLogger.Component.LogReader, LogUploaderEventLogConstants.Message.LogBatchEnqueue, string.Format("Enqueue: called : Type={0}, thread={1}, queueSize={2}, capacity={3}, consumerSemaphore={4} ?---InDequeuDone ", new object[]
			{
				typeof(T),
				Thread.CurrentThread.ManagedThreadId,
				this.queue.Count,
				this.Capacity,
				num
			}), "", "");
			return result;
		}

		private void Enqueue(T item)
		{
			ServiceLogger.LogDebug(ServiceLogger.Component.LogReader, LogUploaderEventLogConstants.Message.LogBatchEnqueue, string.Format("Enqueue: called : Type={0}, thread={1}, queueSize={2}, capacity={3} ?---In ", new object[]
			{
				typeof(T),
				Thread.CurrentThread.ManagedThreadId,
				this.queue.Count,
				this.Capacity
			}), "", "");
			if (this.queue.Count >= this.Capacity)
			{
				ServiceLogger.LogDebug(ServiceLogger.Component.LogReader, LogUploaderEventLogConstants.Message.LogBatchEnqueue, string.Format("Enqueue: called : Type={0}, thread={1}, queueSize={2}, capacity={3} ?---InEnqueuFull ", new object[]
				{
					typeof(T),
					Thread.CurrentThread.ManagedThreadId,
					this.queue.Count,
					this.Capacity
				}), "", "");
				throw new Exception("Queue should never be full here. The caller should have acquired writer quota");
			}
			this.queue.Enqueue(item);
			int num = this.consumerSemaphore.Release();
			this.ConsumerSemaphoreCount = num + 1;
			ServiceLogger.LogDebug(ServiceLogger.Component.LogReader, LogUploaderEventLogConstants.Message.LogBatchEnqueue, string.Format("Enqueue: called : Type={0}, thread={1}, queueSize={2}, capacity={3}, consumerSemaphore={4} ?---InDone ", new object[]
			{
				typeof(T),
				Thread.CurrentThread.ManagedThreadId,
				this.queue.Count,
				this.Capacity,
				num
			}), "", "");
		}

		private readonly ConcurrentQueue<T> queue;

		private int capacity;

		private Semaphore consumerSemaphore;

		private Semaphore producerSemaphore;
	}
}
