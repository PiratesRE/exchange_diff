using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class InvisibleRootTreeNode : TreeNode
	{
		internal InvisibleRootTreeNode(UserContext userContext) : base(userContext)
		{
			base.ChildIndent = 0;
			base.IsExpanded = true;
			this.id = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
		}

		public override string Id
		{
			get
			{
				return this.id;
			}
		}

		protected override bool ContentVisible
		{
			get
			{
				return false;
			}
		}

		private readonly string id;
	}
}
