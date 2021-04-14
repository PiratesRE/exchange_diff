using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextMatching
{
	internal class TransitionTable
	{
		public void Add(int ch, TrieNode state)
		{
			this.table.Add(ch, state);
		}

		public TrieNode GetState(int ch)
		{
			TrieNode result = null;
			if (this.table.TryGetValue(ch, out result))
			{
				return result;
			}
			return null;
		}

		public void Compile(StateNode node, DFACodeGenerator codegen)
		{
			foreach (int num in this.table.Keys)
			{
				codegen.AddTransition(node, this.table[num], num);
			}
		}

		private Dictionary<int, TrieNode> table = new Dictionary<int, TrieNode>();
	}
}
