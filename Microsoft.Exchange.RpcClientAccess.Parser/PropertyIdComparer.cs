using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal class PropertyIdComparer : IComparer<PropertyId>, IEqualityComparer<PropertyId>
	{
		public int Compare(PropertyId x, PropertyId y)
		{
			return (int)(y - x);
		}

		public bool Equals(PropertyId x, PropertyId y)
		{
			return x == y;
		}

		public int GetHashCode(PropertyId tag)
		{
			return (int)tag;
		}

		public static readonly PropertyIdComparer Instance = new PropertyIdComparer();
	}
}
