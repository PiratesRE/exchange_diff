using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class ADObjectIdEqualityComparer : IEqualityComparer<ADObjectId>
	{
		public bool Equals(ADObjectId x, ADObjectId y)
		{
			return ADObjectId.Equals(x, y);
		}

		public int GetHashCode(ADObjectId obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			return obj.GetHashCode();
		}

		internal static readonly ADObjectIdEqualityComparer Instance = new ADObjectIdEqualityComparer();
	}
}
