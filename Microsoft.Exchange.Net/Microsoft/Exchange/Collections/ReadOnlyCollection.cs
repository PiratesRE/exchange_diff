using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Collections
{
	[Serializable]
	public sealed class ReadOnlyCollection<T> : IList<T>, ICollection<!0>, IEnumerable<!0>, IList, ICollection, IEnumerable, ISerializable
	{
		public ReadOnlyCollection(IList<T> list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			this.list = list;
			this.array = (list as T[]);
		}

		private ReadOnlyCollection(SerializationInfo info, StreamingContext context)
		{
			this.list = (IList<T>)info.GetValue("list", typeof(IList<T>));
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public int Count
		{
			get
			{
				if (this.array != null)
				{
					return this.array.Length;
				}
				return this.list.Count;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return true;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				ICollection collection = this.list as ICollection;
				return collection != null && collection.IsSynchronized;
			}
		}

		public object SyncRoot
		{
			get
			{
				ICollection collection = this.list as ICollection;
				if (collection != null)
				{
					return collection.SyncRoot;
				}
				throw new NotSupportedException("Internal collection does not implement ICollection.");
			}
		}

		public T this[int index]
		{
			get
			{
				if (this.array != null)
				{
					return this.array[index];
				}
				return this.list[index];
			}
		}

		T IList<!0>.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new NotSupportedException("Collection is read-only.");
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new NotSupportedException("Collection is read-only.");
			}
		}

		void ICollection<!0>.Add(T item)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		int IList.Add(object value)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		void ICollection<!0>.Clear()
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		void IList.Clear()
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		public bool Contains(T item)
		{
			return this.list.Contains(item);
		}

		bool IList.Contains(object value)
		{
			return ((IList)this.list).Contains(value);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.list.CopyTo(array, arrayIndex);
		}

		public void CopyTo(Array array, int index)
		{
			this.CopyTo(array as T[], index);
		}

		public ReadOnlyCollection<T>.Enumerator GetEnumerator()
		{
			if (this.array != null)
			{
				return new ReadOnlyCollection<T>.Enumerator(this.array);
			}
			return new ReadOnlyCollection<T>.Enumerator(this.list);
		}

		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("list", this.list, typeof(IList<!0>));
		}

		public int IndexOf(T item)
		{
			return this.list.IndexOf(item);
		}

		int IList.IndexOf(object value)
		{
			return ((IList)this.list).IndexOf(value);
		}

		void IList<!0>.Insert(int index, T item)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		bool ICollection<!0>.Remove(T item)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		void IList<!0>.RemoveAt(int index)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		private const string ListSerializationName = "list";

		private IList<T> list;

		private T[] array;

		[Serializable]
		public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			internal Enumerator(IList<T> list)
			{
				this.currentIndex = -1;
				this.currentItem = default(T);
				this.list = list;
				this.array = null;
				this.count = list.Count;
			}

			internal Enumerator(T[] array)
			{
				this.currentIndex = -1;
				this.currentItem = default(T);
				this.list = null;
				this.array = array;
				this.count = array.Length;
			}

			public T Current
			{
				get
				{
					return this.currentItem;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				if (++this.currentIndex < this.count)
				{
					this.currentItem = ((this.array != null) ? this.array[this.currentIndex] : this.list[this.currentIndex]);
					return true;
				}
				return false;
			}

			public void Reset()
			{
				this.currentIndex = -1;
				this.currentItem = default(T);
			}

			private int currentIndex;

			private T currentItem;

			private IList<T> list;

			private T[] array;

			private int count;
		}
	}
}
