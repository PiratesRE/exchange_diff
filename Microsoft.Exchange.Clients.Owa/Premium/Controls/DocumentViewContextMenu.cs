using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class DocumentViewContextMenu : ContextMenu
	{
		public DocumentViewContextMenu(UserContext userContext) : base("divVwm", userContext)
		{
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			base.RenderMenuItem(output, 197744374, ThemeFileId.None, "divO", "opndc");
			base.RenderMenuItem(output, 1927924110, ThemeFileId.None, "divON", "opndlinnew");
			base.RenderMenuItem(output, 1547877601, ThemeFileId.HtmlDocument, "divOAWP", "opnwp");
			base.RenderMenuItem(output, 11725295, ThemeFileId.Send, "divS", "senddc");
			base.RenderMenuItem(output, 636550561, ThemeFileId.Copy, "divCS", "copyuri");
			base.RenderMenuItem(output, -1028120515, ThemeFileId.None, "divAF", "addfav");
			ContextMenu.RenderMenuDivider(output, "divS1");
			base.RenderMenuItem(output, string.Empty, ThemeFileId.Contact, "divIMB", "infomb", false, "shwNms(0)", null);
			base.RenderMenuItem(output, string.Empty, ThemeFileId.Contact, "divICOT", "infocot", false, "shwNms(1)", null);
		}
	}
}
