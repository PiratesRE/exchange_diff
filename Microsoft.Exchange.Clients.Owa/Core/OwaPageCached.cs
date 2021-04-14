using System;
using System.Security.Permissions;
using System.Web;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
	[AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class OwaPageCached : OwaPage
	{
		public OwaPageCached() : base(false)
		{
		}

		protected static int StoreObjectTypeCalendarItem
		{
			get
			{
				return 15;
			}
		}

		protected static int StoreObjectTypeMessage
		{
			get
			{
				return 9;
			}
		}

		protected override bool IsTextHtml
		{
			get
			{
				return false;
			}
		}

		protected override void OnInit(EventArgs e)
		{
			Utilities.MakePageCacheable(base.Response);
			base.OnInit(e);
		}
	}
}
