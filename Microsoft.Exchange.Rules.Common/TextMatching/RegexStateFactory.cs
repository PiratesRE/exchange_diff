using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextMatching
{
	internal sealed class RegexStateFactory
	{
		public RegexState CreateRegexState()
		{
			return new RegexState(this.stateid++);
		}

		public void Destory()
		{
			this.stateid--;
		}

		public RegexState CreateRegexState(List<RegexTreeNode> nodes)
		{
			return new RegexState(this.stateid++, nodes);
		}

		private int stateid;
	}
}
