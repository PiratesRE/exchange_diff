using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Search.Query
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class QuerySuggestion
	{
		internal QuerySuggestion(string suggestedQuery, double weight, QuerySuggestionSources source)
		{
			this.SuggestedQuery = suggestedQuery;
			this.Weight = weight;
			this.Source = source;
		}

		public string SuggestedQuery { get; private set; }

		public double Weight { get; private set; }

		public QuerySuggestionSources Source { get; private set; }
	}
}
