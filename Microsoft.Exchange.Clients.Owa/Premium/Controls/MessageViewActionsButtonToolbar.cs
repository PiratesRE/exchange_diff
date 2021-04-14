using System;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class MessageViewActionsButtonToolbar : Toolbar
	{
		internal MessageViewActionsButtonToolbar() : base("divActBtnTB")
		{
		}

		protected override void RenderButtons()
		{
			base.RenderButton(ToolbarButtons.Actions);
			base.RenderFloatedSpacer(1, "divMeasure");
		}
	}
}
