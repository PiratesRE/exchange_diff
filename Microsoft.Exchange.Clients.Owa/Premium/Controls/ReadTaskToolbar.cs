using System;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class ReadTaskToolbar : Toolbar
	{
		public ReadTaskToolbar(bool isAssignedTask, bool userCanDelete) : base(ToolbarType.Form)
		{
			this.isAssignedTask = isAssignedTask;
			this.userCanDelete = userCanDelete;
		}

		protected override void RenderButtons()
		{
			base.RenderButton(ToolbarButtons.Reply);
			base.RenderButton(ToolbarButtons.ReplyAll);
			base.RenderButton(ToolbarButtons.Forward);
			if (!this.isAssignedTask)
			{
				base.RenderButton(ToolbarButtons.Delete, this.userCanDelete ? ToolbarButtonFlags.None : ToolbarButtonFlags.Disabled);
			}
		}

		private bool isAssignedTask;

		private bool userCanDelete;
	}
}
