using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	internal class ProxyQueue<T>
	{
		public ProxyQueue(IEnumerable<T> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			this.items = items.ToArray<T>();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new ProxyQueue<T>.Enumerator(this);
		}

		private T[] items;

		private int head;

		private class Enumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			public Enumerator(ProxyQueue<T> queue)
			{
				this.queue = queue;
				this.Reset();
			}

			public T Current
			{
				get
				{
					if (this.current < 0)
					{
						throw new InvalidOperationException();
					}
					return this.queue.items[this.current];
				}
			}

			public void Reset()
			{
				this.current = -1;
				this.itemsEnumerated = 0;
			}

			public bool MoveNext()
			{
				this.itemsEnumerated++;
				lock (this.queue)
				{
					if (this.itemsEnumerated == 1)
					{
						this.current = this.queue.head;
					}
					else if (this.current != this.queue.head)
					{
						this.current = this.queue.head;
					}
					else
					{
						this.queue.head = ++this.queue.head % this.queue.items.Length;
						this.current = this.queue.head;
					}
				}
				if (this.itemsEnumerated > this.queue.items.Length)
				{
					this.current = -1;
					return false;
				}
				return true;
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			void IDisposable.Dispose()
			{
			}

			private ProxyQueue<T> queue;

			private int current;

			private int itemsEnumerated;
		}
	}
}
