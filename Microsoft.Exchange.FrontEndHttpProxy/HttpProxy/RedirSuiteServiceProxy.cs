using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.HttpProxy
{
	public class RedirSuiteServiceProxy : OwaPage
	{
		protected override void OnLoad(EventArgs e)
		{
			string value = base.Request.Headers["Host"];
			string text = base.Request.QueryString["returnUrl"];
			if (!string.IsNullOrEmpty(text))
			{
				string script = string.Format("window.top.location = 'https://{0}/owa/InitSuiteServiceProxy.aspx?exsvurl=1&realm=office365.com&returnUrl={1}'", HttpUtility.JavaScriptStringEncode(value), HttpUtility.JavaScriptStringEncode(HttpUtility.UrlEncode(text)));
				base.ClientScript.RegisterClientScriptBlock(base.GetType(), "Redir", script, true);
			}
		}
	}
}
