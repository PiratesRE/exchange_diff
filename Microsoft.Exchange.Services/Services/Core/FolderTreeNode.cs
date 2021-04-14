using System;

namespace Microsoft.Exchange.Services.Core
{
	internal class FolderTreeNode
	{
		public FolderTreeNode(int index, int count)
		{
			this.Index = index;
			this.DescendantCount = count;
		}

		public int Index { get; private set; }

		public int DescendantCount { get; private set; }
	}
}
