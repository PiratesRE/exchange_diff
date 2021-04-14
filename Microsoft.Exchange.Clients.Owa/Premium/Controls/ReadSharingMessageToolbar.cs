using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class ReadSharingMessageToolbar : Toolbar
	{
		internal ReadSharingMessageToolbar(SharingMessageType sharingMessageType, bool disableSharingButton) : base("divShareToolbar", ToolbarType.Form)
		{
			this.sharingMessageType = sharingMessageType;
			this.disableSharingButton = disableSharingButton;
		}

		protected override void RenderButtons()
		{
			ToolbarButtonFlags flags = this.disableSharingButton ? ToolbarButtonFlags.Disabled : ToolbarButtonFlags.None;
			if (this.sharingMessageType.IsInvitationOrAcceptOfRequest)
			{
				base.RenderButton(ToolbarButtons.AddThisCalendar, flags);
			}
			if (this.sharingMessageType.IsRequest)
			{
				base.RenderButton(ToolbarButtons.SharingMyCalendar, flags);
			}
		}

		internal const string Id = "divShareToolbar";

		private SharingMessageType sharingMessageType;

		private bool disableSharingButton;
	}
}
