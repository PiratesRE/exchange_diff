using System;
using System.Web;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class AcceptedDomainsSlab : SlabControl
	{
		protected override void OnLoad(EventArgs e)
		{
			string[] roles = new string[]
			{
				"FFO"
			};
			ListView listView = (ListView)this.FindControl("AcceptedDomainsListView");
			if (LoginUtil.IsInRoles(HttpContext.Current.User, roles))
			{
				listView.ShowSearchBar = false;
				return;
			}
			listView.ShowSearchBar = true;
		}
	}
}
