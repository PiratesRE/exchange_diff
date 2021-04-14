using System;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class ApprovalRequestToolbar : Toolbar
	{
		internal ApprovalRequestToolbar() : base("tblARTB", ToolbarType.Preview)
		{
		}

		internal ApprovalRequestToolbar(bool isApproveEditingEnabled, bool isRejectEditingEnabled) : base("tblARTB", ToolbarType.Preview)
		{
			this.isApproveEditingEnabled = isApproveEditingEnabled;
			this.isRejectEditingEnabled = isRejectEditingEnabled;
		}

		protected override void RenderButtons()
		{
			if (this.isApproveEditingEnabled)
			{
				base.RenderButton(ToolbarButtons.ApprovalApproveMenu, new Toolbar.RenderMenuItems(this.RenderResponseEditingMenuItems));
			}
			else
			{
				base.RenderButton(ToolbarButtons.ApprovalApprove);
			}
			if (this.isRejectEditingEnabled)
			{
				base.RenderButton(ToolbarButtons.ApprovalRejectMenu, new Toolbar.RenderMenuItems(this.RenderResponseEditingMenuItems));
				return;
			}
			base.RenderButton(ToolbarButtons.ApprovalReject);
		}

		private void RenderResponseEditingMenuItems()
		{
			base.RenderMenuItem(ToolbarButtons.ApprovalEditResponse);
			base.RenderMenuItem(ToolbarButtons.ApprovalSendResponseNow);
		}

		private const string ApprovalRequestToolbarId = "tblARTB";

		private bool isApproveEditingEnabled;

		private bool isRejectEditingEnabled;
	}
}
