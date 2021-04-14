using System;

namespace Microsoft.Exchange.Data
{
	[Flags]
	internal enum MatchFlags
	{
		Default = 0,
		IgnoreCase = 1,
		IgnoreNonSpace = 2,
		Loose = 4
	}
}
