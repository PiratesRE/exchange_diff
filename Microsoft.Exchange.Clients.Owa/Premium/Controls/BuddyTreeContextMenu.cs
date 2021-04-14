using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class BuddyTreeContextMenu : ContextMenu
	{
		public BuddyTreeContextMenu(UserContext userContext) : base("divBtm", userContext)
		{
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			base.RenderHeader(output);
			base.RenderMenuItem(output, -124986716, ThemeFileId.Chat, "divBtmChat", "chat");
			ContextMenu.RenderMenuDivider(output, "divBtmS1");
			base.RenderMenuItem(output, -1273337860, ThemeFileId.New, "divBtmNewBase", null, false, null, null, new BuddyTreeContextMenuNew(this.userContext));
			base.RenderMenuItem(output, -1225440563, ThemeFileId.None, "divBtmRenameGroup", "renameGroup");
			base.RenderMenuItem(output, -297665725, ThemeFileId.None, "divBtmRemoveGroup", "removeGroup");
			ContextMenu.RenderMenuDivider(output, "divBtmS2");
			base.RenderMenuItem(output, -371326789, ThemeFileId.BigPresenceBlocked, "divBtmBlock", "block");
			base.RenderMenuItem(output, -153800658, ThemeFileId.BigPresenceBlocked, "divBtmUnblock", "unblock");
			base.RenderMenuItem(output, 1388922078, ThemeFileId.RemoveBuddy, "divBtmRmvBase", null, false, null, null, new BuddyTreeContextMenuRemove(this.userContext));
		}
	}
}
