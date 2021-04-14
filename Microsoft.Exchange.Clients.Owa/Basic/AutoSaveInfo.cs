using System;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	public class AutoSaveInfo : OwaPage
	{
		protected override bool UseStrictMode
		{
			get
			{
				return false;
			}
		}

		protected string MessageIdString
		{
			get
			{
				return this.messageIdString;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			this.messageIdString = Utilities.GetQueryStringParameter(base.Request, "id", true);
		}

		protected void RenderNavigation(NavigationModule navigationModule)
		{
			Navigation navigation = new Navigation(navigationModule, base.OwaContext, base.Response.Output);
			navigation.Render();
		}

		protected void RenderOptions(string helpFile)
		{
			OptionsBar optionsBar = new OptionsBar(base.UserContext, base.Response.Output, OptionsBar.SearchModule.None, OptionsBar.RenderingFlags.None, null);
			optionsBar.Render(helpFile);
		}

		protected void RenderMailSecondaryNavigation()
		{
			MailSecondaryNavigation mailSecondaryNavigation = new MailSecondaryNavigation(base.OwaContext, base.UserContext.InboxFolderId, null, null, null);
			mailSecondaryNavigation.RenderWithoutMruAndAllFolder(base.Response.Output);
		}

		protected void RenderInfoBarIcon()
		{
			base.UserContext.RenderThemeFileUrl(base.Response.Output, ThemeFileId.Informational);
		}

		protected void RenderInfoBarMessage()
		{
			base.Response.Output.Write(LocalizedStrings.GetHtmlEncoded(1537571484));
		}

		private string messageIdString = string.Empty;
	}
}
