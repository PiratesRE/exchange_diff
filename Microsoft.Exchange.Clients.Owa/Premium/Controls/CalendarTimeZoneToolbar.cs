using System;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal sealed class CalendarTimeZoneToolbar : Toolbar
	{
		public CalendarTimeZoneToolbar() : base("divTimeZoneTBL")
		{
		}

		protected override void RenderButtons()
		{
			base.RenderButton(ToolbarButtons.TimeZoneDropDown);
		}

		public override bool IsRightAligned
		{
			get
			{
				return true;
			}
		}
	}
}
