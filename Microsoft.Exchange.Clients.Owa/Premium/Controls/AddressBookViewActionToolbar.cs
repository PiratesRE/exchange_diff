using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class AddressBookViewActionToolbar : ViewActionToolbar
	{
		protected override void RenderButtons()
		{
			base.Writer.Write("<div class=\"tbNoBreakDiv\">");
			base.RenderButton(ToolbarButtons.NewMessageToContacts);
			if (base.UserContext.IsInstantMessageEnabled())
			{
				base.RenderInstantMessageButtons();
			}
			if (base.UserContext.IsSmsEnabled)
			{
				base.RenderButton(ToolbarButtons.SendATextMessage);
			}
			base.Writer.Write("</div>");
			base.RenderHelpButton(HelpIdsLight.AddressBookLight.ToString(), string.Empty, true);
		}
	}
}
