using System;

namespace System.Threading
{
	internal struct SparselyPopulatedArrayAddInfo<T> where T : class
	{
		internal SparselyPopulatedArrayAddInfo(SparselyPopulatedArrayFragment<T> source, int index)
		{
			this.m_source = source;
			this.m_index = index;
		}

		internal SparselyPopulatedArrayFragment<T> Source
		{
			get
			{
				return this.m_source;
			}
		}

		internal int Index
		{
			get
			{
				return this.m_index;
			}
		}

		private SparselyPopulatedArrayFragment<T> m_source;

		private int m_index;
	}
}
