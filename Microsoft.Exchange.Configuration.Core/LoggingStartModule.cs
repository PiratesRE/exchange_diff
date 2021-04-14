using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.Configuration.Core
{
	public class LoggingStartModule : IHttpModule
	{
		static LoggingStartModule()
		{
			InitializeLoggerSettingsHelper.InitLoggerSettings();
			RequestMonitor.InitRequestMonitor(Path.Combine(RpsHttpLogger.LogPath.Value, "RequestMonitor"), 300000);
		}

		void IHttpModule.Init(HttpApplication context)
		{
			SettingOverrideSync.Instance.Start(true);
			context.BeginRequest += this.OnBeginRequest;
		}

		void IHttpModule.Dispose()
		{
			SettingOverrideSync.Instance.Stop();
		}

		private void OnBeginRequest(object sender, EventArgs e)
		{
			ExTraceGlobals.HttpModuleTracer.TraceFunction((long)this.GetHashCode(), "[LoggingStartModule::OnBeginRequest] Enter");
			LatencyTracker latencyTracker = new LatencyTracker(Constants.IsPowerShellWebService ? "Psws.HttpModule" : "Rps.HttpModule", null);
			latencyTracker.Start();
			HttpContext httpContext = HttpContext.Current;
			HttpRequest request = httpContext.Request;
			NameValueCollection headers = httpContext.Request.Headers;
			httpContext.Items["Logging-HttpRequest-Latency"] = latencyTracker;
			HttpLogger.InitializeRequestLogger();
			if (HttpLogger.ActivityScope != null)
			{
				HttpLogger.ActivityScope.UpdateFromMessage(HttpContext.Current.Request);
				HttpLogger.ActivityScope.SerializeTo(HttpContext.Current.Response);
			}
			if (Constants.IsRemotePS && HttpLogger.ActivityScope != null && request.Url.Port == 444)
			{
				Uri url = request.Url;
				NameValueCollection urlProperties = url.GetUrlProperties();
				string text = urlProperties["RequestId48CD6591-0506-4D6E-9131-797489A3260F"];
				Guid guid;
				if (text != null && Guid.TryParse(text, out guid))
				{
					LoggerHelper.UpdateActivityScopeRequestIdFromUrl(url.ToString());
				}
			}
			HttpLogger.SafeSetLogger(ServiceCommonMetadata.HttpMethod, request.HttpMethod);
			WinRMInfo winRMInfoFromHttpHeaders = WinRMInfo.GetWinRMInfoFromHttpHeaders(headers);
			if (winRMInfoFromHttpHeaders != null)
			{
				HttpLogger.SafeSetLogger(RpsHttpMetadata.Action, winRMInfoFromHttpHeaders.Action);
				HttpLogger.SafeSetLogger(RpsCommonMetadata.SessionId, winRMInfoFromHttpHeaders.SessionId);
				HttpLogger.SafeSetLogger(RpsHttpMetadata.ShellId, winRMInfoFromHttpHeaders.ShellId);
				HttpLogger.SafeSetLogger(RpsHttpMetadata.CommandId, winRMInfoFromHttpHeaders.CommandId);
				HttpLogger.SafeSetLogger(RpsHttpMetadata.CommandName, winRMInfoFromHttpHeaders.CommandName);
				httpContext.Items["X-RemotePS-WinRMInfo"] = winRMInfoFromHttpHeaders;
			}
			string sourceCafeServer = CafeHelper.GetSourceCafeServer(request);
			if (sourceCafeServer != null)
			{
				HttpLogger.SafeSetLogger(ConfigurationCoreMetadata.FrontEndServer, sourceCafeServer);
			}
			Uri url2 = request.Url;
			if (url2.IsAbsoluteUri)
			{
				HttpLogger.SafeSetLogger(ConfigurationCoreMetadata.UrlHost, url2.DnsSafeHost);
				HttpLogger.SafeSetLogger(ConfigurationCoreMetadata.UrlStem, url2.LocalPath);
				HttpLogger.SafeSetLogger(ConfigurationCoreMetadata.UrlQuery, url2.Query);
			}
			else
			{
				HttpLogger.SafeAppendGenericError("InvalidRelativeUri", url2.ToString(), false);
			}
			NameValueCollection urlProperties2 = url2.GetUrlProperties();
			if (urlProperties2 != null)
			{
				string text2;
				if (!string.IsNullOrWhiteSpace(text2 = urlProperties2["Organization"]))
				{
					HttpLogger.SafeSetLogger(ConfigurationCoreMetadata.ManagedOrganization, text2);
				}
				else if (!string.IsNullOrWhiteSpace(text2 = urlProperties2["DelegatedOrg"]))
				{
					HttpLogger.SafeSetLogger(ConfigurationCoreMetadata.ManagedOrganization, "Delegate:" + text2);
				}
			}
			if (HttpLogger.ActivityScope != null)
			{
				using (new MonitoredScope("RequestMonitor.Register", "RequestMonitor.Register", HttpModuleHelper.HttpPerfMonitors))
				{
					RequestMonitor.Instance.RegisterRequest(HttpLogger.ActivityScope.ActivityId);
					string text3 = headers[WellKnownHeader.WLIDMemberName] ?? headers[WellKnownHeader.LiveIdMemberName];
					if (!string.IsNullOrEmpty(text3))
					{
						SmtpAddress smtpAddress = new SmtpAddress(text3);
						RequestMonitor.Instance.Log(HttpLogger.ActivityScope.ActivityId, RequestMonitorMetadata.AuthenticatedUser, text3);
						RequestMonitor.Instance.Log(HttpLogger.ActivityScope.ActivityId, RequestMonitorMetadata.Organization, smtpAddress.Domain);
					}
					RequestMonitor.Instance.Log(HttpLogger.ActivityScope.ActivityId, RequestMonitorMetadata.FrontEndServer, sourceCafeServer);
					RequestMonitor.Instance.Log(HttpLogger.ActivityScope.ActivityId, RequestMonitorMetadata.ProtocolAction, HttpLogger.ActivityScope.GetProperty(RpsHttpMetadata.Action));
				}
			}
			ExTraceGlobals.HttpModuleTracer.TraceFunction((long)this.GetHashCode(), "[LoggingStartModule::OnBeginRequest] Exit");
		}

		internal const string HttpStopWatchContextItemKey = "Logging-HttpRequest-Latency";
	}
}
