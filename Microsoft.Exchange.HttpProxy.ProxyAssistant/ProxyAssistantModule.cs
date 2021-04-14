using System;
using System.Web;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.HttpProxy.Common;

namespace Microsoft.Exchange.HttpProxy.ProxyAssistant
{
	internal class ProxyAssistantModule : IHttpModule
	{
		public ProxyAssistantModule() : this(HttpProxySettings.ProxyAssistantEnabled.Value, null)
		{
		}

		internal ProxyAssistantModule(bool isEnabled, IProxyAssistantDiagnostics diagnostics)
		{
			this.isEnabled = isEnabled;
			this.diagnostics = diagnostics;
		}

		public void Init(HttpApplication application)
		{
			application.PostAuthorizeRequest += delegate(object sender, EventArgs args)
			{
				this.OnPostAuthorizeRequest(new HttpContextWrapper(((HttpApplication)sender).Context));
			};
			application.PreSendRequestHeaders += delegate(object sender, EventArgs args)
			{
				this.OnPreSendRequestHeaders(new HttpContextWrapper(((HttpApplication)sender).Context));
			};
		}

		public void Dispose()
		{
		}

		internal void OnPostAuthorizeRequest(HttpContextBase context)
		{
			if (!this.isEnabled)
			{
				return;
			}
			Diagnostics.SendWatsonReportOnUnhandledException(delegate()
			{
				if (NativeProxyHelper.CanNativeProxyHandleRequest(context.ApplicationInstance.Context))
				{
					NativeProxyHelper.UpdateRequestHeaders(context.ApplicationInstance.Context);
				}
			}, new Diagnostics.LastChanceExceptionHandler(this.LastChanceExceptionHandler));
		}

		internal void OnPreSendRequestHeaders(HttpContextBase context)
		{
			if (!this.isEnabled)
			{
				return;
			}
			Diagnostics.SendWatsonReportOnUnhandledException(delegate()
			{
				if (NativeProxyHelper.CanNativeProxyHandleRequest(context.ApplicationInstance.Context) && context.Response.StatusCode == 200)
				{
					this.AddCookiesToClientResponse(context);
				}
			}, new Diagnostics.LastChanceExceptionHandler(this.LastChanceExceptionHandler));
		}

		private IProxyAssistantDiagnostics GetDiagnostics(HttpContextBase context)
		{
			IProxyAssistantDiagnostics result;
			if (this.diagnostics == null)
			{
				result = new ProxyAssistantDiagnostics(context);
			}
			else
			{
				result = this.diagnostics;
			}
			return result;
		}

		private void LastChanceExceptionHandler(Exception ex)
		{
			IProxyAssistantDiagnostics proxyAssistantDiagnostics = this.GetDiagnostics(new HttpContextWrapper(HttpContext.Current));
			if (proxyAssistantDiagnostics != null)
			{
				proxyAssistantDiagnostics.LogUnhandledException(ex);
			}
		}

		private void AddCookiesToClientResponse(HttpContextBase context)
		{
			if (HttpProxyGlobals.ProtocolType == ProtocolType.Ews)
			{
				HttpRequestBase request = context.Request;
				HttpResponseBase response = context.Response;
				string value = response.Headers["X-FromBackend-ServerAffinity"];
				if (!string.IsNullOrEmpty(value) && request.Cookies["X-BackEndOverrideCookie"] == null)
				{
					string text = request.Headers["X-ProxyTargetServer"];
					string s = request.Headers["X-ProxyTargetServerVersion"];
					int version = 0;
					if (!string.IsNullOrWhiteSpace(text) && int.TryParse(s, out version))
					{
						BackEndServer backEndServer = new BackEndServer(text, version);
						HttpCookie httpCookie = new HttpCookie("X-BackEndOverrideCookie", backEndServer.ToString());
						httpCookie.HttpOnly = true;
						httpCookie.Secure = request.IsSecureConnection;
						response.Cookies.Add(httpCookie);
					}
				}
			}
		}

		private readonly bool isEnabled;

		private readonly IProxyAssistantDiagnostics diagnostics;
	}
}
