using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Inference.Common
{
	public static class Extensions
	{
		public static int IndexOf<TSource>(this IEnumerable<TSource> collection, TSource value, IEqualityComparer<TSource> comparer)
		{
			ArgumentValidator.ThrowIfNull("collection", collection);
			ArgumentValidator.ThrowIfNull("comparer", comparer);
			int num = 0;
			foreach (TSource x in collection)
			{
				if (comparer.Equals(x, value))
				{
					return num;
				}
				num++;
			}
			return -1;
		}
	}
}
