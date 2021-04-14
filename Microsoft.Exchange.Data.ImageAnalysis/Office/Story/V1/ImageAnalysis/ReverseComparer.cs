using System;
using System.Collections.Generic;

namespace Microsoft.Office.Story.V1.ImageAnalysis
{
	internal class ReverseComparer<T> : IComparer<T>
	{
		public ReverseComparer(IComparer<T> comparer)
		{
			this.comparer = (comparer ?? Comparer<T>.Default);
		}

		public int Compare(T first, T second)
		{
			return this.comparer.Compare(second, first);
		}

		private readonly IComparer<T> comparer;
	}
}
