using System;
using System.Web;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class ProxyPingHandler : IHttpHandler
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
			context.Response.AppendHeader("msExchEcpVersion", ThemeResource.ApplicationVersion);
			context.Response.StatusCode = 200;
		}
	}
}
