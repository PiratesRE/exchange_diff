using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	public static class ToStringHelpers
	{
		public static string SafeToString(object item)
		{
			if (item == null)
			{
				return string.Empty;
			}
			return item.ToString();
		}

		public static string SafeEnumerableToString<T>(IEnumerable<T> item, string separator)
		{
			if (string.IsNullOrEmpty(separator))
			{
				throw new ArgumentException("separator");
			}
			if (item == null)
			{
				return string.Empty;
			}
			return string.Join(separator, (from n in item
			where n != null
			select n.ToString()).ToArray<string>());
		}
	}
}
