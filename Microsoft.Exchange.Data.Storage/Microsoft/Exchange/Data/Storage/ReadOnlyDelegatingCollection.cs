using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class ReadOnlyDelegatingCollection<T> : ICollection<T>, IEnumerable<!0>, IEnumerable
	{
		public void Add(T item)
		{
			throw ReadOnlyDelegatingCollection<T>.ReadOnlyViolation();
		}

		public void Clear()
		{
			throw ReadOnlyDelegatingCollection<T>.ReadOnlyViolation();
		}

		public virtual bool Contains(T item)
		{
			foreach (T y in this)
			{
				if (EqualityComparer<T>.Default.Equals(item, y))
				{
					return true;
				}
			}
			return false;
		}

		public virtual void CopyTo(T[] array, int arrayIndex)
		{
			Util.ThrowOnNullArgument(array, "array");
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex");
			}
			if (arrayIndex + this.Count > array.Length)
			{
				throw new ArgumentException("Cannot fit all elements of a collection into the given array", "array");
			}
			foreach (T t in this)
			{
				array[arrayIndex++] = t;
			}
		}

		public abstract int Count { get; }

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public bool Remove(T item)
		{
			throw ReadOnlyDelegatingCollection<T>.ReadOnlyViolation();
		}

		public abstract IEnumerator<T> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private static Exception ReadOnlyViolation()
		{
			return new NotSupportedException("Collection is read-only");
		}
	}
}
