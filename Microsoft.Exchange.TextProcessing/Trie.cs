using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.TextProcessing;

namespace Microsoft.Exchange.TextProcessing
{
	internal class Trie
	{
		public Trie(BoundaryType boundaryType = BoundaryType.Normal, bool storeOffsets = true)
		{
			this.boundaryType = boundaryType;
			this.storeOffsets = storeOffsets;
			this.nodeCollection.Add(TrieNode.Default);
		}

		public void Add(string keyword, long id)
		{
			if (string.IsNullOrEmpty(keyword))
			{
				throw new ArgumentException(Strings.InvalidTerm);
			}
			string text = StringHelper.NormalizeKeyword(keyword);
			this.AddNormalizedKeyword((text == keyword) ? keyword : text, id);
		}

		public SearchResult SearchText(string data)
		{
			if (data == null)
			{
				throw new ArgumentException(Strings.InvalidData);
			}
			SearchResult result = SearchResult.Create(this.storeOffsets);
			this.SearchNormalizedText(StringHelper.NormalizeString(data), result);
			return result;
		}

		public void SearchText(string data, SearchResult result)
		{
			if (data == null)
			{
				throw new ArgumentException(Strings.InvalidData);
			}
			this.SearchNormalizedText(StringHelper.NormalizeString(data), result);
		}

		private void SearchNormalizedText(string data, SearchResult result)
		{
			if (this.childrenNodes == null)
			{
				return;
			}
			ActiveStatePool activeStatePool = new ActiveStatePool();
			List<ActiveState> currentStates = new List<ActiveState>(32);
			List<ActiveState> list = new List<ActiveState>(32);
			bool flag = true;
			for (int i = 0; i < data.Length; i++)
			{
				char c = data[i];
				ActiveState.TransitionStates(c, i, this.boundaryType, this.nodeCollection, activeStatePool, currentStates, list, ref result);
				if (flag)
				{
					int num = this.childrenNodes[(int)c];
					if (num != 0)
					{
						list.Add(activeStatePool.GetActiveState(this.nodeCollection[num], num, 1));
					}
				}
				flag = StringHelper.IsLeftHandSideDelimiter(c, this.boundaryType);
				this.Swap<List<ActiveState>>(ref currentStates, ref list);
				list.Clear();
			}
			ActiveState.TransitionStates(' ', data.Length, this.boundaryType, this.nodeCollection, activeStatePool, currentStates, list, ref result);
		}

		private void AddNormalizedKeyword(string keyword, long id)
		{
			if (this.childrenNodes == null)
			{
				this.childrenNodes = new int[65536];
			}
			int num = this.AddToRoot(keyword, id);
			if (num != 0)
			{
				int i = 1;
				TrieNode value = this.nodeCollection[num];
				while (i < keyword.Length)
				{
					if (value.IsMultiNode())
					{
						ExTraceGlobals.SmartTrieTracer.TraceDebug<string, int>((long)this.GetHashCode(), "Current trie node is a multinode which is unexpected. Adding word '{0}' and at index '{1}'", keyword, i);
						throw new InvalidOperationException(Strings.CurrentNodeNotSingleNode);
					}
					int child = value.GetChild(keyword[i], this.nodeCollection);
					if (child == 0)
					{
						TrieNode item = new TrieNode(keyword, i + 1);
						item.AddTerminalId(id);
						this.nodeCollection.Add(item);
						value.AddChild(keyword[i], this.nodeCollection.Count - 1);
						this.nodeCollection[num] = value;
						return;
					}
					value = this.nodeCollection[child];
					num = child;
					if (value.IsMultiNode())
					{
						TrieNode item2 = new TrieNode(value.Keyword, (int)(value.StringIndex + 1));
						item2.AddTerminalIds(value.TerminalIDs);
						this.nodeCollection.Add(item2);
						TrieNode value2 = new TrieNode(keyword[i]);
						value2.AddChild(value.Keyword[i + 1], this.nodeCollection.Count - 1);
						this.nodeCollection[child] = value2;
					}
					value = this.nodeCollection[child];
					if (i + 1 == keyword.Length)
					{
						value.AddTerminalId(id);
						this.nodeCollection[child] = value;
					}
					i++;
				}
			}
		}

		private int AddToRoot(string keyword, long id)
		{
			char c = keyword[0];
			int num = this.childrenNodes[(int)c];
			if (num == 0)
			{
				TrieNode item = new TrieNode(keyword, 1);
				item.AddTerminalId(id);
				this.nodeCollection.Add(item);
				this.childrenNodes[(int)c] = this.nodeCollection.Count - 1;
				return 0;
			}
			TrieNode trieNode = this.nodeCollection[num];
			if (trieNode.IsMultiNode())
			{
				TrieNode item2 = new TrieNode(c);
				item2.AddChild(trieNode.Keyword[(int)trieNode.StringIndex], num);
				this.nodeCollection.Add(item2);
				TrieNode value = new TrieNode(trieNode.Keyword, (int)(trieNode.StringIndex + 1));
				value.AddTerminalIds(trieNode.TerminalIDs);
				this.nodeCollection[num] = value;
				int num2 = this.nodeCollection.Count - 1;
				this.childrenNodes[(int)c] = num2;
				return num2;
			}
			return num;
		}

		private void Swap<T>(ref T first, ref T second)
		{
			T t = first;
			first = second;
			second = t;
		}

		private readonly bool storeOffsets;

		private int[] childrenNodes;

		private BoundaryType boundaryType;

		private RopeList<TrieNode> nodeCollection = new RopeList<TrieNode>(1024);
	}
}
