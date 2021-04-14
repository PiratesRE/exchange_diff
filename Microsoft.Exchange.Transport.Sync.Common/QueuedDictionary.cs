using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class QueuedDictionary<T> : IEnumerable<T>, IEnumerable
	{
		public QueuedDictionary()
		{
			this.lookupAssistor = new Dictionary<T, object>();
			this.internalQueue = new Queue<T>();
		}

		public int Count
		{
			get
			{
				int count;
				lock (this.syncObject)
				{
					count = this.lookupAssistor.Count;
				}
				return count;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			IEnumerator<T> result;
			lock (this.syncObject)
			{
				result = this.internalQueue.GetEnumerator();
			}
			return result;
		}

		public void Clear()
		{
			lock (this.syncObject)
			{
				this.lookupAssistor.Clear();
				this.internalQueue.Clear();
			}
		}

		public bool Contains(T entry)
		{
			bool result;
			lock (this.syncObject)
			{
				result = this.lookupAssistor.ContainsKey(entry);
			}
			return result;
		}

		public void Enqueue(T entry)
		{
			lock (this.syncObject)
			{
				this.lookupAssistor.Add(entry, null);
				this.internalQueue.Enqueue(entry);
			}
		}

		public T Dequeue()
		{
			T result;
			lock (this.syncObject)
			{
				T t = this.internalQueue.Dequeue();
				this.lookupAssistor.Remove(t);
				result = t;
			}
			return result;
		}

		private readonly object syncObject = new object();

		private readonly Dictionary<T, object> lookupAssistor;

		private readonly Queue<T> internalQueue;
	}
}
