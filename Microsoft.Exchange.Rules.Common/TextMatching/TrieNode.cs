using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextMatching
{
	internal sealed class TrieNode : StateNode
	{
		public TrieNode(int state) : base(state, false)
		{
		}

		public Dictionary<int, TrieNode> Children
		{
			get
			{
				return this.children;
			}
		}

		public TrieNode Fail
		{
			get
			{
				return this.fail;
			}
			set
			{
				this.fail = value;
			}
		}

		public TransitionTable TransitionTable
		{
			get
			{
				return this.transitionTable;
			}
		}

		public TrieNode Transit(int ch)
		{
			TrieNode result = null;
			if (this.Children.TryGetValue(ch, out result))
			{
				return result;
			}
			if (base.State == 0)
			{
				return this;
			}
			return null;
		}

		public void Compile(DFACodeGenerator codegen)
		{
			this.transitionTable.Compile(this, codegen);
			foreach (TrieNode trieNode in this.children.Values)
			{
				trieNode.Compile(codegen);
			}
		}

		private TrieNode fail;

		private TransitionTable transitionTable = new TransitionTable();

		private Dictionary<int, TrieNode> children = new Dictionary<int, TrieNode>();
	}
}
