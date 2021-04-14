using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextProcessing
{
	internal sealed class ActiveStatePool
	{
		public void FreeActiveState(ActiveState state)
		{
			this.freeActiveStates.Add(state);
		}

		public ActiveState GetActiveState(TrieNode node, int nodeIndex, int initialCount)
		{
			if (this.freeActiveStates.Count > 0)
			{
				ActiveState activeState = this.freeActiveStates[this.freeActiveStates.Count - 1];
				this.freeActiveStates.RemoveAt(this.freeActiveStates.Count - 1);
				activeState.Reinitialize(node, nodeIndex, initialCount);
				return activeState;
			}
			return new ActiveState(node, nodeIndex, initialCount);
		}

		private List<ActiveState> freeActiveStates = new List<ActiveState>(16);
	}
}
