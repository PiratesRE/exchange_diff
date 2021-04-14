using System;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class ProxyHandler : IHttpAsyncHandler, IHttpHandler
	{
		public ProxyHandler(OutboundProxySession session)
		{
			this.session = session;
		}

		public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback requestCompletedCallback, object requestCompletedData)
		{
			ExTraceGlobals.ProxyTracer.TraceInformation<string, string>(0, 0L, "BeginProcessRequest: {0} {1}", context.Request.RequestType, context.GetRequestUrlForLog());
			context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			context.Response.Cache.SetNoStore();
			return this.session.BeginSendOutboundProxyRequest(context, requestCompletedCallback, requestCompletedData);
		}

		public void EndProcessRequest(IAsyncResult result)
		{
			ExTraceGlobals.ProxyTracer.TraceInformation(0, 0L, "EndProcessRequest");
			this.session.EndSendOutboundProxyRequest(result);
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			this.EndProcessRequest(this.BeginProcessRequest(context, null, null));
		}

		private OutboundProxySession session;
	}
}
