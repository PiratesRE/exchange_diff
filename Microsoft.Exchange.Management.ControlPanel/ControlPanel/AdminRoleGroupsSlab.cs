using System;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class AdminRoleGroupsSlab : SlabControl
	{
		protected override void OnLoad(EventArgs e)
		{
			if (RbacPrincipal.Current.IsInRole("FFO"))
			{
				this.roleGroupsResultPane.ShowSearchBar = false;
			}
			base.OnLoad(e);
		}

		protected ListView roleGroupsResultPane;
	}
}
