using System;

namespace Microsoft.Exchange.Server.Storage.MultiMailboxSearch
{
	internal sealed class KeywordStatsResultRow
	{
		internal KeywordStatsResultRow(string keyword, long count, double size)
		{
			this.count = count;
			this.keyword = keyword;
			this.size = size;
		}

		internal string Keyword
		{
			get
			{
				return this.keyword;
			}
		}

		internal long Count
		{
			get
			{
				return this.count;
			}
		}

		internal double Size
		{
			get
			{
				return this.size;
			}
		}

		private readonly string keyword;

		private readonly long count;

		private readonly double size;
	}
}
