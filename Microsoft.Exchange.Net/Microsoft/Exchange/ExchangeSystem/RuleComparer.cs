using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.ExchangeSystem
{
	internal sealed class RuleComparer : IComparer<ExTimeZoneRule>
	{
		private RuleComparer()
		{
		}

		public int Compare(ExTimeZoneRule r1, ExTimeZoneRule r2)
		{
			if (r1 == null && r2 == null)
			{
				return 0;
			}
			if (r1 == null && r2 != null)
			{
				return -1;
			}
			if (r1 != null && r2 == null)
			{
				return 1;
			}
			if (r1.ObservanceEnd == null && r2.ObservanceEnd == null)
			{
				return 0;
			}
			if (r1.ObservanceEnd == null && r2.ObservanceEnd != null)
			{
				return -1;
			}
			if (r1.ObservanceEnd != null && r2.ObservanceEnd == null)
			{
				return 1;
			}
			return r1.ObservanceEnd.CompareTo(r2.ObservanceEnd);
		}

		public static RuleComparer Instance = new RuleComparer();
	}
}
