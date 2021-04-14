using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextProcessing
{
	internal sealed class ArrayTrie
	{
		public ArrayTrie(BoundaryType boundaryType)
		{
			this.RootIndex = 0;
			this.Nodes = new RopeList<ArrayTrieNode>(1024);
			this.TerminalIds = new RopeList<long>(1024);
			this.Keywords = new RopeList<char>(1024);
			this.Edges = new RopeList<ArrayTrieEdge>(1024);
			this.RootChildrenIndexes = new int[65536];
			this.BoundaryType = boundaryType;
			for (int i = 0; i < this.RootChildrenIndexes.Length; i++)
			{
				this.RootChildrenIndexes[i] = -1;
			}
		}

		public int RootIndex { get; set; }

		public RopeList<ArrayTrieNode> Nodes { get; private set; }

		public RopeList<long> TerminalIds { get; private set; }

		public RopeList<char> Keywords { get; private set; }

		public RopeList<ArrayTrieEdge> Edges { get; private set; }

		public int[] RootChildrenIndexes { get; private set; }

		public BoundaryType BoundaryType { get; private set; }

		public void SearchText(string data, SearchResult result)
		{
			if (data == null)
			{
				throw new ArgumentException(Strings.InvalidData);
			}
			string text = StringHelper.NormalizeString(data);
			ArrayTrieStatePool arrayTrieStatePool = new ArrayTrieStatePool();
			List<ArrayTrieState> currentStates = new List<ArrayTrieState>(32);
			List<ArrayTrieState> list = new List<ArrayTrieState>(32);
			bool flag = true;
			for (int i = 0; i < text.Length; i++)
			{
				ArrayTrieState.TransitionStates(this, text[i], i, arrayTrieStatePool, currentStates, list, ref result);
				if (flag)
				{
					int num = this.RootChildrenIndexes[(int)text[i]];
					if (num != -1)
					{
						list.Add(arrayTrieStatePool.GetActiveState(this.Nodes[num], 0, num));
					}
				}
				flag = StringHelper.IsLeftHandSideDelimiter(text[i], this.BoundaryType);
				this.Swap<List<ArrayTrieState>>(ref list, ref currentStates);
				list.Clear();
			}
			ArrayTrieState.TransitionStates(this, ' ', text.Length, arrayTrieStatePool, currentStates, list, ref result);
		}

		private void Swap<T>(ref T first, ref T second)
		{
			T t = first;
			first = second;
			second = t;
		}
	}
}
