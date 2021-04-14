using System;
using System.Web;
using System.Web.Configuration;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.HttpProxy
{
	public class RpcHttpRequestHandler : IHttpHandler
	{
		internal RpcHttpRequestHandler() : this(RpcHttpProxyRules.DefaultRpcHttpProxyRules)
		{
		}

		internal RpcHttpRequestHandler(RpcHttpProxyRules rule)
		{
			this.proxyRules = rule;
		}

		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

		private bool AllowDiagnostics
		{
			get
			{
				string value = WebConfigurationManager.AppSettings["EnableDiagnostics"];
				bool result;
				bool.TryParse(value, out result);
				return result;
			}
		}

		public static bool CanHandleRequest(HttpRequest request)
		{
			return string.IsNullOrEmpty(request.Url.Query) || !RpcHttpRequestHandler.IsRpcProxyRequest(request) || RpcHttpRequestHandler.IsProxyPreAuthenticationRequest(request) || RpcHttpRequestHandler.IsHttpProxyRequest(request);
		}

		public void ProcessRequest(HttpContext context)
		{
			if (!context.Request.IsAuthenticated)
			{
				context.Response.StatusCode = 401;
				return;
			}
			if (RpcHttpRequestHandler.IsProxyPreAuthenticationRequest(context.Request) || RpcHttpRequestHandler.IsHttpProxyRequest(context.Request))
			{
				context.Response.StatusCode = 400;
				context.Response.StatusDescription = "Detected request from another HttpProxy";
				return;
			}
			if (RpcHttpRequestHandler.IsRpcProxyRequest(context.Request) && string.IsNullOrEmpty(context.Request.Url.Query))
			{
				context.Response.StatusCode = 200;
				return;
			}
			if (context.Request.Url.AbsolutePath.StartsWith("/rpc/diagnostics/", StringComparison.OrdinalIgnoreCase) && this.AllowDiagnostics)
			{
				this.ProcessDiagnosticsRequest(context);
				return;
			}
			context.Response.StatusCode = 404;
		}

		private static bool IsProxyPreAuthenticationRequest(HttpRequest request)
		{
			return string.Equals(request.Headers[WellKnownHeader.PreAuthRequest], "true", StringComparison.OrdinalIgnoreCase);
		}

		private static bool IsHttpProxyRequest(HttpRequest request)
		{
			return string.Equals(request.Headers[Constants.XIsFromCafe], Constants.IsFromCafeHeaderValue, StringComparison.OrdinalIgnoreCase);
		}

		private static bool IsRpcProxyRequest(HttpRequest request)
		{
			return string.Equals(request.Url.AbsolutePath, "/rpc/rpcproxy.dll", StringComparison.OrdinalIgnoreCase) || string.Equals(request.Url.AbsolutePath, "/rpcwithcert/rpcproxy.dll", StringComparison.OrdinalIgnoreCase);
		}

		private void ProcessDiagnosticsRequest(HttpContext context)
		{
			if (string.Equals(context.Request.Url.AbsolutePath, "/rpc/diagnostics/", StringComparison.OrdinalIgnoreCase))
			{
				context.Response.Output.WriteLine("<HTML><BODY>");
				context.Response.Output.WriteLine("<A HREF=\"proxyrules.txt\">Proxy Rules</A>");
				context.Response.Output.WriteLine("</BODY></HTML>");
				return;
			}
			if (string.Equals(context.Request.Url.AbsolutePath, "/rpc/diagnostics/proxyrules.txt", StringComparison.OrdinalIgnoreCase))
			{
				context.Response.AddHeader("Content-Type", "text/plain");
				context.Response.AddHeader("Cache-Control", "no-cache");
				context.Response.Output.WriteLine(this.proxyRules.ToString());
				return;
			}
			context.Response.StatusCode = 404;
		}

		private const string AppSettingsEnableDiagnostics = "EnableDiagnostics";

		private const string RpcProxyPath = "/rpc/rpcproxy.dll";

		private const string RpcWithCertProxyPath = "/rpcwithcert/rpcproxy.dll";

		private const string DiagnosticsPathBase = "/rpc/diagnostics/";

		private readonly RpcHttpProxyRules proxyRules;
	}
}
