using System;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class MessageViewConversationToolbar : Toolbar
	{
		public MessageViewConversationToolbar(bool isNewestOnTop) : base("divCnvTB")
		{
			this.isNewestOnTop = isNewestOnTop;
		}

		protected override void RenderButtons()
		{
			base.RenderButton(ToolbarButtons.NewestOnTop, this.isNewestOnTop ? ToolbarButtonFlags.None : ToolbarButtonFlags.Hidden);
			base.RenderButton(ToolbarButtons.OldestOnTop, (!this.isNewestOnTop) ? ToolbarButtonFlags.None : ToolbarButtonFlags.Hidden);
			base.RenderButton(ToolbarButtons.ExpandAll);
			base.RenderButton(ToolbarButtons.CollapseAll, ToolbarButtonFlags.Hidden);
			base.RenderFloatedSpacer(1, "divMeasure");
		}

		public override bool IsRightAligned
		{
			get
			{
				return true;
			}
		}

		private bool isNewestOnTop;
	}
}
