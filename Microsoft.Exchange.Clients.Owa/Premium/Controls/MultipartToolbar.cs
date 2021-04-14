using System;
using System.IO;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class MultipartToolbar : Toolbar
	{
		public MultipartToolbar(params MultipartToolbar.ToolbarInfo[] toolbars)
		{
			this.toolbars = toolbars;
		}

		public override bool HasBigButton
		{
			get
			{
				foreach (MultipartToolbar.ToolbarInfo toolbarInfo in this.toolbars)
				{
					if (toolbarInfo.Toolbar.HasBigButton)
					{
						return true;
					}
				}
				return false;
			}
		}

		public override void Render(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			foreach (MultipartToolbar.ToolbarInfo toolbarInfo in this.toolbars)
			{
				writer.Write("<div id=\"");
				writer.Write(toolbarInfo.ContainerId);
				writer.Write("\"");
				if (toolbarInfo.Toolbar.IsRightAligned)
				{
					writer.Write(" fRtAlgn=\"1\"");
				}
				writer.Write(">");
				toolbarInfo.Toolbar.Render(writer);
				writer.Write("</div>");
			}
			writer.Write("<div class=\"tfFill ");
			writer.Write(this.HasBigButton ? "tfBigHeight" : "tfHeight");
			writer.Write("\"></div>");
		}

		protected override void RenderButtons()
		{
			throw new NotImplementedException();
		}

		private MultipartToolbar.ToolbarInfo[] toolbars;

		public class ToolbarInfo
		{
			public ToolbarInfo(Toolbar toolbar, string containerId)
			{
				this.Toolbar = toolbar;
				this.ContainerId = containerId;
			}

			public Toolbar Toolbar { get; set; }

			public string ContainerId { get; set; }
		}
	}
}
