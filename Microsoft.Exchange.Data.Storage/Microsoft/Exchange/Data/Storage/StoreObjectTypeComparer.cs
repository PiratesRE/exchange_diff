using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class StoreObjectTypeComparer : IComparer<StoreObjectType>, IEqualityComparer<StoreObjectType>
	{
		public int Compare(StoreObjectType x, StoreObjectType y)
		{
			return y - x;
		}

		public bool Equals(StoreObjectType x, StoreObjectType y)
		{
			return x == y;
		}

		public int GetHashCode(StoreObjectType tag)
		{
			return (int)tag;
		}
	}
}
