using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal abstract class Tree
	{
		protected Tree(UserContext userContext, TreeNode rootNode)
		{
			this.userContext = userContext;
			this.rootNode = rootNode;
		}

		protected UserContext UserContext
		{
			get
			{
				return this.userContext;
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

		internal string ErrDiv
		{
			get
			{
				return this.errDiv;
			}
			set
			{
				this.errDiv = value;
			}
		}

		internal string ErrHideId
		{
			get
			{
				return this.errHideId;
			}
			set
			{
				this.errHideId = value;
			}
		}

		internal TreeNode RootNode
		{
			get
			{
				return this.rootNode;
			}
		}

		internal void Render(TextWriter writer)
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "Tree.Render()");
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.RenderOuterDivStart(writer);
			this.RootNode.Render(writer, 0);
			this.RenderOuterDivEnd(writer);
		}

		protected virtual void RenderAdditionalProperties(TextWriter writer)
		{
		}

		internal virtual string Id
		{
			get
			{
				return "divTr";
			}
		}

		private void RenderOuterDivStart(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<div fTR=1 id=\"");
			writer.Write(this.Id);
			writer.Write("\"");
			if (!string.IsNullOrEmpty(this.ErrDiv))
			{
				writer.Write(" _errDiv=\"");
				writer.Write(this.ErrDiv);
				writer.Write("\"");
			}
			if (!string.IsNullOrEmpty(this.ErrHideId))
			{
				writer.Write(" _errHd=\"");
				writer.Write(this.ErrHideId);
				writer.Write("\"");
			}
			if (!string.IsNullOrEmpty(this.CustomAttributes))
			{
				writer.Write(" ");
				writer.Write(this.CustomAttributes);
			}
			this.RenderAdditionalProperties(writer);
			writer.Write(">");
		}

		private void RenderOuterDivEnd(TextWriter writer)
		{
			writer.Write("</div>");
		}

		private const string TreeDivId = "divTr";

		private readonly UserContext userContext;

		private readonly TreeNode rootNode;

		private string customAttributes = string.Empty;

		private string errDiv;

		private string errHideId;
	}
}
