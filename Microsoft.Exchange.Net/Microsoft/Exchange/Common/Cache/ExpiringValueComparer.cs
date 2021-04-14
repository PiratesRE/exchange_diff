using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Common.Cache
{
	internal class ExpiringValueComparer : IComparer<IExpiringValue>
	{
		public int Compare(IExpiringValue x, IExpiringValue y)
		{
			if (x != null)
			{
				return DateTime.Compare(x.ExpirationTime, y.ExpirationTime);
			}
			if (y == null)
			{
				return 0;
			}
			return -1;
		}
	}
}
