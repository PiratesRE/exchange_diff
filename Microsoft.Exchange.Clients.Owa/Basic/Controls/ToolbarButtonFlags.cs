using System;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	[Flags]
	public enum ToolbarButtonFlags : uint
	{
		Text = 1U,
		Image = 2U,
		ImageAndText = 3U,
		None = 4U,
		NoHover = 8U,
		ImageAndNoHover = 10U,
		TextAndNoHover = 9U,
		Sticky = 16U,
		Selected = 32U,
		Tab = 67U,
		NoAction = 4096U
	}
}
