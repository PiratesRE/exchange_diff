using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextProcessing
{
	internal class SearchResultWithoutOffsets : SearchResult
	{
		internal SearchResultWithoutOffsets(int resultCapacity = 256)
		{
			this.results = new LowAllocSet(resultCapacity);
			this.terminalNodes = new LowAllocSet(resultCapacity);
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
			throw new InvalidOperationException(Strings.OffsetsUnavailable);
		}

		public override IEnumerable<long> GetFoundIDs()
		{
			return this.results.Values;
		}

		public override bool HasId(long id)
		{
			return this.results.Contains(id);
		}

		internal override void AddResult(List<long> ids, long nodeId, int start, int end)
		{
			if (!this.terminalNodes.Contains(nodeId))
			{
				this.terminalNodes.Add(nodeId);
				foreach (long value in ids)
				{
					this.results.Add(value);
				}
			}
		}

		private LowAllocSet results;

		private LowAllocSet terminalNodes;
	}
}
