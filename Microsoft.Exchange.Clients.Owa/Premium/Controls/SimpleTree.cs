using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class SimpleTree : Tree
	{
		internal SimpleTree(UserContext userContext, TreeNode rootNode, string id) : base(userContext, rootNode)
		{
			this.id = id;
		}

		internal SimpleTree(UserContext userContext, TreeNode rootNode) : this(userContext, rootNode, null)
		{
		}

		internal override string Id
		{
			get
			{
				if (this.id == null)
				{
					return base.Id;
				}
				return this.id;
			}
		}

		private string id;
	}
}
