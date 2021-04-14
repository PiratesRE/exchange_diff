using System;

namespace System.Threading
{
	internal class SparselyPopulatedArrayFragment<T> where T : class
	{
		internal SparselyPopulatedArrayFragment(int size) : this(size, null)
		{
		}

		internal SparselyPopulatedArrayFragment(int size, SparselyPopulatedArrayFragment<T> prev)
		{
			this.m_elements = new T[size];
			this.m_freeCount = size;
			this.m_prev = prev;
		}

		internal T this[int index]
		{
			get
			{
				return Volatile.Read<T>(ref this.m_elements[index]);
			}
		}

		internal int Length
		{
			get
			{
				return this.m_elements.Length;
			}
		}

		internal SparselyPopulatedArrayFragment<T> Prev
		{
			get
			{
				return this.m_prev;
			}
		}

		internal T SafeAtomicRemove(int index, T expectedElement)
		{
			T t = Interlocked.CompareExchange<T>(ref this.m_elements[index], default(T), expectedElement);
			if (t != null)
			{
				this.m_freeCount++;
			}
			return t;
		}

		internal readonly T[] m_elements;

		internal volatile int m_freeCount;

		internal volatile SparselyPopulatedArrayFragment<T> m_next;

		internal volatile SparselyPopulatedArrayFragment<T> m_prev;
	}
}
