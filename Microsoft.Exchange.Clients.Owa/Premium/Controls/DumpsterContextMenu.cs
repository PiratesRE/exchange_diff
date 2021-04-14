using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class DumpsterContextMenu : ContextMenu
	{
		public DumpsterContextMenu(UserContext userContext) : base("divVwm", userContext)
		{
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			base.RenderMenuItem(output, -1604222507, ThemeFileId.Delete, "divp", "delete");
			ContextMenu.RenderMenuDivider(output, "divS1");
			base.RenderMenuItem(output, -1010127550, ThemeFileId.Recover, "divr", "recover");
		}

		private const string ContextMenuId = "divVwm";
	}
}
