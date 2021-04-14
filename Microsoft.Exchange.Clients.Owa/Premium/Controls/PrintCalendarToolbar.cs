using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class PrintCalendarToolbar : Toolbar
	{
		public PrintCalendarToolbar(CalendarViewType viewType) : base("divTBL")
		{
			this.viewType = viewType;
		}

		protected override void RenderButtons()
		{
			ToolbarButtonFlags toolbarButtonFlags = ToolbarButtonFlags.Text;
			ToolbarButtonFlags toolbarButtonFlags2 = ToolbarButtonFlags.Pressed;
			base.RenderButton(ToolbarButtons.PrintCalendarLabel);
			base.RenderFloatedSpacer(20);
			base.RenderButton(ToolbarButtons.PrintDailyView, (this.viewType == CalendarViewType.Min) ? (toolbarButtonFlags | toolbarButtonFlags2) : toolbarButtonFlags);
			base.RenderFloatedSpacer(20);
			base.RenderButton(ToolbarButtons.PrintWeeklyView, (this.viewType == CalendarViewType.Weekly || this.viewType == CalendarViewType.WorkWeek) ? (toolbarButtonFlags | toolbarButtonFlags2) : toolbarButtonFlags);
			base.RenderFloatedSpacer(20);
			base.RenderButton(ToolbarButtons.PrintMonthlyView, (this.viewType == CalendarViewType.Monthly) ? (toolbarButtonFlags | toolbarButtonFlags2) : toolbarButtonFlags);
		}

		private const int SpaceBetweenButtons = 20;

		private readonly CalendarViewType viewType;
	}
}
