using System;
using System.Collections.Specialized;
using System.Web;
using Microsoft.Exchange.Configuration.DiagnosticsModules.EventLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.DiagnosticsModules;

namespace Microsoft.Exchange.Configuration.DiagnosticsModules
{
	public class ClientDiagnosticsModule : IHttpModule
	{
		void IHttpModule.Init(HttpApplication application)
		{
			application.BeginRequest += ClientDiagnosticsModule.OnBeginRequest;
		}

		void IHttpModule.Dispose()
		{
		}

		private static void OnBeginRequest(object source, EventArgs args)
		{
			Logger.EnterFunction(ExTraceGlobals.ClientDiagnosticsModuleTracer, "ClientDiagnosticsModule.OnBeginRequest");
			HttpContext httpContext = HttpContext.Current;
			string text = httpContext.Request.Headers[ClientDiagnosticsModule.MsExchProxyUri];
			if (string.IsNullOrEmpty(text))
			{
				ClientDiagnosticsModule.LogVerbose("Get orginal url directly since it is not come from CAFE. Request url: {0}", new object[]
				{
					httpContext.Request.Url
				});
				text = httpContext.Request.Url.ToString();
			}
			NameValueCollection urlProperties = DiagnosticsHelper.GetUrlProperties(new Uri(text));
			if (ClientDiagnosticsModule.NeedAddDiagnostics(urlProperties))
			{
				string value = urlProperties["BEServer"];
				if (ClientDiagnosticsModule.AddBEServerInformationIfNeeded(ref value))
				{
					UriBuilder uriBuilder = new UriBuilder(text);
					urlProperties["BEServer"] = value;
					uriBuilder.Query = urlProperties.ToString().Replace("&", ";").Trim();
					string text2 = uriBuilder.ToString();
					HttpLogger.SafeAppendGenericInfo("DiagRedirect", text + " TO " + text2);
					ClientDiagnosticsModule.LogVerbose("Add diagnositcs information to request: Orginal url is {0}, Redirect url is {1}.", new object[]
					{
						text,
						text2
					});
					Logger.LogEvent(ClientDiagnosticsModule.eventLogger, TaskEventLogConstants.Tuple_ClientDiagnostics_RedirectWithDiagnosticsInformation, null, new object[]
					{
						text,
						httpContext.Server.UrlDecode(text2)
					});
					httpContext.Response.Redirect(text2);
				}
			}
			Logger.ExitFunction(ExTraceGlobals.ClientDiagnosticsModuleTracer, "ClientDiagnosticsModule.OnBeginRequest");
		}

		private static bool NeedAddDiagnostics(NameValueCollection urlProperties)
		{
			string a = urlProperties.Get("diag");
			return string.Equals(a, "true", StringComparison.OrdinalIgnoreCase);
		}

		private static bool AddBEServerInformationIfNeeded(ref string beServers)
		{
			if (string.IsNullOrEmpty(beServers))
			{
				beServers = string.Format("{0}", Environment.MachineName);
				return true;
			}
			string[] array = beServers.Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length > 0 && array[array.Length - 1].Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			beServers = string.Format("{0},{1}", beServers, Environment.MachineName);
			return true;
		}

		private static void LogVerbose(string message, params object[] args)
		{
			Logger.TraceInformation(ExTraceGlobals.ClientDiagnosticsModuleTracer, message, args);
		}

		internal const string DiagnosticsSwitch = "diag";

		internal const string BEServerKey = "BEServer";

		private static readonly string MsExchProxyUri = "msExchProxyUri";

		private static readonly ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.ClientDiagnosticsModuleTracer.Category, "MSExchange Client Diagnostics Module", "MSExchange Management");
	}
}
