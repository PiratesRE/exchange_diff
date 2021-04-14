using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class MessageItemManageToolbar : Toolbar
	{
		public MessageItemManageToolbar(bool isPublicFolder, bool allowConversationView, bool includeEmptyFolderButton, bool isMultiLine, bool isOthersFolder, bool isWebPart, string folderClass, ReadingPanePosition readingPanePosition, bool isConversationView, bool isNewestOnTop, bool isShowTree, bool isDeletedItems, bool isJunkEmail) : base("divMsgItemTB")
		{
			this.isPublicFolder = isPublicFolder;
			this.allowConversationView = allowConversationView;
			this.includeEmptyFolderButton = includeEmptyFolderButton;
			this.isWebPart = isWebPart;
			this.folderClass = folderClass;
			this.isOthersFolder = isOthersFolder;
			this.readingPanePosition = readingPanePosition;
			this.isDeletedItems = isDeletedItems;
			this.isJunkEmail = isJunkEmail;
		}

		protected void RenderNewMenuItems()
		{
			base.RenderMenuItem(ToolbarButtons.NewMessage);
			if (base.UserContext.IsSmsEnabled)
			{
				base.RenderMenuItem(ToolbarButtons.NewSms);
			}
			if (base.UserContext.IsFeatureEnabled(Feature.Calendar))
			{
				base.RenderMenuItem(ToolbarButtons.NewMeetingRequest);
			}
		}

		protected void RenderAllNewMenuItems()
		{
			this.RenderNewMenuItems();
			base.RenderCustomNewMenuItems();
		}

		protected override void RenderButtons()
		{
			if (this.isPublicFolder)
			{
				base.RenderButton(ToolbarButtons.NewWithPostIcon);
			}
			else
			{
				base.RenderButton(ToolbarButtons.NewMessageCombo, new Toolbar.RenderMenuItems(this.RenderAllNewMenuItems));
			}
			if (this.allowConversationView)
			{
				base.RenderButton(ToolbarButtons.DeleteCombo, new Toolbar.RenderMenuItems(this.RenderDeleteMenuItems));
				base.RenderButton(ToolbarButtons.CancelIgnoreConversationCombo, ToolbarButtonFlags.Hidden, new Toolbar.RenderMenuItems(this.RenderCancelIgnoreConversationMenuItems));
			}
			else
			{
				base.RenderButton(ToolbarButtons.DeleteTextOnly);
			}
			base.RenderButton(ToolbarButtons.MoveTextOnly);
			if (this.includeEmptyFolderButton)
			{
				base.RenderButton(ToolbarButtons.EmptyFolder);
			}
			if (!this.isWebPart && !this.isPublicFolder && (string.IsNullOrEmpty(this.folderClass) || ObjectClass.IsMessageFolder(this.folderClass)) && !this.isDeletedItems && !this.isJunkEmail)
			{
				base.RenderButton(ToolbarButtons.FilterCombo, this.isOthersFolder ? ToolbarButtonFlags.Disabled : ToolbarButtonFlags.None);
			}
			base.RenderButton(ToolbarButtons.ChangeView, ToolbarButtonFlags.Menu, new Toolbar.RenderMenuItems(this.RenderChangeViewMenuItems));
			if (this.isPublicFolder && !this.isWebPart)
			{
				base.RenderButton(ToolbarButtons.SearchInPublicFolder);
			}
			base.RenderFloatedSpacer(1, "divMeasure");
		}

		private void RenderDeleteMenuItems()
		{
			base.RenderMenuItem(ToolbarButtons.DeleteInDropDown);
			base.RenderMenuItem(ToolbarButtons.IgnoreConversation);
		}

		private void RenderCancelIgnoreConversationMenuItems()
		{
			base.RenderMenuItem(ToolbarButtons.CancelIgnoreConversationInDropDown);
			base.RenderMenuItem(ToolbarButtons.DeleteInCancelIgnoreConversationDropDown);
		}

		private void RenderChangeViewMenuItems()
		{
			ViewDropDownMenu viewDropDownMenu = new ViewDropDownMenu(base.UserContext, this.readingPanePosition, this.allowConversationView, true);
			viewDropDownMenu.Render(base.Writer);
		}

		private readonly bool isPublicFolder;

		private readonly bool includeEmptyFolderButton;

		private bool allowConversationView;

		private bool isWebPart;

		private bool isOthersFolder;

		private bool isDeletedItems;

		private bool isJunkEmail;

		private string folderClass;

		private ReadingPanePosition readingPanePosition;
	}
}
