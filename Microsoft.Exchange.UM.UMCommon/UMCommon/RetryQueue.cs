using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class RetryQueue<T> where T : class
	{
		public RetryQueue(Trace tracer, TimeSpan retryInterval)
		{
			this.retryInterval = retryInterval;
			this.tracer = tracer;
			this.itemList = new List<RetryQueue<T>.QueueItem<T>>();
		}

		public TimeSpan RetryInterval
		{
			get
			{
				return this.retryInterval;
			}
			set
			{
				this.retryInterval = value;
			}
		}

		public int Count
		{
			get
			{
				return this.itemList.Count;
			}
		}

		public void Enqueue(T item)
		{
			ExDateTime exDateTime = ExDateTime.UtcNow.Add(this.retryInterval);
			CallIdTracer.TraceDebug(this.tracer, this.GetHashCode(), "RetryQueue: Enqueue {0} for retry at {1}.", new object[]
			{
				item.GetHashCode(),
				exDateTime
			});
			this.itemList.Add(new RetryQueue<T>.QueueItem<T>(item, exDateTime));
		}

		public T Dequeue()
		{
			return this.Dequeue(false);
		}

		public T Dequeue(bool forceDequeue)
		{
			if (this.itemList.Count == 0 || (!forceDequeue && !this.itemList[0].Expired))
			{
				return default(T);
			}
			RetryQueue<T>.QueueItem<T> queueItem = this.itemList[0];
			this.itemList.RemoveAt(0);
			CallIdTracer.TraceDebug(this.tracer, this.GetHashCode(), "Dequeue {0} at {1}.", new object[]
			{
				queueItem.GetHashCode(),
				queueItem.Expiry
			});
			return queueItem.Item;
		}

		public void Clear()
		{
			this.itemList.Clear();
		}

		public void CopyTo(List<T> serverList)
		{
			for (int i = 0; i < this.itemList.Count; i++)
			{
				serverList.Add(this.itemList[i].Item);
			}
		}

		public void DeleteInvalid(IList<T> validList)
		{
			Dictionary<T, bool> dictionary = new Dictionary<T, bool>();
			foreach (T key in validList)
			{
				dictionary.Add(key, true);
			}
			int i = 0;
			while (i < this.itemList.Count)
			{
				RetryQueue<T>.QueueItem<T> queueItem = this.itemList[i];
				if (!dictionary.ContainsKey(queueItem.Item))
				{
					this.itemList.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
		}

		public bool Contains(T item)
		{
			for (int i = 0; i < this.itemList.Count; i++)
			{
				T item2 = this.itemList[i].Item;
				if (item.Equals(item2))
				{
					return true;
				}
			}
			return false;
		}

		public bool Remove(T item)
		{
			int num = -1;
			for (int i = 0; i < this.itemList.Count; i++)
			{
				T item2 = this.itemList[i].Item;
				if (item2.Equals(item))
				{
					num = i;
					break;
				}
			}
			if (num != -1)
			{
				this.itemList.RemoveAt(num);
				return true;
			}
			return false;
		}

		private TimeSpan retryInterval;

		private Trace tracer;

		private List<RetryQueue<T>.QueueItem<T>> itemList;

		private class QueueItem<Q> : IEquatable<Q> where Q : class
		{
			public QueueItem(Q item, ExDateTime expiry)
			{
				this.item = item;
				this.expiry = expiry;
			}

			public ExDateTime Expiry
			{
				get
				{
					return this.expiry;
				}
			}

			public bool Expired
			{
				get
				{
					return ExDateTime.Compare(ExDateTime.UtcNow, this.expiry) > 0;
				}
			}

			public Q Item
			{
				get
				{
					return this.item;
				}
			}

			public override bool Equals(object obj)
			{
				if (obj == null)
				{
					return false;
				}
				Q q = obj as Q;
				if (q != null)
				{
					return this.item.Equals(q);
				}
				return base.Equals(obj);
			}

			public override int GetHashCode()
			{
				return this.item.GetHashCode();
			}

			public bool Equals(Q other)
			{
				Q q = this.Item;
				return q.Equals(other);
			}

			private Q item;

			private ExDateTime expiry;
		}
	}
}
