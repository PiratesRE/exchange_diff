using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class InstantMessageCategoryVersion
	{
		public InstantMessageCategoryVersion(int categoryIndex, int categoryVersion)
		{
			this.categoryIndex = categoryIndex;
			this.categoryVersion = categoryVersion;
		}

		public int CategoryIndex
		{
			get
			{
				return this.categoryIndex;
			}
		}

		public int CategoryVersion
		{
			get
			{
				return this.categoryVersion;
			}
		}

		private int categoryIndex;

		private int categoryVersion;
	}
}
