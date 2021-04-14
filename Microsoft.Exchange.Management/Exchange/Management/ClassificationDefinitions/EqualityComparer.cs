using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal class EqualityComparer<T> : IEqualityComparer<!0>
	{
		internal EqualityComparer(Func<T, T, bool> equalityComparerFn, Func<T, int> hasherFn)
		{
			this.equalityComparerFunc = equalityComparerFn;
			this.hashCodeFunc = hasherFn;
		}

		public bool Equals(T x, T y)
		{
			return this.equalityComparerFunc(x, y);
		}

		public int GetHashCode(T obj)
		{
			return this.hashCodeFunc(obj);
		}

		private readonly Func<T, T, bool> equalityComparerFunc;

		private readonly Func<T, int> hashCodeFunc;
	}
}
