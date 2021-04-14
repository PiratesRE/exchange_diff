using System;
using System.Web.UI.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class AdminToolsSlab : SlabControl
	{
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.pnlRemotePowerShell.Visible = RbacPrincipal.Current.RbacConfiguration.ExecutingUserIsAllowedRemotePowerShell;
		}

		protected Panel pnlRemotePowerShell;
	}
}
