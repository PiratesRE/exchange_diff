using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class AddressBookViewListToolbar : ViewListToolbar
	{
		public AddressBookViewListToolbar(bool isMultiLine, ReadingPanePosition readingPanePosition) : base(isMultiLine, readingPanePosition)
		{
			this.readingPanePosition = readingPanePosition;
		}

		protected override void RenderButtons()
		{
			base.RenderButton(ToolbarButtons.ReadingPaneOffSwap, (this.readingPanePosition == ReadingPanePosition.Off) ? ToolbarButtonFlags.Hidden : ToolbarButtonFlags.Image);
			base.RenderButton(ToolbarButtons.ReadingPaneRightSwap, (this.readingPanePosition == ReadingPanePosition.Right) ? ToolbarButtonFlags.Hidden : ToolbarButtonFlags.Image);
		}

		private ReadingPanePosition readingPanePosition;
	}
}
