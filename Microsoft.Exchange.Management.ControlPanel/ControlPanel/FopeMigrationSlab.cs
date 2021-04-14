using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class FopeMigrationSlab : SlabControl
	{
		protected override void OnLoad(EventArgs e)
		{
			IpSafeListings ipSafeListings = new IpSafeListings();
			PowerShellResults<IpSafeListing> @object = ipSafeListings.GetObject(null);
			if (@object.Count<IpSafeListing>() > 0)
			{
				string text = @object.First<IpSafeListing>().FoseLink;
				if (text.Contains("FoseLinkNotAvailable"))
				{
					text = "https://sts.messaging.microsoft.com/fedlogin.aspx?ReturnUrl=%2fDefault.aspx%3fwa%3dwsignin1.0%26wtrealm%3dhttps%253a%252f%252fadmin.messaging.microsoft.com%26wctx%3drm%253d0%2526id%253dpassive%2526ru%253d%25252f%26wct%3d2012-12-04T20%253a03%253a25Z&wa=wsignin1.0&wtrealm=https%3a%2f%2fadmin.messaging.microsoft.com&wctx=rm%3d0%26id%3dpassive%26ru%3d%252f&wct=2012-12-04T20%3a03%3a25Z";
				}
				this.foseLink.NavigateUrl = text;
				return;
			}
			this.foseLink.Visible = false;
		}

		protected HyperLink foseLink;
	}
}
