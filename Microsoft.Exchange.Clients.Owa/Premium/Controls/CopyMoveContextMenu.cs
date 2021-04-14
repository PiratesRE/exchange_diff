using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class CopyMoveContextMenu : ContextMenu
	{
		internal CopyMoveContextMenu(UserContext userContext) : base("divCMM", userContext)
		{
		}

		protected override bool HasShadedColumn
		{
			get
			{
				return false;
			}
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			TargetFolderMRU.GetFolders(base.UserContext, out this.mruFolderIds, out this.mruFolderNames, out this.mruFolderClassNames, out this.mruFolderCount);
			this.currentMenuItemIndex = 0;
			while (this.currentMenuItemIndex < this.mruFolderCount)
			{
				string text = this.mruFolderNames[this.currentMenuItemIndex];
				if (text.Length > 40)
				{
					text = text.Substring(0, 40);
					text += "...";
				}
				string str = this.mruFolderIds[this.currentMenuItemIndex].ToString();
				base.RenderMenuItem(output, text, ThemeFileId.None, "b" + str, "MvToMruF", false, null, null);
				this.currentMenuItemIndex++;
			}
			if (this.mruFolderCount > 0)
			{
				ContextMenu.RenderMenuDivider(output, "divCMMDvd");
			}
			base.RenderMenuItem(output, -1664268159, ThemeFileId.Move, "divMoveToFolder", "MvToF");
			base.RenderMenuItem(output, -1581636675, ThemeFileId.CopyToFolder, "divCopyToFolder", "CpToF");
		}

		protected override void RenderMenuItemExpandoData(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (this.currentMenuItemIndex >= this.mruFolderCount)
			{
				return;
			}
			output.Write(" sT=\"");
			output.Write(this.mruFolderClassNames[this.currentMenuItemIndex]);
			output.Write("\"");
		}

		private const int FolderDisplayNameLengthMax = 40;

		private int currentMenuItemIndex;

		private OwaStoreObjectId[] mruFolderIds;

		private string[] mruFolderNames;

		private string[] mruFolderClassNames;

		private int mruFolderCount;
	}
}
