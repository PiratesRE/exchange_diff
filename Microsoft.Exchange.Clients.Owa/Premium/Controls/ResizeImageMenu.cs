using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class ResizeImageMenu : ContextMenu
	{
		internal ResizeImageMenu(UserContext userContext) : base("divRszMnu", userContext, true)
		{
		}

		private void RenderResizeMenuHeader(TextWriter output)
		{
			output.Write("<div class=\"rszHd\" nowrap>");
			output.Write(LocalizedStrings.GetNonEncoded(2054402001));
			output.Write("</div>");
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			this.RenderResizeMenuHeader(output);
			base.RenderMenuItem(output, -21894641, ThemeFileId.Clear, null, "25");
			base.RenderMenuItem(output, -787528491, ThemeFileId.Clear, null, "50");
			base.RenderMenuItem(output, 1852211349, ThemeFileId.Clear, null, "100");
			base.RenderMenuItem(output, -668906919, ThemeFileId.Clear, null, "200");
			ContextMenu.RenderMenuDivider(output, null);
			base.RenderMenuItem(output, 323750620, ThemeFileId.Clear, null, "fitToWindow");
		}

		private const string ResizeImageMenuId = "divRszMnu";
	}
}
