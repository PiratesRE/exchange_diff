using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal abstract class FolderTree : Tree
	{
		internal FolderTree(UserContext userContext, FolderTreeNode rootNode, FolderTreeRenderType renderType) : base(userContext, rootNode)
		{
			this.renderType = renderType;
		}

		protected override void RenderAdditionalProperties(TextWriter writer)
		{
			base.RenderAdditionalProperties(writer);
			writer.Write(" _frt=");
			writer.Write((int)this.renderType);
		}

		internal new FolderTreeNode RootNode
		{
			get
			{
				return (FolderTreeNode)base.RootNode;
			}
		}

		private readonly FolderTreeRenderType renderType;
	}
}
