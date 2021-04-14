using System;

namespace Microsoft.Exchange.TextProcessing
{
	internal class TrieInfo
	{
		public TrieInfo(long id, Trie trie)
		{
			this.id = id;
			this.trie = trie;
		}

		public Trie Trie
		{
			get
			{
				return this.trie;
			}
		}

		public long ID
		{
			get
			{
				return this.id;
			}
		}

		private readonly long id;

		private Trie trie;
	}
}
