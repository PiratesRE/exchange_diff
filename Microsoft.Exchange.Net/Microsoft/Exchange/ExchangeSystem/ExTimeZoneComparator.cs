using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.ExchangeSystem
{
	internal class ExTimeZoneComparator : IComparer<ExTimeZone>
	{
		public int Compare(ExTimeZone t1, ExTimeZone t2)
		{
			int num = t1.TimeZoneInformation.StandardBias.CompareTo(t2.TimeZoneInformation.StandardBias);
			if (num == 0)
			{
				return t1.LocalizableDisplayName.ToString().CompareTo(t2.LocalizableDisplayName.ToString());
			}
			return num;
		}
	}
}
