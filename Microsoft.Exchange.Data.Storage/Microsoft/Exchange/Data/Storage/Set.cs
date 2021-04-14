using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class Set<T> : ICollection<T>, IEnumerable<T>, IEnumerable
	{
		public int Count
		{
			get
			{
				return this.dataSet.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public Set(int capacity)
		{
			this.dataSet = new Dictionary<T, bool>(capacity);
		}

		public Set()
		{
			this.dataSet = new Dictionary<T, bool>();
		}

		public Set(IList<T> list) : this(list.Count)
		{
			foreach (T element in list)
			{
				this.Add(element);
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.dataSet.Keys.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Add(T element)
		{
			this.dataSet.Add(element, false);
		}

		public void SafeAdd(T element)
		{
			if (!this.dataSet.ContainsKey(element))
			{
				this.dataSet.Add(element, false);
			}
		}

		public T[] ToArray()
		{
			T[] array = new T[this.dataSet.Keys.Count];
			this.dataSet.Keys.CopyTo(array, 0);
			return array;
		}

		public void Clear()
		{
			this.dataSet.Clear();
		}

		public bool Contains(T element)
		{
			return this.dataSet.ContainsKey(element);
		}

		public bool Remove(T element)
		{
			return this.dataSet.Remove(element);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.dataSet.Keys.CopyTo(array, arrayIndex);
		}

		private readonly Dictionary<T, bool> dataSet;
	}
}
