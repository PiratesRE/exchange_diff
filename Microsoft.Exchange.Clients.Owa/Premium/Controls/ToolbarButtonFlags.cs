using System;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	[Flags]
	public enum ToolbarButtonFlags : uint
	{
		None = 0U,
		Text = 1U,
		Image = 2U,
		ImageAndText = 3U,
		Menu = 4U,
		ComboMenu = 8U,
		Sticky = 16U,
		AlwaysPressed = 32U,
		Pressed = 64U,
		Disabled = 128U,
		Hidden = 256U,
		ComboDropDown = 512U,
		Radio = 1024U,
		Tooltip = 2048U,
		CustomMenu = 4096U,
		NoAction = 8192U,
		ImageAfterText = 16384U,
		BigSize = 32768U,
		HasInnerControl = 65536U
	}
}
