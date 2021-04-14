using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class ShareAndPublishCalendarMenu : ContextMenu
	{
		public ShareAndPublishCalendarMenu(UserContext userContext) : base("divShareSubMenu", userContext)
		{
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			base.RenderMenuItem(output, 14861680, ThemeFileId.None, "_divShCal", "sharecal");
			base.RenderMenuItem(output, 1443081199, ThemeFileId.None, "_divChShPm", "changesharingpremissions");
			ContextMenu.RenderMenuDivider(output, "divS1");
			base.RenderMenuItem(output, -1068180164, ThemeFileId.None, "_divPbCal", "pubcal");
			base.RenderMenuItem(output, -2142303303, ThemeFileId.None, "_divSndLnkCal", "sndlnkcal");
			base.RenderMenuItem(output, -517719709, ThemeFileId.None, "_divChPbPm", "pubcal");
			ContextMenu.RenderMenuDivider(output, "divS2");
			base.RenderMenuItem(output, 124032253, ThemeFileId.None, "_divShowUrl", "showurl");
		}
	}
}
