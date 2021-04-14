using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class DumpsterViewListToolbar : ViewListToolbar
	{
		public DumpsterViewListToolbar() : base(true, ReadingPanePosition.Off)
		{
		}

		protected override void RenderButtons()
		{
			base.RenderButton(ToolbarButtons.Recover);
			base.RenderDivider();
			base.RenderButton(ToolbarButtons.Purge);
		}
	}
}
