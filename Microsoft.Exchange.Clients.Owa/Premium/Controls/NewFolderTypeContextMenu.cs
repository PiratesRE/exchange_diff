using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class NewFolderTypeContextMenu : ContextMenu
	{
		public NewFolderTypeContextMenu(UserContext userContext) : base("divNFTM", userContext)
		{
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			base.RenderMenuItem(output, -1620240669, ThemeFileId.Folder, "divNMailF", "createMailFld");
			base.RenderMenuItem(output, 1010454864, ThemeFileId.Appointment, "divNCalF", "createCalFld", !base.UserContext.IsFeatureEnabled(Feature.Calendar));
			base.RenderMenuItem(output, -1391104162, ThemeFileId.Contact, "divNCntF", "createCntFld", !base.UserContext.IsFeatureEnabled(Feature.Contacts));
			base.RenderMenuItem(output, 1086747003, ThemeFileId.Task, "divNTskF", "createTskFld", !base.UserContext.IsFeatureEnabled(Feature.Tasks));
			base.RenderMenuItem(output, 493304213, ThemeFileId.Notes, "divNtsF", "createNtsFld", !base.UserContext.IsFeatureEnabled(Feature.StickyNotes));
		}
	}
}
