using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal sealed class LoadBalancedCollection<T> : ICollection<T>, IEnumerable<!0>, IEnumerable
	{
		public LoadBalancedCollection(IList<T> list, int roundRobinBase)
		{
			this.list = list;
			this.roundRobinIndex = 0;
			if (this.Count > 0)
			{
				this.roundRobinIndex = roundRobinBase % this.Count;
				if (this.roundRobinIndex < 0)
				{
					this.roundRobinIndex = -this.roundRobinIndex;
				}
			}
		}

		public int Count
		{
			get
			{
				if (this.list != null)
				{
					return this.list.Count;
				}
				return 0;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index", index, "index is out of range");
				}
				return this.list[this.TranslateIndex(index)];
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			int count = this.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					yield return this.list[this.TranslateIndex(i)];
				}
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public bool Contains(T item)
		{
			return this.list != null && this.list.Contains(item);
		}

		public void CopyTo(T[] dest, int index)
		{
			foreach (T t in this)
			{
				dest[index++] = t;
			}
		}

		public void Add(T item)
		{
			throw new NotSupportedException("ListLoadBalancer is read-only");
		}

		public void Clear()
		{
			throw new NotSupportedException("ListLoadBalancer is read-only");
		}

		public bool Remove(T item)
		{
			throw new NotSupportedException("ListLoadBalancer is read-only");
		}

		private int TranslateIndex(int index)
		{
			return (index + this.roundRobinIndex) % this.Count;
		}

		private int roundRobinIndex;

		private IList<T> list;
	}
}
