using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class NavigationGroupHeaderTreeNode : TreeNode
	{
		internal NavigationGroupHeaderTreeNode(UserContext userContext, NavigationNodeGroup group) : base(userContext)
		{
			this.group = group;
			if (group.NavigationNodeId != null)
			{
				this.id = "f" + group.NavigationNodeId.ObjectId.ToString();
			}
			else
			{
				this.id = "f" + Convert.ToBase64String(Guid.NewGuid().ToByteArray());
			}
			base.IsExpanded = group.IsExpanded;
			base.HighlightClassName += " trNdGpHdHl";
			base.NodeClassName += " trNdGpHd";
			base.IsRootNode = true;
			if (group.NavigationNodeGroupSection != NavigationNodeGroupSection.First)
			{
				base.ChildIndent = 0;
			}
		}

		protected override void RenderAdditionalProperties(TextWriter writer)
		{
			writer.Write(" _t=\"navigationGroupHeaderNode\"");
			base.RenderAdditionalProperties(writer);
		}

		protected override void RenderContent(TextWriter writer)
		{
			Utilities.HtmlEncode(this.group.Subject, writer, true);
		}

		public override string Id
		{
			get
			{
				return this.id;
			}
		}

		internal override bool Selectable
		{
			get
			{
				return false;
			}
		}

		private readonly NavigationNodeGroup group;

		private string id;
	}
}
