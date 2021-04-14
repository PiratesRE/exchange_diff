using System;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class PublishedCalendarViewToolbar : Toolbar
	{
		public PublishedCalendarViewToolbar() : base("divTBL")
		{
		}

		protected override void RenderButtons()
		{
			base.RenderButton(ToolbarButtons.Today);
			base.RenderFloatedSpacer(3);
			base.RenderButton(ToolbarButtons.DayView);
			base.RenderFloatedSpacer(3);
			base.RenderButton(ToolbarButtons.WeekView);
			base.RenderFloatedSpacer(3);
			base.RenderButton(ToolbarButtons.MonthView, ToolbarButtonFlags.Pressed);
			base.RenderFloatedSpacer(3);
			base.RenderButton(ToolbarButtons.Subscribe);
			base.RenderFloatedSpacer(3);
			base.RenderButton(ToolbarButtons.PrintCalendar, ToolbarButtonFlags.None);
			base.RenderFloatedSpacer(1, "divMeasure");
		}
	}
}
