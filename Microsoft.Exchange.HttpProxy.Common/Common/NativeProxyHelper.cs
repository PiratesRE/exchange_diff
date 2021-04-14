using System;
using System.Web;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.HttpProxy.Common
{
	internal static class NativeProxyHelper
	{
		public static bool CanNativeProxyHandleRequest(HttpContext httpContext)
		{
			ArgumentValidator.ThrowIfNull("httpContext", httpContext);
			if (HttpProxySettings.NativeProxyEnabled.Value)
			{
				string value = httpContext.Request.Headers["X-ProxyTargetServer"];
				if (!string.IsNullOrEmpty(value))
				{
					return true;
				}
				if (!string.IsNullOrEmpty(NativeProxyHelper.NativeProxyTargetServerOverride.Value))
				{
					return true;
				}
			}
			return false;
		}

		public static void UpdateRequestHeaders(HttpContext httpContext)
		{
			ArgumentValidator.ThrowIfNull("httpContext", httpContext);
			string fullRawUrl = httpContext.Request.GetFullRawUrl();
			try
			{
				httpContext.Request.Headers[Constants.MsExchProxyUri] = fullRawUrl;
			}
			catch (ArgumentException)
			{
				httpContext.Request.Headers[Constants.MsExchProxyUri] = Uri.EscapeUriString(fullRawUrl);
			}
			httpContext.Request.Headers[Constants.XIsFromCafe] = Constants.IsFromCafeHeaderValue;
			httpContext.Request.Headers[Constants.XSourceCafeServer] = HttpProxyGlobals.LocalMachineFqdn.Member;
			httpContext.Request.Headers[CafeHelper.CafeProxyHandler] = CafeHelper.NativeHttpProxy;
			httpContext.Request.Headers[Constants.OriginatingClientIpHeader] = AspNetHelper.GetClientIpAsProxyHeader(httpContext.Request);
			httpContext.Request.Headers[Constants.OriginatingClientPortHeader] = AspNetHelper.GetClientPortAsProxyHeader(httpContext);
			string value;
			if (HttpProxyRegistry.AreGccStoredSecretKeysValid.Member && (GccUtils.TryGetGccProxyInfo(httpContext, out value) || GccUtils.TryCreateGccProxyInfo(httpContext, out value)) && !string.IsNullOrEmpty(value))
			{
				httpContext.Request.Headers.Add("X-GCC-PROXYINFO", value);
			}
			string value2 = httpContext.Items["WLID-MemberName"] as string;
			if (!string.IsNullOrEmpty(value2))
			{
				httpContext.Request.Headers[Constants.WLIDMemberNameHeaderName] = value2;
				httpContext.Request.Headers[Constants.LiveIdMemberName] = value2;
			}
			string text = httpContext.Request.Url.PathAndQuery;
			if (HttpProxyGlobals.ProtocolType == ProtocolType.RpcHttp)
			{
				RpcHttpQueryString rpcHttpQueryString = new RpcHttpQueryString(httpContext.Request.Url.Query);
				if (!string.IsNullOrEmpty(rpcHttpQueryString.RcaServer))
				{
					httpContext.Request.Headers[WellKnownHeader.RpcHttpProxyServerTarget] = rpcHttpQueryString.RcaServer;
				}
			}
			else if (HttpProxyGlobals.ProtocolType == ProtocolType.Ews)
			{
				bool selectedNodeIsLast;
				string explicitLogonNode = ProtocolHelper.GetExplicitLogonNode(httpContext.Request.ApplicationPath, httpContext.Request.FilePath, ExplicitLogonNode.Second, out selectedNodeIsLast);
				string text2;
				if (ProtocolHelper.TryGetValidNormalizedExplicitLogonAddress(explicitLogonNode, selectedNodeIsLast, out text2))
				{
					Uri clientUrlForProxy = ProtocolHelper.GetClientUrlForProxy(httpContext.Request.Url, explicitLogonNode);
					text = clientUrlForProxy.PathAndQuery;
				}
			}
			else if (HttpProxyGlobals.ProtocolType == ProtocolType.Eas)
			{
				text = text.Replace("Microsoft-Server-ActiveSync", "Microsoft-Server-ActiveSync/Proxy");
			}
			httpContext.Request.ServerVariables["X-ProxyTargetUrlAbsPath"] = text;
		}

		private static readonly StringAppSettingsEntry NativeProxyTargetServerOverride = new StringAppSettingsEntry("NativeHttpProxy.ProxyTargetServerOverride", string.Empty, ExTraceGlobals.VerboseTracer);
	}
}
