using System;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class OwaModule : IHttpModule
	{
		public void Init(HttpApplication context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaModule.Init");
			if (!OwaModule.isInitialized)
			{
				lock (OwaModule.initializationLock)
				{
					if (!OwaModule.isInitialized)
					{
						if (!string.IsNullOrEmpty(HttpRuntime.AppDomainAppId) && HttpRuntime.AppDomainAppId.EndsWith("calendar", StringComparison.CurrentCultureIgnoreCase))
						{
							this.owaApplication = new CalendarVDirApplication();
						}
						else
						{
							this.owaApplication = new OwaApplication();
						}
						this.owaApplication.ExecuteApplicationStart(null, null);
						OwaModule.isInitialized = true;
					}
				}
			}
			if (Globals.OwaVDirType == OWAVDirType.Calendar)
			{
				this.requestInspector = new CalendarVDirRequestEventInspector();
			}
			else
			{
				this.requestInspector = new OwaRequestEventInspector();
			}
			this.requestInspector.Init();
			context.Error += this.OnError;
			context.PostAuthorizeRequest += this.OnPostAuthorizeRequest;
			context.PreRequestHandlerExecute += this.OnPreRequestHandlerExecute;
			context.BeginRequest += this.OnBeginRequest;
			context.EndRequest += this.OnEndRequest;
			context.AuthenticateRequest += this.OnAuthenticateRequest;
		}

		public void Dispose()
		{
			if (this.owaApplication != null)
			{
				this.owaApplication.ExecuteApplicationEnd(null, null);
			}
		}

		private void OnBeginRequest(object sender, EventArgs e)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaModule.OnBeginRequest");
		}

		private void OnAuthenticateRequest(object sender, EventArgs e)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaModule.OnAuthenticateRequest");
			HttpApplication httpApplication = (HttpApplication)sender;
			if (this.ShouldInterceptRequest(httpApplication.Context, false))
			{
				HttpContext context = httpApplication.Context;
				if (UrlUtilities.IsWacRequest(httpApplication.Context.Request))
				{
					context.User = new WindowsPrincipal(WindowsIdentity.GetAnonymous());
					context.SkipAuthorization = true;
				}
				this.WriteOutlookSessionCookieIfNeeded(context);
				OwaContext owaContext = OwaContext.Create(context);
				OwaContext.Set(context, owaContext);
				bool flag = false;
				this.requestInspector.OnBeginRequest(sender, e, out flag);
				if (flag)
				{
					return;
				}
				if (Globals.FilterETag && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.FilterETag.Enabled)
				{
					context.Request.Headers.Remove("ETag");
				}
				ExTraceGlobals.RequestTracer.TraceDebug<string, string>((long)owaContext.GetHashCode(), "Request: {0} {1}", context.Request.HttpMethod, context.Request.Url.LocalPath);
				owaContext.TraceRequestId = Trace.TraceCasStart(CasTraceEventType.Owa);
				string arg;
				if (!Utilities.ValidateRequest(context, out arg))
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Request is invalid, will not continue processing. Reason = {0}", arg);
					Utilities.EndResponse(context, HttpStatusCode.BadRequest);
				}
				if (!Globals.IsInitialized)
				{
					ExTraceGlobals.CoreTracer.TraceError(0L, "Can't process this request because the application wasn't succesfully initialized");
					lock (this.firstFailedInitRequestLock)
					{
						if (this.firstFailedInitRequest != null)
						{
							if (ExDateTime.UtcNow.Subtract(this.firstFailedInitRequest.Value).TotalMilliseconds > 30000.0)
							{
								OwaDiagnostics.Logger.LogEvent(ClientsEventLogConstants.Tuple_OwaRestartingAfterFailedLoad, string.Empty, new object[0]);
								AppDomain.Unload(Thread.GetDomain());
							}
						}
						else
						{
							this.firstFailedInitRequest = new ExDateTime?(ExDateTime.UtcNow);
						}
					}
					Utilities.RewritePathToError(owaContext, LocalizedStrings.GetNonEncoded(-1556449487), Globals.InitializationError.Message);
					return;
				}
			}
			else if (this.IsBasicRequest(httpApplication.Context.Request))
			{
				Utilities.EndResponse(httpApplication.Context, HttpStatusCode.BadRequest);
			}
		}

		private void OnPostAuthorizeRequest(object sender, EventArgs e)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaModule.OnPostAuthorizeRequest");
			HttpApplication httpApplication = (HttpApplication)sender;
			if (this.ShouldInterceptRequest(httpApplication.Context, false))
			{
				if (!Globals.IsInitialized)
				{
					return;
				}
				this.requestInspector.OnPostAuthorizeRequest(sender, e);
				if (httpApplication.Context.Response.Headers["X-Frame-Options"] == null && !RequestContextHelper.IsSuiteServiceProxyRequestType(httpApplication))
				{
					httpApplication.Context.Response.Headers.Set("X-Frame-Options", "SAMEORIGIN");
				}
			}
		}

		private void OnPreRequestHandlerExecute(object sender, EventArgs e)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaModule.OnPreRequestHandlerExecute");
			HttpApplication httpApplication = (HttpApplication)sender;
			if (this.ShouldInterceptRequest(httpApplication.Context, false))
			{
				if (!Globals.IsInitialized)
				{
					return;
				}
				this.requestInspector.OnPreRequestHandlerExecute(OwaContext.Current);
			}
		}

		private void OnEndRequest(object sender, EventArgs e)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaModule.OnEndRequest");
			HttpApplication httpApplication = (HttpApplication)sender;
			if (this.ShouldInterceptRequest(httpApplication.Context, false))
			{
				if (!Globals.IsInitialized)
				{
					return;
				}
				HttpContext context = httpApplication.Context;
				OwaContext owaContext = OwaContext.Get(context);
				if (owaContext == null)
				{
					return;
				}
				try
				{
					this.requestInspector.OnEndRequest(owaContext);
				}
				finally
				{
					if (Globals.FilterETag && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.FilterETag.Enabled)
					{
						context.Response.Headers.Remove("ETag");
					}
					long requestLatencyMilliseconds = owaContext.RequestLatencyMilliseconds;
					if (Globals.OwaVDirType == OWAVDirType.OWA && Globals.ArePerfCountersEnabled)
					{
						if (RequestDispatcher.IsUserInitiatedRequest(context.Request))
						{
							PerformanceCounterManager.UpdateResponseTimePerformanceCounter(requestLatencyMilliseconds, OwaContext.Current.RequestExecution == RequestExecution.Proxy);
						}
						OwaSingleCounters.TotalRequests.Increment();
						if (owaContext.ErrorInformation != null)
						{
							OwaSingleCounters.TotalRequestsFailed.Increment();
							Exception exception = owaContext.ErrorInformation.Exception;
							this.UpdateExceptionsPerfCountersQueues(exception);
						}
						else
						{
							this.UpdateExceptionsPerfCountersQueues(null);
						}
						if (owaContext.RequestExecution == RequestExecution.Proxy)
						{
							OwaSingleCounters.ProxiedUserRequests.Increment();
						}
					}
					ExTraceGlobals.RequestTracer.TraceDebug<string, long>((long)owaContext.GetHashCode(), "Response: HTTP {0}, time:{1} ms.", owaContext.HttpContext.Response.Status, requestLatencyMilliseconds);
					OwaDiagnostics.ClearThreadTracing();
				}
			}
		}

		private void OnError(object sender, EventArgs e)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaModule.OnError");
			HttpApplication httpApplication = (HttpApplication)sender;
			Exception lastError = httpApplication.Server.GetLastError();
			if (this.ShouldInterceptRequest(httpApplication.Context, true))
			{
				httpApplication.Server.ClearError();
				Utilities.HandleException(OwaContext.Get(httpApplication.Context), lastError);
			}
		}

		private bool ShouldInterceptRequest(HttpContext httpContext, bool avoidUserContextAccess = false)
		{
			if (Globals.OwaVDirType == OWAVDirType.Calendar)
			{
				return true;
			}
			if (RequestDispatcherUtilities.IsPremiumRequest(httpContext.Request))
			{
				return false;
			}
			OwaRequestType requestType = Utilities.GetRequestType(httpContext.Request);
			return requestType == OwaRequestType.LanguagePage || requestType == OwaRequestType.Attachment || requestType == OwaRequestType.WebReadyRequest || RequestDispatcherUtilities.IsDownLevelClient(httpContext, avoidUserContextAccess);
		}

		private void UpdateExceptionsPerfCountersQueues(Exception ex)
		{
			bool result = true;
			bool result2 = true;
			bool result3 = true;
			bool result4 = true;
			if (ex != null)
			{
				if (ex is MailboxOfflineException)
				{
					result = false;
				}
				else if (ex is ConnectionFailedTransientException)
				{
					result2 = false;
				}
				else if (ex is StoragePermanentException)
				{
					result3 = false;
				}
				else if (ex is StorageTransientException)
				{
					result4 = false;
				}
			}
			PerformanceCounterManager.AddMailboxOfflineExResult(result);
			PerformanceCounterManager.AddConnectionFailedTransientExResult(result2);
			PerformanceCounterManager.AddStoragePermanantExResult(result3);
			PerformanceCounterManager.AddStorageTransientExResult(result4);
		}

		private void WriteOutlookSessionCookieIfNeeded(HttpContext httpContext)
		{
			if (httpContext.Request.Cookies["OutlookSession"] == null)
			{
				HttpCookie httpCookie = new HttpCookie("OutlookSession");
				httpCookie.HttpOnly = true;
				httpCookie.Path = "/";
				httpCookie.Value = Guid.NewGuid().ToString("N");
				httpContext.Response.Cookies.Add(httpCookie);
			}
		}

		private bool IsBasicRequest(HttpRequest httpRequest)
		{
			return httpRequest.Url.LocalPath.Contains("forms/basic/");
		}

		private const int FailedInitRecycleInterval = 30000;

		private const string OutlookSessionCookieName = "OutlookSession";

		private static readonly object initializationLock = new object();

		private static bool isInitialized;

		private ExDateTime? firstFailedInitRequest;

		private object firstFailedInitRequestLock = new object();

		private RequestEventInspectorBase requestInspector;

		private OwaApplicationBase owaApplication;
	}
}
