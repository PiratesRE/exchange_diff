using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class CalendarViewToolbar : Toolbar
	{
		public CalendarViewToolbar(CalendarViewType viewType, bool isPublicFolder, bool userCanCreateItem, bool userHasRightToLoad, bool isWebPartRequest, ReadingPanePosition readingPanePosition, SanitizedHtmlString folderInfo) : base("divTBL")
		{
			this.viewType = viewType;
			this.folderInfo = folderInfo;
			this.isPublicFolder = isPublicFolder;
			this.flagsForNewButton = (userCanCreateItem ? ToolbarButtonFlags.None : ToolbarButtonFlags.Disabled);
			this.flagsForNonNewButton = (userHasRightToLoad ? ToolbarButtonFlags.None : ToolbarButtonFlags.Disabled);
			this.readingPanePosition = readingPanePosition;
			this.isWebPartRequest = isWebPartRequest;
		}

		protected override void RenderButtons()
		{
			if (base.UserContext.IsWebPartRequest && this.folderInfo != null)
			{
				base.RenderButton(ToolbarButtons.CalendarTitle, this.folderInfo);
			}
			if (this.isPublicFolder)
			{
				base.RenderButton(ToolbarButtons.NewWithAppointmentIcon, this.flagsForNewButton);
			}
			else
			{
				base.RenderButton(ToolbarButtons.NewAppointmentCombo, this.flagsForNewButton, new Toolbar.RenderMenuItems(this.RenderNewMenuItems));
			}
			this.RenderNonNewButton(ToolbarButtons.DeleteTextOnly);
			base.RenderFloatedSpacer(3);
			this.RenderNonNewButton(ToolbarButtons.Today);
			base.RenderFloatedSpacer(3);
			if (this.viewType == CalendarViewType.Min)
			{
				this.RenderNonNewButton(ToolbarButtons.DayView, ToolbarButtonFlags.Pressed);
			}
			else
			{
				this.RenderNonNewButton(ToolbarButtons.DayView);
			}
			if (base.UserContext.IsFeatureEnabled(Feature.Organization))
			{
				if (this.viewType == CalendarViewType.WorkWeek)
				{
					this.RenderNonNewButton(ToolbarButtons.WorkWeekView, ToolbarButtonFlags.Pressed);
				}
				else
				{
					this.RenderNonNewButton(ToolbarButtons.WorkWeekView);
				}
			}
			if (this.viewType == CalendarViewType.Weekly)
			{
				this.RenderNonNewButton(ToolbarButtons.WeekView, ToolbarButtonFlags.Pressed);
			}
			else
			{
				this.RenderNonNewButton(ToolbarButtons.WeekView);
			}
			if (this.viewType == CalendarViewType.Monthly)
			{
				this.RenderNonNewButton(ToolbarButtons.MonthView, ToolbarButtonFlags.Pressed);
			}
			else
			{
				this.RenderNonNewButton(ToolbarButtons.MonthView);
			}
			if (!this.isPublicFolder && !this.isWebPartRequest)
			{
				base.RenderFloatedSpacer(3);
				base.RenderButton(ToolbarButtons.ShareCalendar, ToolbarButtonFlags.None);
			}
			base.RenderButton(ToolbarButtons.ChangeView, this.flagsForNonNewButton, new Toolbar.RenderMenuItems(this.RenderReadingPaneAndListViewMenuItems));
			base.RenderFloatedSpacer(3);
			base.RenderButton(ToolbarButtons.PrintCalendar, ToolbarButtonFlags.None);
			base.RenderFloatedSpacer(1, "divMeasure");
		}

		private void RenderNonNewButton(ToolbarButton button)
		{
			base.RenderButton(button, this.flagsForNonNewButton);
		}

		private void RenderNonNewButton(ToolbarButton button, ToolbarButtonFlags flags)
		{
			base.RenderButton(button, flags | this.flagsForNonNewButton);
		}

		private void RenderNewMenuItems()
		{
			base.RenderMenuItem(ToolbarButtons.NewAppointment);
			base.RenderMenuItem(ToolbarButtons.NewMeetingRequest);
			base.RenderMenuItem(ToolbarButtons.NewMessage);
			base.RenderCustomNewMenuItems();
		}

		private void RenderReadingPaneAndListViewMenuItems()
		{
			ViewDropDownMenu viewDropDownMenu = new ViewDropDownMenu(base.UserContext, this.readingPanePosition, false, false);
			viewDropDownMenu.Render(base.Writer);
		}

		private readonly CalendarViewType viewType;

		private readonly SanitizedHtmlString folderInfo;

		private readonly bool isPublicFolder;

		private readonly ToolbarButtonFlags flagsForNonNewButton;

		private readonly ToolbarButtonFlags flagsForNewButton;

		private readonly ReadingPanePosition readingPanePosition;

		private readonly bool isWebPartRequest;
	}
}
