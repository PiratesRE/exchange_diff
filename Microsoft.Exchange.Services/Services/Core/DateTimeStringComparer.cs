using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core
{
	internal class DateTimeStringComparer : IComparer<string>
	{
		public int Compare(string x, string y)
		{
			return Convert.ToDateTime(x).CompareTo(Convert.ToDateTime(y));
		}
	}
}
