using System;

namespace Microsoft.Exchange.TextProcessing
{
	internal sealed class KeywordEntry
	{
		public KeywordEntry(string keyword, long identifier)
		{
			this.Keyword = keyword;
			this.Identifier = identifier;
		}

		public string Keyword { get; set; }

		public long Identifier { get; set; }
	}
}
