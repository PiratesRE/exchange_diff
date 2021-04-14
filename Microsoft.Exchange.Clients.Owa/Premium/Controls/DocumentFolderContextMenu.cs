using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class DocumentFolderContextMenu : ContextMenu
	{
		public DocumentFolderContextMenu(UserContext userContext) : base("divDfm", userContext)
		{
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			base.RenderMenuItem(output, 197744374, ThemeFileId.None, "divDfmO", "opendl");
			base.RenderMenuItem(output, 839524911, ThemeFileId.None, "divDfmON", "opennewdl");
			base.RenderMenuItem(output, 461135208, ThemeFileId.None, "divDfmR", "renamedl");
			base.RenderMenuItem(output, 1381996313, ThemeFileId.Delete, "divDfmD", "deletedl");
		}
	}
}
