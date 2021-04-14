using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextProcessing
{
	internal class SingleTrieTermMatch : IMatch
	{
		internal SingleTrieTermMatch(IEnumerable<string> terms, BoundaryType boundaryType, TrieInfo trieInfo)
		{
			if (terms == null || !terms.GetEnumerator().MoveNext())
			{
				throw new ArgumentException(Strings.EmptyTermSet);
			}
			this.boundaryType = boundaryType;
			this.terms = new List<string>(terms);
			this.trieInfo = trieInfo;
			this.id = SearchResultEncodedId.GetEncodedId(IDGenerator.GetNextID(), boundaryType);
			foreach (string keyword in this.terms)
			{
				this.trieInfo.Trie.Add(keyword, this.id);
			}
		}

		internal SingleTrieTermMatch(SingleTrieTermMatch original, TrieInfo trieInfo) : this(original.terms, original.boundaryType, trieInfo)
		{
		}

		public bool IsMatch(TextScanContext data)
		{
			this.UpdateContextWithTrieSearchResults(data);
			return data.IsMatchedTermSet(this.id);
		}

		private void UpdateContextWithTrieSearchResults(TextScanContext context)
		{
			if (!context.IsTrieScanComplete(this.trieInfo.ID))
			{
				SearchResultEncodedId searchResultEncodedId = new SearchResultEncodedId(context.NormalizedData, 256);
				this.trieInfo.Trie.SearchText(context.NormalizedData, searchResultEncodedId);
				foreach (long num in searchResultEncodedId.GetFoundIDs())
				{
					context.AddMatchedTermSetID(num);
				}
				context.SetTrieScanComplete(this.trieInfo.ID);
			}
		}

		private readonly long id;

		private IEnumerable<string> terms;

		private TrieInfo trieInfo;

		private BoundaryType boundaryType;
	}
}
