using System;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class MessageViewActionToolbar : ViewActionToolbar
	{
		public MessageViewActionToolbar(bool isJunkEmailFolder)
		{
			this.isJunkEmailFolder = isJunkEmailFolder;
		}

		protected override void RenderButtons()
		{
			if (this.isJunkEmailFolder)
			{
				if (base.UserContext.IsJunkEmailEnabled)
				{
					base.RenderButton(ToolbarButtons.NotJunk);
				}
			}
			else
			{
				base.RenderButton(ToolbarButtons.PostReply, ToolbarButtonFlags.Hidden);
				base.RenderButton(ToolbarButtons.ReplyTextOnly);
				base.RenderButton(ToolbarButtons.ReplyAllTextOnly);
				base.RenderButton(ToolbarButtons.ForwardTextOnly);
			}
			base.RenderFloatedSpacer(1, "divMeasure");
		}

		private bool isJunkEmailFolder;
	}
}
