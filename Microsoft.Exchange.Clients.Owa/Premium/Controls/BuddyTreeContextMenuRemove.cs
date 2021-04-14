using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class BuddyTreeContextMenuRemove : ContextMenu
	{
		public BuddyTreeContextMenuRemove(UserContext userContext) : base("divBtmRmv", userContext)
		{
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			base.RenderMenuItem(output, 699356425, ThemeFileId.None, "divBtmRmvGroup", "removeFromGroup");
			base.RenderMenuItem(output, -205408082, ThemeFileId.None, "divBtmRmvList", "removeFromList");
		}
	}
}
