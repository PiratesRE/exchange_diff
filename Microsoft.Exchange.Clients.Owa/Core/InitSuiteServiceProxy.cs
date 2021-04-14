using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using Microsoft.Exchange.Clients.Owa2.Server.Core;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class InitSuiteServiceProxy : Page
	{
		protected override void OnLoad(EventArgs e)
		{
			HttpCookie httpCookie = new HttpCookie("SuiteServiceProxyInit", "true");
			httpCookie.Expires = new DateTime(9999, 12, 31);
			base.Response.Cookies.Add(httpCookie);
			string text = base.Request.QueryString["returnUrl"];
			if (!string.IsNullOrEmpty(text) && this.IsRedirectAllowed(text))
			{
				base.Response.Redirect(text, false);
			}
		}

		protected bool IsRedirectAllowed(string returnUrl)
		{
			bool result = false;
			Uri uri = new Uri(returnUrl);
			if (uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
			{
				SuiteServiceProxyHelper suiteServiceProxyHelper = new SuiteServiceProxyHelper();
				string[] suiteServiceProxyOriginAllowedList = suiteServiceProxyHelper.GetSuiteServiceProxyOriginAllowedList();
				foreach (string pattern in suiteServiceProxyOriginAllowedList)
				{
					Regex regex = new Regex(pattern);
					if (regex.IsMatch(uri.Authority))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}
	}
}
