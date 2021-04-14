using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Collections
{
	[Serializable]
	internal sealed class VirtualList<T> : IList<!0>, ICollection<!0>, IEnumerable<!0>, IEnumerable
	{
		public VirtualList(bool readOnly)
		{
			if (!readOnly)
			{
				throw new NotImplementedException("VirtualList<T> does not yet support writes.");
			}
			this.list = null;
			this.start = -1;
			this.length = -1;
		}

		public bool IsReadOnly
		{
			get
			{
				this.ThrowIfNotInitialized();
				return true;
			}
		}

		public int Count
		{
			get
			{
				this.ThrowIfNotInitialized();
				return this.length;
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
				this.ThrowIfNotInitialized();
				return this.list[index + this.start];
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
				throw new NotImplementedException("VirtualList<T> does not yet support writes.");
			}
		}

		public void SetRange(IList<T> list, int start, int length)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (start >= list.Count)
			{
				throw new ArgumentException("start must be less than the size of the list.");
			}
			if (start + length > list.Count)
			{
				throw new ArgumentException("'start + length' must be less than or equal to the size of the list.");
			}
			this.list = list;
			this.start = start;
			this.length = length;
		}

		public void Add(T value)
		{
			throw new NotImplementedException("VirtualList<T> does not yet support writes.");
		}

		public void Clear()
		{
			throw new NotImplementedException("VirtualList<T> does not yet support writes.");
		}

		public bool Contains(T item)
		{
			throw new NotImplementedException("VirtualList.Contains has not been implemented.");
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			for (int i = 0; i < this.Count; i++)
			{
				array[i + arrayIndex] = this[i];
			}
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException("VirtualList.CopyTo has not been implemented.");
		}

		public IEnumerator<T> GetEnumerator()
		{
			throw new NotImplementedException("VirtualList.GetEnumerator has not been implemented.");
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException("VirtualList.GetEnumerator has not been implemented.");
		}

		public int IndexOf(T item)
		{
			throw new NotImplementedException("VirtualList.IndexOf has not been implemented.");
		}

		public void Insert(int index, T item)
		{
			throw new NotImplementedException("VirtualList.Insert has not been implemented.");
		}

		public bool Remove(T item)
		{
			throw new NotSupportedException("VirtualList<T> does not yet support writes.");
		}

		public void RemoveAt(int index)
		{
			throw new NotSupportedException("VirtualList<T> does not yet support writes.");
		}

		private void ThrowIfNotInitialized()
		{
			if (this.list == null || this.start == -1 || this.length == -1)
			{
				throw new InvalidOperationException("Call VirtualList.SetRange first.");
			}
		}

		private const string ListSerializationName = "list";

		private const string VirtualListIsReadOnly = "VirtualList<T> does not yet support writes.";

		private IList<T> list;

		private int start;

		private int length;
	}
}
