using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextProcessing
{
	internal abstract class SearchResult
	{
		public abstract int HitCount { get; }

		public abstract IEnumerable<Offset> GetOffsetsForID(long id);

		public abstract IEnumerable<long> GetFoundIDs();

		public abstract bool HasId(long id);

		internal static SearchResult Create(bool storeOffsets)
		{
			if (storeOffsets)
			{
				return new SearchResultWithOffsets();
			}
			return new SearchResultWithoutOffsets(256);
		}

		internal abstract void AddResult(List<long> ids, long nodeId, int start, int end);
	}
}
