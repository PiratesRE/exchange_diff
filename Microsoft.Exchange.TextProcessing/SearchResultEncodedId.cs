using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextProcessing
{
	internal class SearchResultEncodedId : SearchResult
	{
		internal SearchResultEncodedId(string text, int resultCapacity = 256)
		{
			this.results = new LowAllocSet(resultCapacity);
			this.terminalNodes = new LowAllocSet(resultCapacity);
			this.text = text;
			this.textLength = text.Length;
		}

		public override int HitCount
		{
			get
			{
				return this.results.Count;
			}
		}

		public static long GetEncodedId(long id, BoundaryType type)
		{
			return id * 10L + (long)type;
		}

		public static BoundaryType GetBoundaryType(long id)
		{
			return (BoundaryType)(id % 10L);
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
				bool flag = true;
				foreach (long num in ids)
				{
					BoundaryType boundaryType = SearchResultEncodedId.GetBoundaryType(num);
					bool flag2 = start <= 0 || boundaryType == BoundaryType.None || StringHelper.IsLeftHandSideDelimiter(this.text[start - 1], boundaryType);
					bool flag3 = end >= this.textLength - 1 || boundaryType == BoundaryType.None || StringHelper.IsRightHandSideDelimiter(this.text[end + 1], boundaryType);
					if (flag2 && flag3)
					{
						this.results.Add(num);
					}
					else
					{
						flag = false;
					}
				}
				if (flag)
				{
					this.terminalNodes.Add(nodeId);
				}
			}
		}

		private readonly string text;

		private readonly int textLength;

		private LowAllocSet results;

		private LowAllocSet terminalNodes;
	}
}
