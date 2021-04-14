using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal static class Extensions
	{
		public static IEnumerable<T> Cache<T>(this IEnumerable<T> enumerable)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException("enumerable");
			}
			return new CachedIterator<T>(enumerable.GetEnumerator());
		}
	}
}
