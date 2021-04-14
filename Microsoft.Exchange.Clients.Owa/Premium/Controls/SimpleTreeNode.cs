using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class SimpleTreeNode : TreeNode
	{
		internal SimpleTreeNode(UserContext userContext, string id) : base(userContext)
		{
			this.id = id;
		}

		public override string Id
		{
			get
			{
				return this.id;
			}
		}

		public void SetIconSrc(string iconSrc)
		{
			this.iconHTML = string.Format(CultureInfo.InvariantCulture, "<img src=\"{0}\" {{0}}>", new object[]
			{
				Utilities.HtmlEncode(iconSrc)
			});
			this.themeFileId = ThemeFileId.None;
		}

		public void SetIcon(ThemeFileId themeFileId)
		{
			this.themeFileId = themeFileId;
			this.iconHTML = null;
		}

		internal string NodeAdditionalProperties { get; set; }

		internal string ClientNodeType { get; set; }

		internal override bool Selectable
		{
			get
			{
				return this.selectable;
			}
		}

		public void SetSelectable(bool value)
		{
			this.selectable = value;
		}

		protected override bool HasIcon
		{
			get
			{
				return !string.IsNullOrEmpty(this.iconHTML) || this.themeFileId != ThemeFileId.None;
			}
		}

		protected override void RenderAdditionalProperties(TextWriter writer)
		{
			if (!string.IsNullOrEmpty(this.ClientNodeType))
			{
				writer.Write(" _t=\"");
				Utilities.HtmlEncode(this.ClientNodeType, writer);
				writer.Write("\"");
			}
			if (!string.IsNullOrEmpty(this.NodeAdditionalProperties))
			{
				writer.Write(" ");
				writer.Write(this.NodeAdditionalProperties);
			}
		}

		public void SetContent(string content)
		{
			this.SetContent(content, false);
		}

		public void SetContent(string content, bool htmlEncoded)
		{
			if (htmlEncoded)
			{
				this.contentHTML = content;
				return;
			}
			this.contentHTML = Utilities.HtmlEncode(content, true);
		}

		protected override void RenderContent(TextWriter writer)
		{
			writer.Write(this.contentHTML);
		}

		protected override void RenderIcon(TextWriter writer, params string[] extraAttributes)
		{
			if (this.iconHTML != null)
			{
				writer.Write(this.iconHTML, extraAttributes);
				return;
			}
			if (this.themeFileId != ThemeFileId.None)
			{
				base.UserContext.RenderThemeImage(writer, this.themeFileId, null, extraAttributes);
			}
		}

		private string id;

		private string iconHTML;

		private ThemeFileId themeFileId;

		private bool selectable;

		private string contentHTML = string.Empty;
	}
}
