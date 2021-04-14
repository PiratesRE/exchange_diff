using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextProcessing
{
	internal sealed class ActiveState
	{
		public ActiveState(TrieNode node, int nodeIndex, int initialCount)
		{
			this.Initialize(node, nodeIndex, initialCount);
		}

		private bool Terminal
		{
			get
			{
				return this.isTerminal;
			}
		}

		private int NodeId
		{
			get
			{
				return this.nodeId;
			}
		}

		private int TransitionCount
		{
			get
			{
				return this.transitionCount;
			}
		}

		private List<long> IDs
		{
			get
			{
				return this.terminalIDs;
			}
		}

		public static void TransitionStates(char ch, int position, BoundaryType boundaryType, RopeList<TrieNode> nodes, ActiveStatePool activeStatePool, List<ActiveState> currentStates, List<ActiveState> newStates, ref SearchResult result)
		{
			foreach (ActiveState activeState in currentStates)
			{
				if (activeState.Terminal && StringHelper.IsRightHandSideDelimiter(ch, boundaryType))
				{
					result.AddResult(activeState.IDs, (long)activeState.NodeId, position - activeState.TransitionCount, position - 1);
				}
				if (activeState.Transition(ch, nodes))
				{
					activeState.IncrementTransitionCount();
					newStates.Add(activeState);
				}
				else
				{
					activeStatePool.FreeActiveState(activeState);
				}
			}
		}

		public void Reinitialize(TrieNode node, int nodeIndex, int initialCount)
		{
			this.Initialize(node, nodeIndex, initialCount);
		}

		private bool Transition(char ch, RopeList<TrieNode> nodes)
		{
			if (nodes[this.nodeId].Transition(ch, this.nodePosition, nodes, ref this.nodeId, ref this.nodePosition))
			{
				this.isTerminal = nodes[this.nodeId].IsTerminal(this.nodePosition);
				this.terminalIDs = nodes[this.nodeId].TerminalIDs;
				return true;
			}
			return false;
		}

		private void IncrementTransitionCount()
		{
			this.transitionCount++;
		}

		private void Initialize(TrieNode node, int nodeIndex, int initialCount)
		{
			this.nodePosition = 0;
			this.transitionCount = initialCount;
			this.nodeId = nodeIndex;
			this.terminalIDs = node.TerminalIDs;
			this.isTerminal = node.IsTerminal(this.nodePosition);
		}

		private int nodePosition;

		private int nodeId;

		private int transitionCount;

		private bool isTerminal;

		private List<long> terminalIDs;
	}
}
