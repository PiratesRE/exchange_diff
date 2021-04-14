using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class OptionsHelpToolbar : Toolbar
	{
		public OptionsHelpToolbar() : base("tblTBH")
		{
		}

		protected override void RenderButtons()
		{
			base.RenderHelpButton(HelpIdsLight.OptionsLight.ToString(), string.Empty, true);
		}
	}
}
