using System;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class ProxyLogonRequest : ProxyProtocolRequest
	{
		internal void BeginSend(OwaContext owaContext, HttpRequest originalRequest, SerializedClientSecurityContext serializedContext, AsyncCallback callback, object extraData)
		{
			ExTraceGlobals.ProxyCallTracer.TraceDebug(0L, "ProxyLogonRequest.BeginSend");
			string proxyRequestBody = serializedContext.Serialize();
			base.BeginSend(owaContext, originalRequest, OwaUrl.ProxyLogon.GetExplicitUrl(owaContext), proxyRequestBody, callback, extraData);
		}
	}
}
