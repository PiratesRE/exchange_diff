using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Collections
{
	[Serializable]
	public class ShortList<T> : IList<!0>, ICollection<!0>, IEnumerable<!0>, IEnumerable
	{
		public ShortList()
		{
		}

		public ShortList(IEnumerable<T> enumerable) : this()
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException("enumerable");
			}
			foreach (T item in enumerable)
			{
				this.Add(item);
			}
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		private List<T> List
		{
			get
			{
				if (this.list == null)
				{
					this.list = new List<T>();
				}
				return this.list;
			}
		}

		public T this[int index]
		{
			get
			{
				if (index >= this.count)
				{
					throw new ArgumentOutOfRangeException("index", "Index is out of range");
				}
				if (index > 0)
				{
					return this.list[index - 1];
				}
				return this.firstItem;
			}
			set
			{
				if (index >= this.count)
				{
					throw new ArgumentOutOfRangeException("index", "Index is out of range");
				}
				if (index > 0)
				{
					this.list[index - 1] = value;
				}
				else
				{
					this.firstItem = value;
				}
				this.version++;
			}
		}

		public int IndexOf(T item)
		{
			if (this.count == 0)
			{
				return -1;
			}
			if (ShortList<T>.comparer.Equals(this.firstItem, item))
			{
				return 0;
			}
			if (this.list != null)
			{
				int num = this.list.IndexOf(item);
				if (num != -1)
				{
					return num + 1;
				}
			}
			return -1;
		}

		public void Insert(int index, T item)
		{
			if (index > this.count)
			{
				throw new ArgumentOutOfRangeException("index", "Index is out of range");
			}
			if (index > 0)
			{
				this.List.Insert(index - 1, item);
			}
			else
			{
				if (this.count > 0)
				{
					this.List.Insert(0, this.firstItem);
				}
				this.firstItem = item;
			}
			this.count++;
			this.version++;
		}

		public void RemoveAt(int index)
		{
			if (index >= this.count)
			{
				throw new ArgumentOutOfRangeException("index", "Index is out of range");
			}
			if (index > 0)
			{
				this.list.RemoveAt(index - 1);
			}
			else if (this.count > 1)
			{
				this.firstItem = this.list[0];
				this.list.RemoveAt(0);
			}
			else
			{
				this.firstItem = default(T);
			}
			this.count--;
			this.version++;
		}

		public void Add(T item)
		{
			this.Insert(this.count, item);
		}

		public void Clear()
		{
			if (this.list != null)
			{
				this.list.Clear();
			}
			this.firstItem = default(T);
			this.count = 0;
			this.version++;
		}

		public bool Contains(T item)
		{
			return this.IndexOf(item) != -1;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Length - arrayIndex < this.count)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
			if (this.count == 0)
			{
				return;
			}
			array[arrayIndex] = this.firstItem;
			if (this.list != null)
			{
				this.list.CopyTo(array, arrayIndex + 1);
			}
		}

		public bool Remove(T item)
		{
			int num = this.IndexOf(item);
			if (num != -1)
			{
				this.RemoveAt(num);
				return true;
			}
			return false;
		}

		public ShortList<T>.Enumerator GetEnumerator()
		{
			return new ShortList<T>.Enumerator(this);
		}

		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private static EqualityComparer<T> comparer = EqualityComparer<T>.Default;

		private int count;

		private int version;

		private T firstItem;

		private List<T> list;

		[Serializable]
		public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			internal Enumerator(ShortList<T> list)
			{
				this.list = list;
				this.index = 0;
				this.version = this.list.version;
				this.current = default(T);
			}

			public T Current
			{
				get
				{
					return this.current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					if (this.index == 0 || this.index == this.list.count + 1)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					return this.current;
				}
			}

			public void Dispose()
			{
				this.list = null;
				this.current = default(T);
			}

			public bool MoveNext()
			{
				if (this.version != this.list.version)
				{
					throw new InvalidOperationException("List has been changed during enumeration.");
				}
				if (this.index < this.list.count)
				{
					this.current = this.list[this.index];
					this.index++;
					return true;
				}
				this.index = this.list.count + 1;
				this.current = default(T);
				return false;
			}

			void IEnumerator.Reset()
			{
				if (this.version != this.list.version)
				{
					throw new InvalidOperationException("List has been changed during enumeration.");
				}
				this.index = 0;
				this.current = default(T);
			}

			private ShortList<T> list;

			private int index;

			private int version;

			private T current;
		}
	}
}
