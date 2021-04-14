using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextMatching
{
	internal sealed class RegexCharSet
	{
		public RegexCharSet()
		{
			this.dict = new Dictionary<RegexCharacterClass, List<RegexTreeNode>>();
		}

		public List<RegexTreeNode> this[RegexCharacterClass cl]
		{
			get
			{
				return this.dict[cl];
			}
		}

		public void Add(RegexCharacterClass cl, RegexTreeNode node)
		{
			List<RegexTreeNode> list;
			if (!this.dict.ContainsKey(cl))
			{
				list = new List<RegexTreeNode>();
				this.dict.Add(cl, list);
			}
			else
			{
				list = this.dict[cl];
			}
			list.Add(node);
		}

		public IEnumerable<RegexCharacterClass> Chars()
		{
			foreach (RegexCharacterClass cl in this.dict.Keys)
			{
				yield return cl;
			}
			yield break;
		}

		private Dictionary<RegexCharacterClass, List<RegexTreeNode>> dict;
	}
}
