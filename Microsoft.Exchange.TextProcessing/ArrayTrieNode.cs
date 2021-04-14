using System;

namespace Microsoft.Exchange.TextProcessing
{
	internal struct ArrayTrieNode
	{
		public int ChildrenNodesStart { get; set; }

		public ushort ChildrenNodeCount { get; set; }

		public int KeywordStart { get; set; }

		public ushort KeywordLength { get; set; }

		public int TerminalIdsStart { get; set; }

		public ushort TerminalIdsCount { get; set; }

		public ushort Flags { get; set; }

		public bool Transition(ArrayTrie trie, char ch, int param, out ArrayTrieNode newNode, out int newParam, ref int nodeIndex)
		{
			if (StringHelper.IsWhitespaceCharacter(ch))
			{
				ch = ' ';
			}
			if (' ' == ch && (this.IsWhitespaceSpinnerNode(trie) || (param > 0 && ' ' == trie.Keywords[this.KeywordStart + param - 1])))
			{
				newNode = this;
				newParam = param;
				return true;
			}
			if (this.IsWhitespaceSpinnerNode(trie) || param >= (int)this.KeywordLength)
			{
				newNode = default(ArrayTrieNode);
				newParam = 0;
				return this.ChildrenNodeCount > 0 && this.TryFindChild(trie, ch, this.ChildrenNodesStart, this.ChildrenNodesStart + (int)this.ChildrenNodeCount - 1, out newNode, ref nodeIndex);
			}
			if (trie.Keywords[this.KeywordStart + param] == ch)
			{
				newNode = this;
				newParam = param + 1;
				return true;
			}
			newNode = default(ArrayTrieNode);
			newParam = 0;
			return false;
		}

		public bool IsTerminal(int param)
		{
			return (int)this.KeywordLength == param && this.TerminalIdsCount > 0;
		}

		private bool TryFindChild(ArrayTrie trie, char ch, int start, int end, out ArrayTrieNode child, ref int nodeIndex)
		{
			child = default(ArrayTrieNode);
			uint num = (uint)StringHelper.FindMask(ch);
			if ((num & (uint)this.Flags) != num)
			{
				return false;
			}
			if (end < start)
			{
				return false;
			}
			if (end == start)
			{
				if (ch == trie.Edges[start].Character)
				{
					nodeIndex = trie.Edges[start].Index;
					child = trie.Nodes[nodeIndex];
					return true;
				}
				return false;
			}
			else
			{
				int num2 = (start + end) / 2;
				if (ch > trie.Edges[num2].Character)
				{
					return this.TryFindChild(trie, ch, num2 + 1, end, out child, ref nodeIndex);
				}
				if (ch < trie.Edges[num2].Character)
				{
					return this.TryFindChild(trie, ch, start, num2 - 1, out child, ref nodeIndex);
				}
				nodeIndex = trie.Edges[num2].Index;
				child = trie.Nodes[nodeIndex];
				return true;
			}
		}

		private bool IsWhitespaceSpinnerNode(ArrayTrie trie)
		{
			return this.KeywordLength == 1 && trie.Keywords[this.KeywordStart].Equals(' ');
		}
	}
}
