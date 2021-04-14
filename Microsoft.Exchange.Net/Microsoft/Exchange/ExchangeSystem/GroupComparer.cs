using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.ExchangeSystem
{
	internal sealed class GroupComparer : IComparer<ExTimeZoneRuleGroup>
	{
		public int Compare(ExTimeZoneRuleGroup g1, ExTimeZoneRuleGroup g2)
		{
			long num = (g1.EndTransition != null) ? g1.EndTransition.Value.Ticks : DateTime.MaxValue.Ticks;
			long num2 = (g2.EndTransition != null) ? g2.EndTransition.Value.Ticks : DateTime.MaxValue.Ticks;
			return Math.Sign(num - num2);
		}

		public static GroupComparer Instance = new GroupComparer();
	}
}
