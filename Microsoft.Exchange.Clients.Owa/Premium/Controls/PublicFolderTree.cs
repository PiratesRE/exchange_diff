using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class PublicFolderTree : FolderTree
	{
		private PublicFolderTree(UserContext userContext, FolderTreeNode rootNode, FolderTreeRenderType renderType) : base(userContext, rootNode, renderType)
		{
		}

		protected override void RenderAdditionalProperties(TextWriter writer)
		{
			base.RenderAdditionalProperties(writer);
			writer.Write(" _fPF=1");
		}

		internal static PublicFolderTree CreatePublicFolderRootTree(UserContext userContext)
		{
			PublicFolderTree publicFolderTree = new PublicFolderTree(userContext, FolderTreeNode.CreatePublicFolderTreeRootNode(userContext), FolderTreeRenderType.None);
			publicFolderTree.RootNode.IsExpanded = true;
			publicFolderTree.RootNode.Selected = true;
			FolderTreeNode rootNode = publicFolderTree.RootNode;
			rootNode.HighlightClassName += " trNdGpHdHl";
			return publicFolderTree;
		}
	}
}
