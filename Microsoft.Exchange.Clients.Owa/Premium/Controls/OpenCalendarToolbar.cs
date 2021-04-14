using System;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class OpenCalendarToolbar : Toolbar
	{
		internal OpenCalendarToolbar() : base("openCalendarTb", ToolbarType.Form)
		{
		}

		public override bool HasBigButton
		{
			get
			{
				return true;
			}
		}

		protected override void RenderButtons()
		{
			base.RenderButton(ToolbarButtons.ShowCalendar);
		}
	}
}
