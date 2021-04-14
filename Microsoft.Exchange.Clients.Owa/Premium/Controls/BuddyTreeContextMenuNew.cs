using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class BuddyTreeContextMenuNew : ContextMenu
	{
		public BuddyTreeContextMenuNew(UserContext userContext) : base("divBtmNew", userContext)
		{
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			base.RenderMenuItem(output, 1939053649, ThemeFileId.None, "divBtmNewGroup", "newGroup");
			base.RenderMenuItem(output, 252880604, ThemeFileId.None, "divBtmNewContact", "newContact");
			base.RenderMenuItem(output, 252880604, ThemeFileId.None, "divBtmNewOcsContact", "newOcsContact");
		}
	}
}
