using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class ShareCalendarContextMenu : ContextMenu
	{
		public ShareCalendarContextMenu(UserContext userContext) : base("divShareCalCm", userContext)
		{
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			base.RenderMenuItem(output, -1037074462, ThemeFileId.CalendarSharedTo, "divOpenShareCalendar", "opnshcal");
			ContextMenu.RenderMenuDivider(output, "divS1");
			base.RenderMenuItem(output, 14861680, ThemeFileId.CalendarSharedOut, "divShareACalendar", "shcurcal");
			base.RenderMenuItem(output, 1443081199, ThemeFileId.ChangePermission, "divChangeSharingPermission", "chgperm");
			ContextMenu.RenderMenuDivider(output, "divS2");
			base.RenderMenuItem(output, -1068180164, ThemeFileId.WebCalendar, "divPublishACalendar", "pubcal");
			base.RenderMenuItem(output, -2142303303, ThemeFileId.WebCalendar, "divSendPublishLink", "sndLnk");
			base.RenderMenuItem(output, -517719709, ThemeFileId.ChangePermission, "divChangePublishingPermission", "chgpbperm");
			ContextMenu.RenderMenuDivider(output, "divS3");
			base.RenderMenuItem(output, 124032253, ThemeFileId.None, "divShowUrl", "showurl");
		}
	}
}
