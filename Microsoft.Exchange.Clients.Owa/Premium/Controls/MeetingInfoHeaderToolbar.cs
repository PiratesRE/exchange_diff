using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class MeetingInfoHeaderToolbar : Toolbar
	{
		internal MeetingInfoHeaderToolbar() : base("mtgHeaderTb", ToolbarType.Form)
		{
		}

		internal MeetingInfoHeaderToolbar(ResponseType responseType) : base("mtgHeaderTb", ToolbarType.Form)
		{
			this.responseType = responseType;
		}

		protected override void RenderButtons()
		{
			switch (this.responseType)
			{
			case ResponseType.Tentative:
				base.RenderButton(ToolbarButtons.ResponseTentative);
				return;
			case ResponseType.Accept:
				base.RenderButton(ToolbarButtons.ResponseAccepted);
				return;
			case ResponseType.Decline:
				base.RenderButton(ToolbarButtons.ResponseDeclined);
				return;
			default:
				base.RenderButton(ToolbarButtons.MeetingCancelled);
				return;
			}
		}

		private ResponseType responseType;
	}
}
