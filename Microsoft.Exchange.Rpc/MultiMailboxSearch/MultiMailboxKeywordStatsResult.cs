using System;

namespace Microsoft.Exchange.Rpc.MultiMailboxSearch
{
	[Serializable]
	internal sealed class MultiMailboxKeywordStatsResult : MultiMailboxSearchResultItem
	{
		internal MultiMailboxKeywordStatsResult(int version, string keyword, long count, long size) : base(version)
		{
			this.count = count;
			this.keyword = keyword;
			this.size = size;
		}

		internal MultiMailboxKeywordStatsResult(string keyword, long count, long size) : base(MultiMailboxSearchBase.CurrentVersion)
		{
			this.count = count;
			this.keyword = keyword;
			this.size = size;
		}

		internal long Count
		{
			get
			{
				return this.count;
			}
		}

		internal string Keyword
		{
			get
			{
				return this.keyword;
			}
		}

		internal long Size
		{
			get
			{
				return this.size;
			}
		}

		private long count;

		private string keyword;

		private long size;
	}
}
