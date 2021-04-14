using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class PublicFolderTreeContextMenu : ContextMenu
	{
		public PublicFolderTreeContextMenu(UserContext userContext) : base("divPFtm", userContext)
		{
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			base.RenderMenuItem(output, 197744374, ThemeFileId.FolderOpen, "divOPF", "open");
			base.RenderMenuItem(output, 839524911, ThemeFileId.None, "divOPFN", "opennew");
			ContextMenu.RenderMenuDivider(output, "divS1");
			base.RenderMenuItem(output, -874816512, ThemeFileId.MoveFolder, "divMvPF", "movefolder");
			base.RenderMenuItem(output, 891793844, ThemeFileId.CopyFolder, "divCpPF", "copyfolder");
			ContextMenu.RenderMenuDivider(output, "divS2");
			base.RenderMenuItem(output, 1381996313, ThemeFileId.Delete, "divDPF", "delete");
			base.RenderMenuItem(output, 461135208, ThemeFileId.None, "divRnmPF", "rename");
			ContextMenu.RenderMenuDivider(output, "divS3");
			base.RenderMenuItem(output, -1171996716, ThemeFileId.None, "divCPF", null, false, null, null, new NewFolderTypeContextMenu(this.userContext));
			base.RenderMenuItem(output, -1392259974, ThemeFileId.MessageRead, "divMPFR", "markread");
			base.RenderMenuItem(output, 491943887, ThemeFileId.Deleted, "divEPF", "emptyfldr");
		}
	}
}
