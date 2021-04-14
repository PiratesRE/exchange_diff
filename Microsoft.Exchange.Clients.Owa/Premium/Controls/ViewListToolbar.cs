using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public abstract class ViewListToolbar : Toolbar
	{
		protected ViewListToolbar(bool isMultiLine, ReadingPanePosition readingPanePosition) : base("divTBL")
		{
			this.isMultiLine = isMultiLine;
			this.readingPanePosition = readingPanePosition;
		}

		protected virtual bool ShowMultiLineToggle
		{
			get
			{
				return false;
			}
		}

		protected virtual bool ShowMarkComplete
		{
			get
			{
				return false;
			}
		}

		protected bool IsMultiLine
		{
			get
			{
				return this.isMultiLine;
			}
		}

		protected virtual bool ShowCategoryButton
		{
			get
			{
				return true;
			}
		}

		protected void RenderButtons(ToolbarButton newButton, params ToolbarButton[] extraButtons)
		{
			if (newButton == null)
			{
				throw new ArgumentNullException("newButton");
			}
			if ((newButton.Flags & ToolbarButtonFlags.ComboMenu) == ToolbarButtonFlags.ComboMenu || (newButton.Flags & ToolbarButtonFlags.Menu) == ToolbarButtonFlags.Menu)
			{
				base.RenderButton(newButton, new Toolbar.RenderMenuItems(this.RenderAllNewMenuItems));
			}
			else
			{
				base.RenderButton(newButton);
			}
			base.RenderButton(ToolbarButtons.Delete);
			base.RenderFloatedSpacer(3);
			base.RenderButton(ToolbarButtons.Move);
			if (this.ShowCategoryButton)
			{
				base.RenderFloatedSpacer(3);
				base.RenderButton(ToolbarButtons.Categories);
			}
			base.RenderFloatedSpacer(3);
			this.RenderSharingButton();
			if (this.ShowMarkComplete)
			{
				base.RenderButton(ToolbarButtons.MarkCompleteNoText);
			}
			base.RenderButton(ToolbarButtons.ChangeView, new Toolbar.RenderMenuItems(this.RenderChangeViewMenuItems));
			base.RenderButton(ToolbarButtons.CheckMessages);
			if (extraButtons != null)
			{
				foreach (ToolbarButton button in extraButtons)
				{
					base.RenderButton(button);
				}
			}
			base.RenderFloatedSpacer(1, "divMeasure");
		}

		protected virtual void RenderSharingButton()
		{
		}

		protected virtual void RenderNewMenuItems()
		{
		}

		protected void RenderAllNewMenuItems()
		{
			this.RenderNewMenuItems();
			base.RenderCustomNewMenuItems();
		}

		private void RenderChangeViewMenuItems()
		{
			ViewDropDownMenu viewDropDownMenu = new ViewDropDownMenu(base.UserContext, this.readingPanePosition, false, true);
			viewDropDownMenu.Render(base.Writer);
		}

		private bool isMultiLine = true;

		private ReadingPanePosition readingPanePosition;
	}
}
