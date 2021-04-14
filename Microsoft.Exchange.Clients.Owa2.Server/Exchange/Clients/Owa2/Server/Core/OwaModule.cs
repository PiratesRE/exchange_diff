using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public sealed class OwaModule : IHttpModule
	{
		public void Init(HttpApplication context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OWA Server -- OwaModule.Init");
			if (Globals.IsAnonymousCalendarApp)
			{
				this.requestHandler = new CalendarRequestHandler();
			}
			else
			{
				this.requestHandler = new OwaRequestHandler();
			}
			context.Error += this.OnError;
			context.BeginRequest += this.OnBeginRequest;
			context.AuthenticateRequest += this.OnAuthenticateRequest;
			context.PostAuthorizeRequest += this.OnPostAuthorizeRequest;
			context.PreRequestHandlerExecute += this.OnPreRequestHandlerExecute;
			context.EndRequest += this.OnEndRequest;
			context.PreSendRequestHeaders += this.OnPreSendRequestHeaders;
		}

		public void Dispose()
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OWA Server -- OwaModule.Dispose");
		}

		private void OnAuthenticateRequest(object sender, EventArgs e)
		{
			if (!Globals.IsInitialized)
			{
				return;
			}
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaModule.OnAuthenticateRequest");
			HttpApplication httpApplication = (HttpApplication)sender;
			HttpContext context = httpApplication.Context;
			if (UrlUtilities.IsWacRequest(context.Request))
			{
				context.User = new WindowsPrincipal(WindowsIdentity.GetAnonymous());
				context.SkipAuthorization = true;
			}
		}

		private void OnBeginRequest(object sender, EventArgs e)
		{
			if (!Globals.IsInitialized)
			{
				return;
			}
			HttpApplication httpApplication = (HttpApplication)sender;
			HttpContext context = httpApplication.Context;
			string cookieValueAndSetIfNull = ClientIdCookie.GetCookieValueAndSetIfNull(context);
			try
			{
				context.Response.AppendToLog(string.Format("&{0}={1}", "ClientId", cookieValueAndSetIfNull));
			}
			catch (Exception ex)
			{
				ExTraceGlobals.ExceptionTracer.TraceError((long)this.GetHashCode(), ex.Message);
			}
			context.Request.Headers["X-BackEnd-Begin"] = ExDateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
			httpApplication.Context.Response.Headers.Set("X-Content-Type-Options", "nosniff");
			RequestContext requestContext = RequestContext.Create(context);
			requestContext.Set(context);
			ExTraceGlobals.RequestTracer.TraceDebug<string, string>((long)requestContext.GetHashCode(), "Request: {0} {1}", context.Request.HttpMethod, context.Request.Url.LocalPath);
		}

		private void OnPostAuthorizeRequest(object sender, EventArgs e)
		{
			if (!Globals.IsInitialized)
			{
				return;
			}
			this.requestHandler.OnPostAuthorizeRequest(sender, e);
		}

		private void OnPreRequestHandlerExecute(object sender, EventArgs e)
		{
			if (!Globals.IsInitialized)
			{
				return;
			}
			this.requestHandler.OnPreRequestHandlerExecute(sender, e);
		}

		private void OnEndRequest(object sender, EventArgs e)
		{
			if (!Globals.IsInitialized)
			{
				return;
			}
			this.requestHandler.OnEndRequest(sender, e);
			OwaSingleCounters.TotalRequests.Increment();
		}

		private void OnPreSendRequestHeaders(object sender, EventArgs e)
		{
			if (!Globals.IsInitialized)
			{
				return;
			}
			this.requestHandler.OnPreSendRequestHeaders(sender, e);
		}

		private void OnError(object sender, EventArgs e)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaModule.OnError");
			HttpApplication httpApplication = (HttpApplication)sender;
			Exception lastError = httpApplication.Server.GetLastError();
			if (lastError == null)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "GetLastError returned null.  Bailing out.");
				return;
			}
			RequestContext requestContext = RequestContext.Current;
			if (requestContext.RequestType == OwaRequestType.Invalid)
			{
				requestContext.RequestType = RequestDispatcherUtilities.GetRequestType(requestContext.HttpContext.Request);
			}
			if (!RequestDispatcherUtilities.IsDownLevelClient(requestContext.HttpContext, true) && (RequestDispatcherUtilities.IsPremiumRequest(requestContext.HttpContext.Request) || requestContext.RequestType == OwaRequestType.Form15 || requestContext.RequestType == OwaRequestType.LanguagePost))
			{
				httpApplication.Server.ClearError();
				ErrorHandlerUtilities.HandleException(requestContext, lastError);
				return;
			}
			ErrorHandlerUtilities.RecordException(requestContext, lastError);
		}

		private RequestHandlerBase requestHandler;
	}
}
