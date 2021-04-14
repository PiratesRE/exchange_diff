using System;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class ReadPublishInvitionMessageToolbar : Toolbar
	{
		internal ReadPublishInvitionMessageToolbar(bool hideViewButton, bool disableSubscribeButton) : base("divShareToolbar", ToolbarType.Form)
		{
			this.hideViewButton = hideViewButton;
			this.disableSubscribeButton = disableSubscribeButton;
		}

		protected override void RenderButtons()
		{
			if (CalendarUtilities.CanSubscribeInternetCalendar())
			{
				base.RenderButton(ToolbarButtons.SubscribeToThisCalendar, this.disableSubscribeButton ? ToolbarButtonFlags.Disabled : ToolbarButtonFlags.None);
			}
			if (!this.hideViewButton)
			{
				base.RenderButton(ToolbarButtons.ViewThisCalendar);
			}
		}

		private bool hideViewButton;

		private bool disableSubscribeButton;
	}
}
