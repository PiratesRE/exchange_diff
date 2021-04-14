using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class FolderTreeContextMenu : ContextMenu
	{
		public FolderTreeContextMenu(UserContext userContext) : base("divFtm", userContext)
		{
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			base.RenderMenuItem(output, 197744374, ThemeFileId.FolderOpen, "divO", "open");
			base.RenderMenuItem(output, 839524911, ThemeFileId.None, "divON", "opennew");
			base.RenderMenuItem(output, 1737823205, ThemeFileId.None, "divOUM", "openotherfolder");
			base.RenderMenuItem(output, 907385866, ThemeFileId.None, "divRSF", "removefromview");
			ContextMenu.RenderMenuDivider(output, "divS1");
			base.RenderMenuItem(output, -1782355260, ThemeFileId.None, "divRs", "restore");
			base.RenderMenuItem(output, -874816512, ThemeFileId.MoveFolder, "divMvF", "move");
			base.RenderMenuItem(output, 891793844, ThemeFileId.CopyFolder, "divCpF", "copy");
			ContextMenu.RenderMenuDivider(output, "divS2");
			base.RenderMenuItem(output, 1381996313, ThemeFileId.Delete, "divD", "delete");
			base.RenderMenuItem(output, 461135208, ThemeFileId.None, "divRnm", "rename");
			ContextMenu.RenderMenuDivider(output, "divS3");
			base.RenderMenuItem(output, -1028120515, ThemeFileId.None, "divFvr", "addfavorite");
			base.RenderMenuItem(output, -1415426061, ThemeFileId.None, "divRmFvr", "removefromfavorite");
			ContextMenu.RenderMenuDivider(output, "divS4");
			base.RenderMenuItem(output, 446088665, ThemeFileId.Previous, "divMvUp", "moveup");
			base.RenderMenuItem(output, 1959814124, ThemeFileId.Next, "divMvDn", "movedown");
			ContextMenu.RenderMenuDivider(output, "divS5");
			base.RenderMenuItem(output, 540527327, ThemeFileId.Folder2, "divCF", "createfolder");
			base.RenderMenuItem(output, -1392259974, ThemeFileId.MessageRead, "divMAR", "markread");
			DeletePolicyContextMenu.RenderAsSubmenu(output, this.userContext, new RenderMenuItemDelegate(base.RenderMenuItem));
			MovePolicyContextMenu.RenderAsSubmenu(output, this.userContext, new RenderMenuItemDelegate(base.RenderMenuItem));
			base.RenderMenuItem(output, 616592932, ThemeFileId.Deleted, "divED", "emptydeleted");
			base.RenderMenuItem(output, 369288321, ThemeFileId.RecoverDeletedItemsSmall, "divRD", "recoverdeleted");
			base.RenderMenuItem(output, 491943887, ThemeFileId.Deleted, "divEF", "emptyfldr");
			base.RenderMenuItem(output, 1628292131, ThemeFileId.Deleted, "divEJ", "emptyjunk");
		}
	}
}
