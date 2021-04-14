using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class RecipientAnrContextMenu : ContextMenu
	{
		public RecipientAnrContextMenu(UserContext userContext) : base("divAm", userContext, false, true)
		{
		}

		private void RenderAnrHeader(TextWriter output)
		{
			output.Write("<div class=\"sttc\" nowrap><span id=\"spnImg\" class=\"cmIco\"></span>");
			RenderingUtilities.RenderInlineSpacer(output, this.userContext, 12);
			output.Write("<span id=\"spnHdr\"></span></div>");
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			this.RenderAnrHeader(output);
			output.Write("<div id=divAmNms></div>");
			ContextMenu.RenderMenuDivider(output, "divRemoveDivider", false);
			base.RenderMenuItem(output, 1388922078, ThemeFileId.None, "divAmRmv", "rmv");
		}
	}
}
