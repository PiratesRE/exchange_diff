using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextProcessing
{
	internal class TermMatch : IMatch
	{
		internal TermMatch(IEnumerable<string> terms, BoundaryType boundaryType, Dictionary<BoundaryType, TrieInfo> trieMap)
		{
			if (this.IsEmpty(terms))
			{
				throw new ArgumentException(Strings.EmptyTermSet);
			}
			this.boundaryType = boundaryType;
			this.terms = new List<string>(terms);
			this.trieInfo = trieMap[boundaryType];
			this.id = IDGenerator.GetNextID();
			this.AddTermsToTrie();
		}

		internal TermMatch(TermMatch original, Dictionary<BoundaryType, TrieInfo> trieMap) : this(original.terms, original.boundaryType, trieMap)
		{
		}

		public bool IsMatch(TextScanContext data)
		{
			this.UpdateContextWithTrieSearchResults(data);
			return data.IsMatchedTermSet(this.id);
		}

		private void AddTermsToTrie()
		{
			foreach (string keyword in this.terms)
			{
				this.trieInfo.Trie.Add(keyword, this.id);
			}
		}

		private bool IsEmpty(IEnumerable<string> collection)
		{
			return collection == null || !collection.GetEnumerator().MoveNext();
		}

		private void UpdateContextWithTrieSearchResults(TextScanContext context)
		{
			if (!context.IsTrieScanComplete(this.trieInfo.ID))
			{
				SearchResult searchResult = this.trieInfo.Trie.SearchText(context.Data);
				foreach (long num in searchResult.GetFoundIDs())
				{
					context.AddMatchedTermSetID(num);
				}
				context.SetTrieScanComplete(this.trieInfo.ID);
			}
		}

		private readonly long id;

		private IEnumerable<string> terms;

		private BoundaryType boundaryType;

		private TrieInfo trieInfo;
	}
}
