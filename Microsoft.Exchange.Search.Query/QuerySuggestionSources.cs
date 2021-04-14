using System;

namespace Microsoft.Exchange.Search.Query
{
	[Flags]
	public enum QuerySuggestionSources
	{
		None = 0,
		RecentSearches = 1,
		Spelling = 2,
		Synonyms = 4,
		Nicknames = 8,
		TopN = 16,
		Fuzzy = 26,
		All = 31
	}
}
