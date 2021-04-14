using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.Deployment.XforestTenantMigration
{
	internal sealed class Node
	{
		public string Name { get; set; }

		public DirectoryObject Value { get; set; }

		public Dictionary<string, Node> Children { get; set; }

		public Node(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(name);
			}
			this.Name = name;
			this.Children = new Dictionary<string, Node>();
			this.Value = null;
		}

		public Node()
		{
		}
	}
}
