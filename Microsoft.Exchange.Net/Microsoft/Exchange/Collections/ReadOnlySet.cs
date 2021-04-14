using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Collections
{
	[Serializable]
	internal class ReadOnlySet<T> : ICollection<!0>, IEnumerable<!0>, ICollection, IEnumerable
	{
		public ReadOnlySet(ICollection<T> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			this.collection = collection;
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
				return this.collection.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				ICollection collection = this.collection as ICollection;
				return collection != null && collection.IsSynchronized;
			}
		}

		public object SyncRoot
		{
			get
			{
				ICollection collection = this.collection as ICollection;
				if (collection != null)
				{
					return collection.SyncRoot;
				}
				throw new NotSupportedException("Internal collection does not implement ICollection.");
			}
		}

		void ICollection<!0>.Add(T item)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		void ICollection<!0>.Clear()
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		public bool Contains(T item)
		{
			return this.collection.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.collection.CopyTo(array, arrayIndex);
		}

		public void CopyTo(Array array, int index)
		{
			this.CopyTo(array as T[], index);
		}

		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			return this.collection.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.collection.GetEnumerator();
		}

		bool ICollection<!0>.Remove(T item)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		private ICollection<T> collection;
	}
}
