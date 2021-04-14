using System;

namespace Microsoft.Exchange.TextProcessing
{
	[Flags]
	internal enum MatchRegexOptions : byte
	{
		None = 0,
		Compiled = 1,
		ExplicitCaptures = 2,
		Cached = 4,
		Primed = 8,
		LazyOptimize = 16,
		CultureInvariant = 32
	}
}
