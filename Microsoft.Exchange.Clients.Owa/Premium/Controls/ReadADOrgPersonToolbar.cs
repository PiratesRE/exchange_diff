using System;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class ReadADOrgPersonToolbar : Toolbar
	{
		internal ReadADOrgPersonToolbar() : base(ToolbarType.Form)
		{
		}

		protected override void RenderButtons()
		{
			if (base.UserContext.IsInstantMessageEnabled())
			{
				base.RenderInstantMessageButtons();
			}
		}
	}
}
