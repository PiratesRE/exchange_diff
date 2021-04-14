using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal abstract class TreeNode
	{
		protected TreeNode(UserContext userContext)
		{
			this.UserContext = userContext;
			this.ChildIndent = 8;
		}

		private protected UserContext UserContext { protected get; private set; }

		public abstract string Id { get; }

		internal List<TreeNode> Children
		{
			get
			{
				return this.children;
			}
		}

		internal int ChildIndent { get; set; }

		protected virtual bool ContentVisible
		{
			get
			{
				return true;
			}
		}

		internal virtual bool Selectable
		{
			get
			{
				return true;
			}
		}

		protected virtual bool Visible
		{
			get
			{
				return true;
			}
		}

		internal virtual bool HasChildren
		{
			get
			{
				return this.Children.Count > 0;
			}
		}

		internal TreeNode Parent
		{
			get
			{
				return this.parent;
			}
			private set
			{
				this.parent = value;
			}
		}

		internal bool Selected
		{
			get
			{
				return this.selected;
			}
			set
			{
				this.selected = value;
			}
		}

		internal string HighlightClassName
		{
			get
			{
				return this.highlightClassName;
			}
			set
			{
				this.highlightClassName = value;
			}
		}

		internal string NodeClassName
		{
			get
			{
				return this.nodeClassName;
			}
			set
			{
				this.nodeClassName = value;
			}
		}

		internal bool IsDummy
		{
			get
			{
				return this.isDummy;
			}
			set
			{
				this.isDummy = value;
			}
		}

		protected ThemeFileId GetECIcon
		{
			get
			{
				ThemeFileId result = ThemeFileId.Clear;
				if (this.HasChildren)
				{
					if (this.IsExpanded)
					{
						if (this.UserContext.IsRtl)
						{
							result = ThemeFileId.MinusRTL;
						}
						else
						{
							result = ThemeFileId.Minus;
						}
					}
					else if (this.UserContext.IsRtl)
					{
						result = ThemeFileId.PlusRTL;
					}
					else
					{
						result = ThemeFileId.Plus;
					}
				}
				return result;
			}
		}

		protected virtual bool HasIcon
		{
			get
			{
				return this.hasIcon;
			}
			set
			{
				this.hasIcon = value;
			}
		}

		internal bool NeedSync
		{
			get
			{
				bool? flag = this.needSync;
				if (flag == null)
				{
					return this.HasChildren && this.Children.Count == 0;
				}
				return flag.GetValueOrDefault();
			}
			set
			{
				this.needSync = new bool?(value);
			}
		}

		internal bool IsExpanded
		{
			get
			{
				return this.isExpanded;
			}
			set
			{
				this.isExpanded = value;
			}
		}

		internal bool IsRootNode
		{
			get
			{
				return this.isRootNode;
			}
			set
			{
				this.isRootNode = value;
			}
		}

		internal string CustomAttributes
		{
			get
			{
				return this.customAttributes;
			}
			set
			{
				this.customAttributes = value;
			}
		}

		internal static void WrapTreeNodeStart(TextWriter writer, UserContext userContext, string nodeID, string highlightClassName, bool contentVisibility, bool hasChildren, bool selected, bool isRootNode, int indent, ThemeFileId expandCollapseIcon, bool isRtl)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			writer.Write("<div id=\"divTrNdO\"");
			if (isRootNode)
			{
				writer.Write(" root=\"1\"");
			}
			writer.Write(">");
			writer.Write("<div id=\"divTrNdHl\" class=\"");
			writer.Write(highlightClassName);
			writer.Write("\"");
			if (!contentVisibility)
			{
				writer.Write("style=\"display:none\"");
			}
			writer.Write(">");
			if (hasChildren && contentVisibility)
			{
				userContext.RenderThemeImage(writer, expandCollapseIcon, null, new object[]
				{
					"id=ec",
					string.Format("style=\"{0}:{1}px\"", isRtl ? "right" : "left", indent)
				});
			}
		}

		internal static void WrapTreeNodeEndToChild(TextWriter writer, string nodeID, bool hasChildren, bool displayChildren, bool isRootNode)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("</div>");
			if (hasChildren)
			{
				writer.Write("<div id=\"divTrNdCC\"");
				if (!displayChildren)
				{
					writer.Write(" style=\"display:none;\"");
				}
				else if (isRootNode)
				{
					writer.Write("style=\"padding-bottom:9px;\"");
				}
				writer.Write(">");
			}
		}

		internal static void WrapTreeNodeEnd(TextWriter writer, bool hasChildren)
		{
			if (hasChildren)
			{
				writer.Write("</div>");
			}
			writer.Write("</div>");
		}

		internal void AddChild(TreeNode child)
		{
			this.Children.Add(child);
			child.Parent = this;
		}

		internal bool SelectSpecifiedFolder(OwaStoreObjectId folderId)
		{
			foreach (TreeNode treeNode in this.Children)
			{
				FolderTreeNode folderTreeNode = treeNode as FolderTreeNode;
				if (folderTreeNode != null && folderId.Equals(folderTreeNode.FolderId))
				{
					folderTreeNode.Selected = true;
					return true;
				}
				if (treeNode.SelectSpecifiedFolder(folderId))
				{
					return true;
				}
			}
			return false;
		}

		protected virtual void RenderAdditionalProperties(TextWriter writer)
		{
		}

		protected virtual void RenderIcon(TextWriter writer, params string[] extraAttributes)
		{
		}

		protected virtual void RenderContent(TextWriter writer)
		{
		}

		internal void RenderUndecoratedChildrenNode(TextWriter writer)
		{
			foreach (TreeNode treeNode in this.Children)
			{
				treeNode.RenderUndecoratedNode(writer);
			}
		}

		internal void RenderUndecoratedNode(TextWriter writer)
		{
			this.RenderUndecoratedNode(writer, 0);
		}

		internal void RenderUndecoratedNode(TextWriter writer, int indent)
		{
			writer.Write("<a class=\"");
			writer.Write(this.NodeClassName);
			writer.Write("\" hideFocus=1 href=\"#\"");
			writer.Write(" style=\"");
			if (!this.ContentVisible)
			{
				writer.Write("display:none;");
			}
			else
			{
				writer.Write(this.UserContext.IsRtl ? "right:" : "left:");
				writer.Write(indent + 21);
				writer.Write("px;");
			}
			writer.Write("\" id=\"");
			Utilities.HtmlEncode(this.Id, writer);
			writer.Write("\"");
			writer.Write(" _indnt=");
			writer.Write(indent);
			if (!this.Selectable)
			{
				writer.Write(" _nosel=1");
			}
			if (!string.IsNullOrEmpty(this.HighlightClassName))
			{
				writer.Write(" _hlCls=\"");
				writer.Write(this.HighlightClassName);
				writer.Write("\"");
			}
			if (this.Selected)
			{
				writer.Write(" _sel=1");
			}
			if (this.IsExpanded)
			{
				writer.Write(" _exp=1");
			}
			if (this.NeedSync)
			{
				writer.Write(" _sync=1");
			}
			if (this.HasChildren)
			{
				writer.Write(" _hsChld=1");
			}
			if (this.IsDummy)
			{
				writer.Write(" _dummy=1");
			}
			writer.Write(" _chldIndnt=");
			writer.Write(this.ChildIndent);
			if (!string.IsNullOrEmpty(this.CustomAttributes))
			{
				writer.Write(" ");
				writer.Write(this.CustomAttributes);
			}
			this.RenderAdditionalProperties(writer);
			writer.Write(">");
			this.RenderNodeBody(writer);
			writer.Write("</a>");
		}

		internal virtual void RenderNodeBody(TextWriter writer)
		{
			if (this.HasIcon)
			{
				this.RenderIcon(writer, new string[]
				{
					"id=\"imgTrNd\""
				});
			}
			writer.Write("<span id=\"spnTrNdCnt\">");
			this.RenderContent(writer);
			writer.Write("</span>");
		}

		internal void Render(TextWriter writer, int indent)
		{
			if (!this.Visible)
			{
				return;
			}
			TreeNode.WrapTreeNodeStart(writer, this.UserContext, this.Id, this.highlightClassName, this.ContentVisible, this.HasChildren, this.Selected, this.isRootNode, indent, this.GetECIcon, this.UserContext.IsRtl);
			this.RenderUndecoratedNode(writer, indent);
			TreeNode.WrapTreeNodeEndToChild(writer, this.Id, this.HasChildren, this.IsExpanded, this.IsRootNode);
			foreach (TreeNode treeNode in this.Children)
			{
				treeNode.Render(writer, indent + this.ChildIndent);
			}
			TreeNode.WrapTreeNodeEnd(writer, this.HasChildren);
		}

		public const string TreeNodeGroupHeaderClass = " trNdGpHd";

		public const string DeletedItemsFoldersClass = " trNdDelFol";

		protected const int DefaultChildIndent = 8;

		protected const int ContentOffset = 21;

		protected const string ImageIconId = "imgTrNd";

		protected const string IdPrefix = "f";

		private readonly List<TreeNode> children = new List<TreeNode>();

		private TreeNode parent;

		private bool selected;

		private bool isExpanded;

		private string customAttributes = string.Empty;

		private string highlightClassName = "trNdHl";

		private string nodeClassName = "trNd";

		private bool? needSync;

		private bool hasIcon;

		private bool isRootNode;

		private bool isDummy;
	}
}
