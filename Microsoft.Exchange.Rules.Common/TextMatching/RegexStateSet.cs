using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextMatching
{
	internal sealed class RegexStateSet
	{
		public RegexStateSet()
		{
			this.list = new List<RegexState>();
		}

		public IEnumerable<RegexState> UnmarkedStates()
		{
			for (int counter = 0; counter < this.list.Count; counter++)
			{
				if (!this.list[counter].Marked)
				{
					yield return this.list[counter];
				}
			}
			yield break;
		}

		public void Add(RegexState state)
		{
			this.list.Add(state);
		}

		public RegexState MatchingState(RegexState state)
		{
			for (int i = 0; i < this.list.Count; i++)
			{
				if (this.list[i].Equals(state))
				{
					return this.list[i];
				}
			}
			return null;
		}

		public bool Contains(RegexState state)
		{
			for (int i = 0; i < this.list.Count; i++)
			{
				if (this.list[i].Equals(state))
				{
					return true;
				}
			}
			return false;
		}

		private List<RegexState> list;
	}
}
