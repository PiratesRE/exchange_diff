using System;
using System.Collections;
using System.Collections.Generic;

namespace System
{
	[__DynamicallyInvokable]
	[Serializable]
	public struct ArraySegment<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IReadOnlyList<T>, IReadOnlyCollection<T>
	{
		[__DynamicallyInvokable]
		public ArraySegment(T[] array)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			this._array = array;
			this._offset = 0;
			this._count = array.Length;
		}

		[__DynamicallyInvokable]
		public ArraySegment(T[] array, int offset, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (array.Length - offset < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
			}
			this._array = array;
			this._offset = offset;
			this._count = count;
		}

		[__DynamicallyInvokable]
		public T[] Array
		{
			[__DynamicallyInvokable]
			get
			{
				return this._array;
			}
		}

		[__DynamicallyInvokable]
		public int Offset
		{
			[__DynamicallyInvokable]
			get
			{
				return this._offset;
			}
		}

		[__DynamicallyInvokable]
		public int Count
		{
			[__DynamicallyInvokable]
			get
			{
				return this._count;
			}
		}

		[__DynamicallyInvokable]
		public override int GetHashCode()
		{
			if (this._array != null)
			{
				return this._array.GetHashCode() ^ this._offset ^ this._count;
			}
			return 0;
		}

		[__DynamicallyInvokable]
		public override bool Equals(object obj)
		{
			return obj is ArraySegment<T> && this.Equals((ArraySegment<T>)obj);
		}

		[__DynamicallyInvokable]
		public bool Equals(ArraySegment<T> obj)
		{
			return obj._array == this._array && obj._offset == this._offset && obj._count == this._count;
		}

		[__DynamicallyInvokable]
		public static bool operator ==(ArraySegment<T> a, ArraySegment<T> b)
		{
			return a.Equals(b);
		}

		[__DynamicallyInvokable]
		public static bool operator !=(ArraySegment<T> a, ArraySegment<T> b)
		{
			return !(a == b);
		}

		[__DynamicallyInvokable]
		T IList<!0>.this[int index]
		{
			[__DynamicallyInvokable]
			get
			{
				if (this._array == null)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NullArray"));
				}
				if (index < 0 || index >= this._count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return this._array[this._offset + index];
			}
			[__DynamicallyInvokable]
			set
			{
				if (this._array == null)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NullArray"));
				}
				if (index < 0 || index >= this._count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				this._array[this._offset + index] = value;
			}
		}

		[__DynamicallyInvokable]
		int IList<!0>.IndexOf(T item)
		{
			if (this._array == null)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NullArray"));
			}
			int num = System.Array.IndexOf<T>(this._array, item, this._offset, this._count);
			if (num < 0)
			{
				return -1;
			}
			return num - this._offset;
		}

		[__DynamicallyInvokable]
		void IList<!0>.Insert(int index, T item)
		{
			throw new NotSupportedException();
		}

		[__DynamicallyInvokable]
		void IList<!0>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		[__DynamicallyInvokable]
		T IReadOnlyList<!0>.this[int index]
		{
			[__DynamicallyInvokable]
			get
			{
				if (this._array == null)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NullArray"));
				}
				if (index < 0 || index >= this._count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return this._array[this._offset + index];
			}
		}

		[__DynamicallyInvokable]
		bool ICollection<!0>.IsReadOnly
		{
			[__DynamicallyInvokable]
			get
			{
				return true;
			}
		}

		[__DynamicallyInvokable]
		void ICollection<!0>.Add(T item)
		{
			throw new NotSupportedException();
		}

		[__DynamicallyInvokable]
		void ICollection<!0>.Clear()
		{
			throw new NotSupportedException();
		}

		[__DynamicallyInvokable]
		bool ICollection<!0>.Contains(T item)
		{
			if (this._array == null)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NullArray"));
			}
			int num = System.Array.IndexOf<T>(this._array, item, this._offset, this._count);
			return num >= 0;
		}

		[__DynamicallyInvokable]
		void ICollection<!0>.CopyTo(T[] array, int arrayIndex)
		{
			if (this._array == null)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NullArray"));
			}
			System.Array.Copy(this._array, this._offset, array, arrayIndex, this._count);
		}

		[__DynamicallyInvokable]
		bool ICollection<!0>.Remove(T item)
		{
			throw new NotSupportedException();
		}

		[__DynamicallyInvokable]
		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			if (this._array == null)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NullArray"));
			}
			return new ArraySegment<T>.ArraySegmentEnumerator(this);
		}

		[__DynamicallyInvokable]
		IEnumerator IEnumerable.GetEnumerator()
		{
			if (this._array == null)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NullArray"));
			}
			return new ArraySegment<T>.ArraySegmentEnumerator(this);
		}

		private T[] _array;

		private int _offset;

		private int _count;

		[Serializable]
		private sealed class ArraySegmentEnumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			internal ArraySegmentEnumerator(ArraySegment<T> arraySegment)
			{
				this._array = arraySegment._array;
				this._start = arraySegment._offset;
				this._end = this._start + arraySegment._count;
				this._current = this._start - 1;
			}

			public bool MoveNext()
			{
				if (this._current < this._end)
				{
					this._current++;
					return this._current < this._end;
				}
				return false;
			}

			public T Current
			{
				get
				{
					if (this._current < this._start)
					{
						throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_EnumNotStarted"));
					}
					if (this._current >= this._end)
					{
						throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_EnumEnded"));
					}
					return this._array[this._current];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			void IEnumerator.Reset()
			{
				this._current = this._start - 1;
			}

			public void Dispose()
			{
			}

			private T[] _array;

			private int _start;

			private int _end;

			private int _current;
		}
	}
}
