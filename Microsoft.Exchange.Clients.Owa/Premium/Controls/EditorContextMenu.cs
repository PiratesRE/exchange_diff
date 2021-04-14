using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class EditorContextMenu : ContextMenu
	{
		public EditorContextMenu(UserContext userContext) : base("divEcm", userContext, true)
		{
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			base.RenderMenuItem(output, 1740969141, ThemeFileId.EditorUndo, "divUndo", "Undo");
			ContextMenu.RenderMenuDivider(output, "divS1");
			base.RenderMenuItem(output, 1340229483, ThemeFileId.EditorCut, "divCut", "Cut");
			base.RenderMenuItem(output, 363101290, ThemeFileId.Copy, "divCpy", "Copy");
			base.RenderMenuItem(output, 1217315034, ThemeFileId.EditorPaste, "divPst", "Paste");
			base.RenderMenuItem(output, -574210492, ThemeFileId.Delete, "divDel", "Delete");
			ContextMenu.RenderMenuDivider(output, "divS2");
			base.RenderMenuItem(output, -1658453902, ThemeFileId.None, "divSlA", "SelectAll");
		}
	}
}
