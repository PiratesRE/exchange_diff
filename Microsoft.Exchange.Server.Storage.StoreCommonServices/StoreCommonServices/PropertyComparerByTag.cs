using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class PropertyComparerByTag : IComparer<Property>
	{
		public static PropertyComparerByTag Comparer
		{
			get
			{
				if (PropertyComparerByTag.comparer == null)
				{
					PropertyComparerByTag.comparer = new PropertyComparerByTag();
				}
				return PropertyComparerByTag.comparer;
			}
		}

		public int Compare(Property x, Property y)
		{
			return x.Tag.CompareTo(y.Tag);
		}

		private static PropertyComparerByTag comparer;
	}
}
