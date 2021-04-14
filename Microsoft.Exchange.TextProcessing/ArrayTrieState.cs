using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextProcessing
{
	internal sealed class ArrayTrieState
	{
		public ArrayTrieState(ArrayTrieNode node, int initialCount, int nodeIndex)
		{
			this.node = node;
			this.transitionCount = initialCount;
			this.nodeIndex = nodeIndex;
		}

		private bool Terminal
		{
			get
			{
				return this.node.IsTerminal(this.nodeParam);
			}
		}

		public static void TransitionStates(ArrayTrie trie, char ch, int position, ArrayTrieStatePool pool, List<ArrayTrieState> currentStates, List<ArrayTrieState> newStates, ref SearchResult result)
		{
			foreach (ArrayTrieState arrayTrieState in currentStates)
			{
				if (arrayTrieState.Terminal && StringHelper.IsRightHandSideDelimiter(ch, trie.BoundaryType))
				{
					result.AddResult(arrayTrieState.GetTerminalIds(trie), (long)arrayTrieState.nodeIndex, position - arrayTrieState.transitionCount - 1, position - 1);
				}
				if (arrayTrieState.Transition(trie, ch))
				{
					arrayTrieState.transitionCount++;
					newStates.Add(arrayTrieState);
				}
				else
				{
					pool.FreeActiveState(arrayTrieState);
				}
			}
		}

		public void Reinitialize(ArrayTrieNode node, int initialCount, int nodeIndex)
		{
			this.node = node;
			this.transitionCount = initialCount;
			this.nodeIndex = nodeIndex;
			this.nodeParam = 0;
		}

		private bool Transition(ArrayTrie trie, char ch)
		{
			ArrayTrieNode arrayTrieNode;
			int num;
			if (this.node.Transition(trie, ch, this.nodeParam, out arrayTrieNode, out num, ref this.nodeIndex))
			{
				this.node = arrayTrieNode;
				this.nodeParam = num;
				return true;
			}
			return false;
		}

		private List<long> GetTerminalIds(ArrayTrie trie)
		{
			if (this.node.TerminalIdsCount == 0)
			{
				return null;
			}
			List<long> list = new List<long>((int)this.node.TerminalIdsCount);
			for (int i = 0; i < (int)this.node.TerminalIdsCount; i++)
			{
				list.Add(trie.TerminalIds[this.node.TerminalIdsStart + i]);
			}
			return list;
		}

		private ArrayTrieNode node;

		private int nodeParam;

		private int nodeIndex;

		private int transitionCount;
	}
}
