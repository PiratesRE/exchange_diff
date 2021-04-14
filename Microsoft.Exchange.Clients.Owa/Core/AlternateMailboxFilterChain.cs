using System;
using System.Web;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class AlternateMailboxFilterChain : RequestFilterChain
	{
		internal static bool IsExplicitLogon(string appVdir, string requestVirtualPath, string requestRawUrl, out bool endsWithSlash, out string alternateMailboxSmtpAddress, out string updatedRequestUrl)
		{
			if (appVdir == null)
			{
				throw new ArgumentNullException("appVdir");
			}
			if (requestVirtualPath == null)
			{
				throw new ArgumentNullException("requestVirtualPath");
			}
			if (requestRawUrl == null)
			{
				throw new ArgumentNullException("requestRawUrl");
			}
			bool flag = false;
			alternateMailboxSmtpAddress = string.Empty;
			updatedRequestUrl = appVdir;
			int num = appVdir.Length + 1;
			endsWithSlash = false;
			int length = requestVirtualPath.Length;
			int num2 = length - 1;
			for (int i = num; i < length; i++)
			{
				if (i != num && requestVirtualPath[i] == '@')
				{
					flag = true;
				}
				if (requestVirtualPath[i] == '/')
				{
					endsWithSlash = true;
					num2 = i - 1;
					break;
				}
			}
			if (flag)
			{
				string text = appVdir;
				if (text.Length == 1 && text[0] == '/')
				{
					text = string.Empty;
				}
				if (endsWithSlash)
				{
					updatedRequestUrl = text + requestVirtualPath.Substring(num2 + 1);
				}
				alternateMailboxSmtpAddress = requestVirtualPath.Substring(num, num2 - num + 1);
			}
			return flag;
		}

		internal static bool FilterAlternateRequest(HttpContext httpContext)
		{
			HttpRequest request = httpContext.Request;
			bool flag2;
			string value;
			string path;
			bool flag = AlternateMailboxFilterChain.IsExplicitLogon(HttpRuntime.AppDomainAppVirtualPath, request.Path, request.RawUrl, out flag2, out value, out path);
			if (flag)
			{
				if (!flag2)
				{
					httpContext.Response.Redirect(request.RawUrl + "/", false);
					httpContext.ApplicationInstance.CompleteRequest();
					return true;
				}
				httpContext.Request.Headers.Add("X-OWA-ExplicitLogonUser", value);
				httpContext.RewritePath(path);
			}
			return false;
		}

		internal override bool FilterRequest(object source, EventArgs e, RequestEventType eventType)
		{
			HttpApplication httpApplication = (HttpApplication)source;
			HttpContext context = httpApplication.Context;
			return AlternateMailboxFilterChain.FilterAlternateRequest(context);
		}
	}
}
