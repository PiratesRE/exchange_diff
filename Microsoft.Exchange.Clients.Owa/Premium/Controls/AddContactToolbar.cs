using System;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class AddContactToolbar : Toolbar
	{
		internal AddContactToolbar() : base(ToolbarType.AddContact)
		{
		}

		protected override void RenderButtons()
		{
			base.RenderButton(ToolbarButtons.InviteContact);
			base.RenderButton(ToolbarButtons.AddressBook);
			base.RenderButton(ToolbarButtons.CheckNames);
		}
	}
}
