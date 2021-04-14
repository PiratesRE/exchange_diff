using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class EditSmsToolbar : Toolbar
	{
		internal EditSmsToolbar() : base(ToolbarType.Form)
		{
		}

		protected override void RenderButtons()
		{
			BrowserType browserType = base.UserContext.BrowserType;
			base.RenderHelpButton(HelpIdsLight.DefaultLight.ToString(), string.Empty);
			base.RenderButton(ToolbarButtons.SendSms);
			base.RenderButton(ToolbarButtons.SaveImageOnly);
			base.RenderButton(ToolbarButtons.AddressBook);
			base.RenderButton(ToolbarButtons.CheckNames);
		}
	}
}
