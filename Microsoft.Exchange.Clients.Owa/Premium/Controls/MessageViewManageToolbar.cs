using System;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class MessageViewManageToolbar : Toolbar
	{
		public MessageViewManageToolbar() : base("divMsgViewTB")
		{
		}

		protected override void RenderButtons()
		{
			base.RenderButton(ToolbarButtons.CheckMessages);
			base.RenderFloatedSpacer(1, "divMeasure");
		}

		public override bool IsRightAligned
		{
			get
			{
				return true;
			}
		}
	}
}
