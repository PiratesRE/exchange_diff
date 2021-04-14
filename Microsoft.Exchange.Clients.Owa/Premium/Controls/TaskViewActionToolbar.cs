using System;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class TaskViewActionToolbar : ViewActionToolbar
	{
		protected override void RenderButtons()
		{
			if (base.ShouldUseTwistyForReplyButton)
			{
				base.RenderButton(ToolbarButtons.ReplyCombo, new Toolbar.RenderMenuItems(base.RenderReplyMenuItems));
			}
			else
			{
				base.RenderButton(ToolbarButtons.Reply);
			}
			base.RenderFloatedSpacer(3);
			base.RenderButton(ToolbarButtons.ReplyAll);
			base.RenderFloatedSpacer(3);
			base.RenderButton(ToolbarButtons.ForwardAsAttachment);
			base.RenderButton(ToolbarButtons.ForwardCombo, ToolbarButtonFlags.Hidden, new Toolbar.RenderMenuItems(base.RenderForwardMenuItems));
		}
	}
}
