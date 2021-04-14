using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextProcessing
{
	internal class SearchResultWithOffsets : SearchResult
	{
		internal SearchResultWithOffsets()
		{
			this.results = new Dictionary<long, List<Offset>>();
		}

		public override int HitCount
		{
			get
			{
				return this.results.Count;
			}
		}

		public override IEnumerable<Offset> GetOffsetsForID(long id)
		{
			return this.results[id];
		}

		public override IEnumerable<long> GetFoundIDs()
		{
			return this.results.Keys;
		}

		public override bool HasId(long id)
		{
			return this.results.ContainsKey(id);
		}

		internal override void AddResult(List<long> ids, long nodeId, int start, int end)
		{
			Offset item = new Offset(start, end);
			foreach (long key in ids)
			{
				if (!this.results.ContainsKey(key))
				{
					this.results[key] = new List<Offset>();
				}
				this.results[key].Add(item);
			}
		}

		private Dictionary<long, List<Offset>> results;
	}
}
