using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class TaskViewListToolbar : ViewListToolbar
	{
		public TaskViewListToolbar(bool isPublicFolder, bool isOthersFolder, bool isWebpart, ReadingPanePosition readingPanePosition) : base(false, readingPanePosition)
		{
			this.isPublicFolder = isPublicFolder;
			this.isWebpart = isWebpart;
		}

		protected override bool ShowMultiLineToggle
		{
			get
			{
				return false;
			}
		}

		protected override bool ShowMarkComplete
		{
			get
			{
				return true;
			}
		}

		protected override void RenderButtons()
		{
			if (!this.isPublicFolder)
			{
				base.RenderButtons(ToolbarButtons.NewTaskCombo, new ToolbarButton[0]);
				return;
			}
			if (this.isWebpart)
			{
				base.RenderButtons(ToolbarButtons.NewWithTaskIcon, new ToolbarButton[0]);
				return;
			}
			base.RenderButtons(ToolbarButtons.NewWithTaskIcon, new ToolbarButton[]
			{
				ToolbarButtons.SearchInPublicFolder
			});
		}

		protected override void RenderSharingButton()
		{
		}

		private void RenderShareTaskMenuItems()
		{
			base.RenderMenuItem(ToolbarButtons.OpenSharedTask);
		}

		protected override void RenderNewMenuItems()
		{
			base.RenderMenuItem(ToolbarButtons.NewTask);
			base.RenderMenuItem(ToolbarButtons.NewMessage);
		}

		private readonly bool isPublicFolder;

		private readonly bool isWebpart;
	}
}
