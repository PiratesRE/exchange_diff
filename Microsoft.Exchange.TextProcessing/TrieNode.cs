using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextProcessing
{
	internal struct TrieNode
	{
		public TrieNode(string value, int stringIndex = 0)
		{
			if (stringIndex > 32767)
			{
				throw new ArgumentException(Strings.TermExceedsMaximumLength);
			}
			this.keyword = value;
			this.stringIndex = (short)stringIndex;
			this.flags = 0;
			this.childrenMap = null;
			this.idList = null;
		}

		public TrieNode(char ch)
		{
			this = new TrieNode(string.Empty, 0);
			if (StringHelper.IsWhitespaceCharacter(ch))
			{
				this.keyword = " ";
				this.flags |= TrieNode.WhitespaceSpinnerFlag;
			}
		}

		public short StringIndex
		{
			get
			{
				return this.stringIndex;
			}
		}

		public List<long> TerminalIDs
		{
			get
			{
				return this.idList;
			}
		}

		public string Keyword
		{
			get
			{
				return this.keyword;
			}
		}

		private List<long> AllocatedIdList
		{
			get
			{
				if (this.idList == null)
				{
					this.idList = new List<long>();
				}
				return this.idList;
			}
		}

		public bool Transition(char ch, int position, RopeList<TrieNode> nodes, ref int nodeId, ref int newPosition)
		{
			if (StringHelper.IsWhitespaceCharacter(ch))
			{
				ch = ' ';
			}
			if (' ' == ch && (this.IsWhitespaceSpinnerNode() || (position > 0 && ' ' == this.keyword[position + (int)this.stringIndex - 1])))
			{
				newPosition = position;
				return true;
			}
			if (this.IsWhitespaceSpinnerNode() || position + (int)this.stringIndex >= this.keyword.Length)
			{
				if (this.childrenMap != null)
				{
					uint num = (uint)StringHelper.FindMask(ch);
					if ((num & (uint)this.flags) == num)
					{
						newPosition = 0;
						this.childrenMap.TryGetValue(ch, out nodeId);
						return nodeId != 0;
					}
				}
			}
			else if (this.keyword[position + (int)this.stringIndex] == ch)
			{
				newPosition = position + 1;
				return true;
			}
			return false;
		}

		public void AddTerminalId(long id)
		{
			this.flags |= TrieNode.TerminalFlag;
			this.AllocatedIdList.Add(id);
		}

		public void AddTerminalIds(List<long> addedIds)
		{
			this.flags |= TrieNode.TerminalFlag;
			this.AllocatedIdList.AddRange(addedIds);
		}

		public bool IsTerminal(int position)
		{
			return (this.flags & TrieNode.TerminalFlag) != 0 && this.keyword.Length == position + (int)this.stringIndex;
		}

		public int AddChild(char ch, int childIndex)
		{
			if (this.childrenMap == null)
			{
				this.childrenMap = new SortedList<char, int>();
			}
			this.childrenMap[ch] = childIndex;
			this.flags |= StringHelper.FindMask(ch);
			return childIndex;
		}

		public int GetChild(char ch, RopeList<TrieNode> nodes)
		{
			int result;
			if (this.childrenMap == null || !this.childrenMap.TryGetValue(ch, out result))
			{
				return 0;
			}
			return result;
		}

		public bool IsMultiNode()
		{
			return this.keyword.Length - (int)this.stringIndex > 0 && !this.IsWhitespaceSpinnerNode();
		}

		private bool IsWhitespaceSpinnerNode()
		{
			return (this.flags & TrieNode.WhitespaceSpinnerFlag) == TrieNode.WhitespaceSpinnerFlag;
		}

		public static readonly TrieNode Default = new TrieNode("Default", 0);

		private static readonly ushort WhitespaceSpinnerFlag = 1;

		private static readonly ushort TerminalFlag = 2;

		private readonly string keyword;

		private List<long> idList;

		private SortedList<char, int> childrenMap;

		private short stringIndex;

		private ushort flags;
	}
}
