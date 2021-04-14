using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class MessageViewListToolbar : ViewListToolbar
	{
		public MessageViewListToolbar(bool isMultiLine, bool isPublicFolder, bool isOthersFolder, bool isSearchFolder, bool isWebPart, string folderClass, ReadingPanePosition readingPanePosition) : base(isMultiLine, readingPanePosition)
		{
			this.isPublicFolder = isPublicFolder;
			this.isWebPart = isWebPart;
			this.isSearchFolder = isSearchFolder;
		}

		protected override void RenderButtons()
		{
			if (!this.isPublicFolder)
			{
				base.RenderButtons(ToolbarButtons.NewMessageCombo, new ToolbarButton[0]);
				return;
			}
			if (this.isWebPart)
			{
				base.RenderButtons(ToolbarButtons.NewWithPostIcon, new ToolbarButton[0]);
				return;
			}
			base.RenderButtons(ToolbarButtons.NewWithPostIcon, new ToolbarButton[]
			{
				ToolbarButtons.SearchInPublicFolder
			});
		}

		protected override void RenderNewMenuItems()
		{
			base.RenderMenuItem(ToolbarButtons.NewMessage);
			if (base.UserContext.IsSmsEnabled)
			{
				base.RenderMenuItem(ToolbarButtons.NewSms);
			}
			if (base.UserContext.IsFeatureEnabled(Feature.Calendar))
			{
				base.RenderMenuItem(ToolbarButtons.NewAppointment);
				base.RenderMenuItem(ToolbarButtons.NewMeetingRequest);
			}
			if (base.UserContext.IsFeatureEnabled(Feature.Contacts))
			{
				base.RenderMenuItem(ToolbarButtons.NewContact);
				base.RenderMenuItem(ToolbarButtons.NewContactDistributionList);
			}
			if (base.UserContext.IsFeatureEnabled(Feature.Tasks))
			{
				base.RenderMenuItem(ToolbarButtons.NewTask);
			}
			if (base.UserContext.IsFeatureEnabled(Feature.PublicFolders))
			{
				base.RenderMenuItem(ToolbarButtons.NewPost, this.isSearchFolder);
			}
		}

		private readonly bool isPublicFolder;

		private readonly bool isWebPart;

		private readonly bool isSearchFolder;
	}
}
