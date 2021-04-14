using System;

namespace Microsoft.Exchange.Transport.Categorizer
{
	[Flags]
	internal enum RouteComparison
	{
		None = 0,
		CompareNames = 1,
		CompareRestrictions = 2,
		IgnoreRGCosts = 4
	}
}
