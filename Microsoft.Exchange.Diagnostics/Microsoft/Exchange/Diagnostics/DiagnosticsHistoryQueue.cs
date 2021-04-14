using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Diagnostics
{
	internal class DiagnosticsHistoryQueue<T>
	{
		public DiagnosticsHistoryQueue(int limit)
		{
			ArgumentValidator.ThrowIfZeroOrNegative("limit", limit);
			this.limit = limit;
			this.lastQueuedElement = default(T);
		}

		public void Enqueue(T obj)
		{
			ArgumentValidator.ThrowIfNull("obj", obj);
			lock (this.queue)
			{
				this.lastQueuedElement = obj;
				this.queue.Enqueue(obj);
				while (this.queue.Count > this.limit)
				{
					this.queue.Dequeue();
				}
			}
		}

		public T[] ToArray()
		{
			T[] result;
			lock (this.queue)
			{
				result = this.queue.ToArray();
			}
			return result;
		}

		public T GetLastQueuedElement()
		{
			T result;
			lock (this.queue)
			{
				result = this.lastQueuedElement;
			}
			return result;
		}

		public void Clear()
		{
			lock (this.queue)
			{
				this.queue.Clear();
				this.lastQueuedElement = default(T);
			}
		}

		private readonly Queue<T> queue = new Queue<T>();

		private readonly int limit;

		private T lastQueuedElement;
	}
}
