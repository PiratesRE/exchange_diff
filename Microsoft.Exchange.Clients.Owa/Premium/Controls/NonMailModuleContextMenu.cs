using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class NonMailModuleContextMenu : ContextMenu
	{
		public NonMailModuleContextMenu(UserContext userContext) : base("divNmCm", userContext)
		{
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			base.RenderMenuItem(output, 197744374, ThemeFileId.FolderOpen, "_divO", "open");
			base.RenderMenuItem(output, 839524911, ThemeFileId.None, "_divON", "opennew");
			ContextMenu.RenderMenuDivider(output, "divS1");
			if (this.userContext.IsFeatureEnabled(Feature.Calendar))
			{
				base.RenderMenuItem(output, -475179893, ThemeFileId.None, "_divPCC", null, false, null, null, ColorPickerMenu.Create(this.userContext, "divCalPkr"));
			}
			ContextMenu.RenderMenuDivider(output, "divS2");
			if (this.userContext.IsFeatureEnabled(Feature.Calendar))
			{
				base.RenderMenuItem(output, -317350490, ThemeFileId.None, "_divMCal", "move");
				base.RenderMenuItem(output, 1797288186, ThemeFileId.None, "_divCCal", "copy");
			}
			if (this.userContext.IsFeatureEnabled(Feature.Contacts))
			{
				base.RenderMenuItem(output, 367634897, ThemeFileId.None, "_divMCnt", "move");
				base.RenderMenuItem(output, 904376357, ThemeFileId.None, "_divCCnt", "copy");
			}
			if (this.userContext.IsFeatureEnabled(Feature.Tasks))
			{
				base.RenderMenuItem(output, -1403670248, ThemeFileId.None, "_divMTsk", "move");
				base.RenderMenuItem(output, 1237204988, ThemeFileId.None, "_divCTsk", "copy");
			}
			ContextMenu.RenderMenuDivider(output, "divS3");
			base.RenderMenuItem(output, 1381996313, ThemeFileId.Delete, "_divDel", "delete");
			base.RenderMenuItem(output, 461135208, ThemeFileId.None, "_divRnm", "rename");
			base.RenderMenuItem(output, 1536677211, ThemeFileId.None, "_divRnmGp", "rename");
			base.RenderMenuItem(output, -1233257483, ThemeFileId.None, "_divDelGp", "delete");
			base.RenderMenuItem(output, 54757203, ThemeFileId.None, "_divNG", "newgroup");
			ContextMenu.RenderMenuDivider(output, "divS4");
			base.RenderMenuItem(output, 446088665, ThemeFileId.Previous, "_divMvUp", "moveup");
			base.RenderMenuItem(output, 1959814124, ThemeFileId.Next, "_divMvDn", "movedown");
			ContextMenu.RenderMenuDivider(output, "divS5");
			if (this.userContext.IsFeatureEnabled(Feature.Calendar))
			{
				base.RenderMenuItem(output, 869186573, ThemeFileId.None, "_divShare", null, false, null, null, new ShareAndPublishCalendarMenu(base.UserContext));
			}
			if (this.userContext.IsFeatureEnabled(Feature.Calendar))
			{
				base.RenderMenuItem(output, -1244709321, ThemeFileId.None, "_divNCal", "new");
			}
			if (this.userContext.IsFeatureEnabled(Feature.Contacts))
			{
				base.RenderMenuItem(output, -986693298, ThemeFileId.None, "_divNCnt", "new");
			}
			if (this.userContext.IsFeatureEnabled(Feature.Tasks))
			{
				base.RenderMenuItem(output, 1407967857, ThemeFileId.None, "_divNTsk", "new");
			}
			ContextMenu.RenderMenuDivider(output, "divS6");
			base.RenderMenuItem(output, 907385866, ThemeFileId.None, "_divRFV", "removefromview");
			if (this.userContext.IsFeatureEnabled(Feature.Calendar))
			{
				base.RenderMenuItem(output, 1299617537, ThemeFileId.None, "_divDonotShowCal", "donotshowthiscalendar");
				base.RenderMenuItem(output, -1037074462, ThemeFileId.None, "_divOUCal", "openotherfolder");
			}
		}
	}
}
