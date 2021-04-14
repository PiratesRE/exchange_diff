using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class DagSubnetIdComparer : IComparer<DatabaseAvailabilityGroupSubnetId>
	{
		private DagSubnetIdComparer()
		{
		}

		public int Compare(DatabaseAvailabilityGroupSubnetId subnet1, DatabaseAvailabilityGroupSubnetId subnet2)
		{
			if (subnet1 == null || subnet2 == null)
			{
				if (subnet1 != null)
				{
					return 1;
				}
				if (subnet2 == null)
				{
					return 0;
				}
				return -1;
			}
			else
			{
				IPRange iprange = subnet1.IPRange;
				IPRange iprange2 = subnet2.IPRange;
				if (iprange == iprange2)
				{
					return 0;
				}
				if (iprange.Contains(iprange2.LowerBound))
				{
					return 0;
				}
				if (iprange2.Contains(iprange.LowerBound))
				{
					return 0;
				}
				return iprange.CompareTo(iprange2);
			}
		}

		internal static DagSubnetIdComparer Comparer
		{
			get
			{
				return DagSubnetIdComparer.s_comparer;
			}
		}

		private static DagSubnetIdComparer s_comparer = new DagSubnetIdComparer();
	}
}
