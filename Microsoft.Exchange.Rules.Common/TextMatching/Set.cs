using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextMatching
{
	internal sealed class Set<T> : ICollection<T>, IEnumerable<T>, IEnumerable
	{
		public Set()
		{
			this.hash = new Dictionary<T, string>();
		}

		public int Count
		{
			get
			{
				return this.hash.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public void Add(T item)
		{
			if (!this.hash.ContainsKey(item))
			{
				this.hash.Add(item, string.Empty);
			}
		}

		public void Clear()
		{
			this.hash.Clear();
		}

		public bool Remove(T item)
		{
			return this.hash.Remove(item);
		}

		public bool Contains(T item)
		{
			return this.hash.ContainsKey(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.hash.Keys.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.hash.Keys.GetEnumerator();
		}

		private Dictionary<T, string> hash;
	}
}
