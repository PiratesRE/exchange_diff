using System;
using System.Web;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class ProxyLogonHandler : IHttpHandler
	{
		bool IHttpHandler.IsReusable
		{
			get
			{
				return true;
			}
		}

		void IHttpHandler.ProcessRequest(HttpContext context)
		{
			context.Response.StatusCode = 241;
		}
	}
}
