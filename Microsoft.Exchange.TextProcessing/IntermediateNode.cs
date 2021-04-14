using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.TextProcessing
{
	internal class IntermediateNode
	{
		public IntermediateNode()
		{
		}

		public IntermediateNode(string value)
		{
			this.keyword = value;
		}

		public IntermediateNode(char ch)
		{
			if (StringHelper.IsWhitespaceCharacter(ch))
			{
				this.keyword = " ";
			}
		}

		public SortedList<char, IntermediateNode> ChildrenMap
		{
			get
			{
				return this.childrenMap;
			}
		}

		public List<long> TerminalIds
		{
			get
			{
				return this.terminalIds;
			}
		}

		public string Keyword
		{
			get
			{
				return this.keyword;
			}
		}

		public bool IsMultiNode
		{
			get
			{
				return this.keyword.Length > 0 && !this.keyword.Equals(" ");
			}
		}

		public void Reinitialize(string value)
		{
			this.terminalIds.Clear();
			this.childrenMap.Clear();
			this.foldedNodes.Clear();
			this.keyword = value;
		}

		public void Reinitialize(char ch)
		{
			this.Reinitialize(StringHelper.IsWhitespaceCharacter(ch) ? " " : string.Empty);
		}

		public IntermediateNode AddChild(char ch, IntermediateNode child)
		{
			this.childrenMap[ch] = child;
			return child;
		}

		public IntermediateNode GetChild(char ch, IntermediateNodePool pool)
		{
			IntermediateNode child;
			if (this.childrenMap.TryGetValue(ch, out child))
			{
				return this.MakeSimpleChild(ch, child, pool);
			}
			return null;
		}

		public void AddFoldedNode(char ch, int index)
		{
			if (this.foldedNodes.Any((KeyValuePair<char, int> item) => item.Key == ch))
			{
				throw new InvalidOperationException();
			}
			this.foldedNodes.Add(new KeyValuePair<char, int>(ch, index));
		}

		public List<KeyValuePair<char, int>> GetFoldedNodes()
		{
			return this.foldedNodes;
		}

		private IntermediateNode MakeSimpleChild(char ch, IntermediateNode child, IntermediateNodePool pool)
		{
			if (child.IsMultiNode)
			{
				IntermediateNode intermediateNode = pool.Create(ch);
				this.childrenMap[ch] = intermediateNode;
				this.ReconstructOldKeyword(child, intermediateNode, pool);
				return intermediateNode;
			}
			return child;
		}

		private void ReconstructOldKeyword(IntermediateNode original, IntermediateNode replacement, IntermediateNodePool pool)
		{
			IntermediateNode intermediateNode = pool.Create(original.Keyword.Substring(1));
			replacement.AddChild(original.Keyword[0], intermediateNode);
			intermediateNode.TerminalIds.AddRange(original.TerminalIds);
		}

		private List<long> terminalIds = new List<long>();

		private SortedList<char, IntermediateNode> childrenMap = new SortedList<char, IntermediateNode>();

		private string keyword = string.Empty;

		private List<KeyValuePair<char, int>> foldedNodes = new List<KeyValuePair<char, int>>();
	}
}
