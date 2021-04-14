using System;

namespace Microsoft.Exchange.Common
{
	internal static class Logic
	{
		public static bool Implies(bool a, bool b)
		{
			return (a && b) || !a;
		}
	}
}
