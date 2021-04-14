using System;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class EditTaskToolbar : Toolbar
	{
		public EditTaskToolbar(bool isEmbedded, bool userCanDelete) : base(ToolbarType.Form)
		{
			this.isEmbedded = isEmbedded;
			this.userCanDelete = userCanDelete;
		}

		protected override void RenderButtons()
		{
			base.RenderButton(ToolbarButtons.SaveAndClose);
			base.RenderButton(ToolbarButtons.MarkComplete);
			base.RenderButton(ToolbarButtons.AttachFile);
			base.RenderButton(ToolbarButtons.Recurrence);
			base.RenderButton(ToolbarButtons.Forward);
			base.RenderButton(ToolbarButtons.Delete, this.userCanDelete ? ToolbarButtonFlags.None : ToolbarButtonFlags.Disabled);
			if (!this.isEmbedded)
			{
				base.RenderButton(ToolbarButtons.Categories);
			}
		}

		private bool isEmbedded;

		private bool userCanDelete;
	}
}
