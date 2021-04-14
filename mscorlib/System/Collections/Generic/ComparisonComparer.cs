using System;

namespace System.Collections.Generic
{
	[Serializable]
	internal class ComparisonComparer<T> : Comparer<T>
	{
		public ComparisonComparer(Comparison<T> comparison)
		{
			this._comparison = comparison;
		}

		public override int Compare(T x, T y)
		{
			return this._comparison(x, y);
		}

		private readonly Comparison<T> _comparison;
	}
}
