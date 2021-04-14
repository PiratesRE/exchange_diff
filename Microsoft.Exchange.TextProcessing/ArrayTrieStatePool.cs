using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextProcessing
{
	internal sealed class ArrayTrieStatePool
	{
		public void FreeActiveState(ArrayTrieState state)
		{
			this.pool.Add(state);
		}

		public ArrayTrieState GetActiveState(ArrayTrieNode node, int initialCount, int nodeIndex)
		{
			if (this.pool.Count > 0)
			{
				ArrayTrieState arrayTrieState = this.pool[this.pool.Count - 1];
				this.pool.RemoveAt(this.pool.Count - 1);
				arrayTrieState.Reinitialize(node, initialCount, nodeIndex);
				return arrayTrieState;
			}
			return new ArrayTrieState(node, initialCount, nodeIndex);
		}

		private List<ArrayTrieState> pool = new List<ArrayTrieState>(16);
	}
}
