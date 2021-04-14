using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Common.Cluster
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Cluster
	{
		public static bool StringIEquals(string str1, string str2)
		{
			return string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
		}
	}
}
