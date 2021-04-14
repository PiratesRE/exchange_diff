using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextMatching
{
	internal sealed class MatchAC
	{
		public MatchAC(string[] keywords)
		{
			this.BuildKeyWordsTrie(keywords);
		}

		public PatternMatcher Compile()
		{
			DFACodeGenerator dfacodeGenerator = new DFACodeGenerator("matcher", this.newstate);
			dfacodeGenerator.BeginCompile();
			this.root.Compile(dfacodeGenerator);
			this.root = null;
			this.chars = null;
			return dfacodeGenerator.EndCompile();
		}

		private void BuildKeyWordsTrie(string[] keywords)
		{
			if (keywords == null || keywords.Length == 0)
			{
				throw new ArgumentException(MatchAC.errorMessage);
			}
			foreach (string text in keywords)
			{
				if (!string.IsNullOrEmpty(text))
				{
					this.Enter(text);
				}
			}
			this.ComputeTransitions();
		}

		private void Enter(string keyword)
		{
			TrieNode trieNode = this.root;
			TrieNode trieNode2 = null;
			int num = 0;
			while (trieNode.Children.TryGetValue((int)keyword[num], out trieNode2))
			{
				trieNode = trieNode2;
				num++;
				if (num >= keyword.Length)
				{
					trieNode.IsFinal = true;
					return;
				}
			}
			for (int i = num; i < keyword.Length; i++)
			{
				TrieNode trieNode3 = new TrieNode(this.newstate++);
				trieNode.Children[(int)keyword[i]] = trieNode3;
				this.chars.Add((int)keyword[i]);
				trieNode = trieNode3;
			}
			trieNode.IsFinal = true;
		}

		private void ComputeTransitions()
		{
			Queue<TrieNode> queue = new Queue<TrieNode>();
			using (Dictionary<int, TrieNode>.KeyCollection.Enumerator enumerator = this.root.Children.Keys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int num = enumerator.Current;
					TrieNode trieNode = this.root.Children[num];
					this.root.TransitionTable.Add(num, trieNode);
					queue.Enqueue(trieNode);
					trieNode.Fail = this.root;
				}
				goto IL_13B;
			}
			IL_7A:
			TrieNode trieNode2 = queue.Dequeue();
			foreach (int ch in this.chars)
			{
				TrieNode trieNode3 = trieNode2.Transit(ch);
				if (trieNode3 != null)
				{
					queue.Enqueue(trieNode3);
					trieNode2.TransitionTable.Add(ch, trieNode3);
					TrieNode fail = trieNode2.Fail;
					TrieNode fail2;
					while ((fail2 = fail.Transit(ch)) == null)
					{
						fail = fail.Fail;
					}
					trieNode3.Fail = fail2;
				}
				else
				{
					TrieNode state = trieNode2.Fail.TransitionTable.GetState(ch) ?? this.root;
					trieNode2.TransitionTable.Add(ch, state);
				}
			}
			IL_13B:
			if (queue.Count <= 0)
			{
				return;
			}
			goto IL_7A;
		}

		private static string errorMessage = "no keywords";

		private TrieNode root = new TrieNode(0);

		private int newstate = 1;

		private Set<int> chars = new Set<int>();
	}
}
