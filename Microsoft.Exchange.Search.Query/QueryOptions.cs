using System;

namespace Microsoft.Exchange.Search.Query
{
	[Flags]
	internal enum QueryOptions
	{
		None = 0,
		Suggestions = 1,
		SuggestionsPrimer = 2,
		Results = 4,
		Refiners = 8,
		SearchTerms = 16,
		ExplicitSearch = 32,
		AllowFuzzing = 64
	}
}
