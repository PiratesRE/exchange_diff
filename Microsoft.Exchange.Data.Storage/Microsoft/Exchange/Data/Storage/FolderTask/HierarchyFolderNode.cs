using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Storage.FolderTask
{
	public sealed class HierarchyFolderNode
	{
		public HierarchyFolderNode(string path)
		{
			this.Path = path;
			this.ChildNodes = new Dictionary<string, HierarchyFolderNode>();
		}

		public string Path { get; set; }

		public ulong TotalItemSize { get; set; }

		public ulong AggregateTotalItemSize { get; set; }

		public HierarchyFolderNode Parent { get; set; }

		public Dictionary<string, HierarchyFolderNode> ChildNodes { get; set; }
	}
}
