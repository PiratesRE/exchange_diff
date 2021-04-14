using System;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class OptionsToolbar : Toolbar
	{
		protected override void RenderButtons()
		{
			base.RenderButton(ToolbarButtons.Save);
		}
	}
}
