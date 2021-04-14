using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class PersonViewActionToolbar : ViewActionToolbar
	{
		protected override void RenderButtons()
		{
			base.RenderButton(ToolbarButtons.NewMessageToContacts);
			if (base.UserContext.IsSmsEnabled)
			{
				base.RenderButton(ToolbarButtons.SendATextMessage);
			}
			if (base.UserContext.IsFeatureEnabled(Feature.Calendar))
			{
				base.RenderButton(ToolbarButtons.NewMeetingRequestToContacts);
			}
			base.RenderButton(ToolbarButtons.ForwardAsAttachment);
		}
	}
}
