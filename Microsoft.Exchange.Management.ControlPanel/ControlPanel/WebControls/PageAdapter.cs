using System;
using System.Web;
using System.Web.UI.Adapters;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class PageAdapter : PageAdapter
	{
		protected override void OnInit(EventArgs e)
		{
			base.Page.ViewStateUserKey = HttpContext.Current.GetSessionID();
			base.OnInit(e);
		}
	}
}
