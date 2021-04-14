using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.Net.Wopi;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.HttpProxy
{
	public class ProxyModule : IHttpModule
	{
		internal PfdTracer PfdTracer { get; set; }

		public void Init(HttpApplication application)
		{
			Diagnostics.SendWatsonReportOnUnhandledException(delegate()
			{
				LatencyTracker latencyTracker = new LatencyTracker();
				latencyTracker.StartTracking(LatencyTrackerKey.ProxyModuleInitLatency, false);
				Microsoft.Exchange.Diagnostics.Components.HttpProxy.ExTraceGlobals.VerboseTracer.TraceDebug<ProtocolType>((long)this.GetHashCode(), "[ProxyModule::Init]: Init called.  Protocol type: {0}", HttpProxyGlobals.ProtocolType);
				if (application == null)
				{
					string message = "[ProxyModule::Init]: ProxyModule.Init called with null HttpApplication context.";
					Microsoft.Exchange.Diagnostics.Components.HttpProxy.ExTraceGlobals.BriefTracer.TraceError((long)this.GetHashCode(), message);
					throw new ArgumentNullException("application", message);
				}
				this.PfdTracer = new PfdTracer(0, this.GetHashCode());
				application.BeginRequest += this.OnBeginRequest;
				application.AuthenticateRequest += this.OnAuthenticateRequest;
				application.PostAuthorizeRequest += this.OnPostAuthorizeRequest;
				application.PreSendRequestHeaders += this.OnPreSendRequestHeaders;
				application.EndRequest += this.OnEndRequest;
				if (Microsoft.Exchange.Diagnostics.Components.HttpProxy.ExTraceGlobals.VerboseTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					Microsoft.Exchange.Diagnostics.Components.HttpProxy.ExTraceGlobals.VerboseTracer.TraceDebug<ProtocolType, long>((long)this.GetHashCode(), "[ProxyModule::Init]: Protocol type: {0}, InitLatency {1}", HttpProxyGlobals.ProtocolType, latencyTracker.GetCurrentLatency(LatencyTrackerKey.ProxyModuleInitLatency));
				}
			});
		}

		public void Dispose()
		{
		}

		protected virtual void OnBeginRequestInternal(HttpApplication httpApplication)
		{
			if (HttpProxyGlobals.OnlyProxySecureConnections && !httpApplication.Request.IsSecureConnection)
			{
				AspNetHelper.TerminateRequestWithSslRequiredResponse(httpApplication);
			}
		}

		protected virtual void OnAuthenticateInternal(HttpApplication httpApplication)
		{
			HttpContext context = httpApplication.Context;
			if (this.AllowAnonymousRequest(context.Request))
			{
				context.User = new WindowsPrincipal(WindowsIdentity.GetAnonymous());
				context.SkipAuthorization = true;
			}
		}

		protected virtual void OnPostAuthorizeInternal(HttpApplication httpApplication)
		{
			HttpContext context = httpApplication.Context;
			if (NativeProxyHelper.CanNativeProxyHandleRequest(context))
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLoggerBase<RequestDetailsLogger>.GetCurrent(context), "ProxyRequestHandler", "NativeHttpProxy");
				return;
			}
			IHttpHandler httpHandler;
			if (context.Request.IsAuthenticated)
			{
				httpHandler = this.SelectHandlerForAuthenticatedRequest(context);
			}
			else
			{
				httpHandler = this.SelectHandlerForUnauthenticatedRequest(context);
			}
			if (httpHandler != null)
			{
				if (Microsoft.Exchange.Diagnostics.Components.HttpProxy.ExTraceGlobals.VerboseTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					Microsoft.Exchange.Diagnostics.Components.HttpProxy.ExTraceGlobals.VerboseTracer.TraceDebug<Type, object>((long)this.GetHashCode(), "[ProxyModule::OnPostAuthorizeInternal]: The selected HttpHandler is {0}; Context {1};", httpHandler.GetType(), context.Items[Constants.TraceContextKey]);
				}
				PerfCounters.HttpProxyCountersInstance.TotalRequests.Increment();
				if (httpHandler is ProxyRequestHandler)
				{
					((ProxyRequestHandler)httpHandler).Run(context);
				}
				else
				{
					context.RemapHandler(httpHandler);
				}
				long currentLatency = LatencyTracker.FromHttpContext(context).GetCurrentLatency(LatencyTrackerKey.ProxyModuleLatency);
				if (currentLatency > 100L)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLoggerBase<RequestDetailsLogger>.GetCurrent(context), "RemapHandler", currentLatency);
				}
			}
		}

		protected virtual void OnEndRequestInternal(HttpApplication httpApplication)
		{
		}

		protected virtual bool AllowAnonymousRequest(HttpRequest httpRequest)
		{
			return HttpProxyGlobals.ProtocolType != ProtocolType.Mapi && (WopiRequestPathHandler.IsWopiRequest(httpRequest, AuthCommon.IsFrontEnd) || AnonymousCalendarProxyRequestHandler.IsAnonymousCalendarRequest(httpRequest) || OwaExtensibilityProxyRequestHandler.IsOwaExtensibilityRequest(httpRequest) || OwaCobrandingRedirProxyRequestHandler.IsCobrandingRedirRequest(httpRequest) || E4eProxyRequestHandler.IsE4ePayloadRequest(httpRequest) || httpRequest.IsWsSecurityRequest() || PsgwProxyRequestHandler.IsPsgwRequest(httpRequest));
		}

		private static void FinalizeRequestLatencies(HttpContext httpContext, RequestDetailsLogger requestDetailsLogger, IActivityScope activityScope, LatencyTracker tracker, int traceContext)
		{
			if (tracker == null)
			{
				return;
			}
			if (requestDetailsLogger == null)
			{
				throw new ArgumentNullException("requestDetailsLogger");
			}
			if (activityScope == null)
			{
				throw new ArgumentNullException("activityScope");
			}
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			long num = tracker.GetCurrentLatency(LatencyTrackerKey.ProxyModuleLatency);
			if (num >= 0L)
			{
				long num2 = 0L;
				long.TryParse(activityScope.GetProperty(ServiceLatencyMetadata.HttpPipelineLatency), out num2);
				long num3 = 0L;
				bool flag = requestDetailsLogger.TryGetLatency(HttpProxyMetadata.BackendProcessingLatency, out num3);
				long num4 = requestDetailsLogger.GetLatency(HttpProxyMetadata.ClientRequestStreamingLatency, 0L) + requestDetailsLogger.GetLatency(HttpProxyMetadata.BackendRequestStreamingLatency, 0L) + num3 + requestDetailsLogger.GetLatency(HttpProxyMetadata.BackendResponseStreamingLatency, 0L) + requestDetailsLogger.GetLatency(HttpProxyMetadata.ClientResponseStreamingLatency, 0L);
				long num5 = num - num4;
				PerfCounters.UpdateMovingAveragePerformanceCounter(PerfCounters.HttpProxyCountersInstance.MovingAverageCasLatency, num5);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(requestDetailsLogger, HttpProxyMetadata.HttpProxyOverhead, num5);
				long num6 = num5 - num2;
				if (flag)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(requestDetailsLogger, HttpProxyMetadata.RoutingLatency, Math.Max(num6, 0L));
					string property = activityScope.GetProperty(HttpProxyMetadata.TargetServer);
					if (PerfCounters.RoutingLatenciesEnabled && !string.IsNullOrEmpty(property))
					{
						string empty = string.Empty;
						Utilities.TryGetSiteNameFromServerFqdn(property, out empty);
						PercentilePerfCounters.UpdateRoutingLatencyPerfCounter(empty, (double)num6);
						PerfCounters.GetHttpProxyPerSiteCountersInstance(empty).TotalProxyWithLatencyRequests.Increment();
					}
				}
				long val = num6 - requestDetailsLogger.GetLatency(HttpProxyMetadata.BackendRequestInitLatency, 0L) - requestDetailsLogger.GetLatency(HttpProxyMetadata.BackendResponseInitLatency, 0L);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(requestDetailsLogger, HttpProxyMetadata.CoreLatency, Math.Max(val, 0L));
				long currentLatency = tracker.GetCurrentLatency(LatencyTrackerKey.ProxyModuleLatency);
				long num7 = currentLatency - num;
				num = currentLatency;
				if (num7 > 5L)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(requestDetailsLogger, "TotalRequestTimeDelta", num7);
				}
			}
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(requestDetailsLogger, HttpProxyMetadata.TotalRequestTime, num);
		}

		private IHttpHandler SelectHandlerForAuthenticatedRequest(HttpContext httpContext)
		{
			IHttpHandler result;
			try
			{
				IHttpHandler httpHandler;
				if (HttpProxyGlobals.ProtocolType == ProtocolType.Mapi)
				{
					httpHandler = new MapiProxyRequestHandler();
				}
				else if (HttpProxyGlobals.ProtocolType == ProtocolType.Ecp)
				{
					if (SiteMailboxCreatingProxyRequestHandler.IsSiteMailboxCreatingProxyRequest(httpContext.Request))
					{
						httpHandler = new SiteMailboxCreatingProxyRequestHandler();
					}
					else if (EDiscoveryExportToolProxyRequestHandler.IsEDiscoveryExportToolProxyRequest(httpContext.Request))
					{
						httpHandler = new EDiscoveryExportToolProxyRequestHandler();
					}
					else if (BEResourceRequestHandler.CanHandle(httpContext.Request))
					{
						httpHandler = new BEResourceRequestHandler();
					}
					else
					{
						httpHandler = new EcpProxyRequestHandler();
					}
				}
				else if (HttpProxyGlobals.ProtocolType == ProtocolType.Autodiscover)
				{
					httpHandler = new AutodiscoverProxyRequestHandler();
				}
				else if (HttpProxyGlobals.ProtocolType == ProtocolType.Ews)
				{
					if (EwsUserPhotoProxyRequestHandler.IsUserPhotoRequest(httpContext.Request))
					{
						httpHandler = new EwsUserPhotoProxyRequestHandler();
					}
					else if (EwsODataProxyRequestHandler.IsODataRequest(httpContext.Request))
					{
						httpHandler = new EwsODataProxyRequestHandler();
					}
					else if (MrsProxyRequestHandler.IsMrsRequest(httpContext.Request))
					{
						httpHandler = new MrsProxyRequestHandler();
					}
					else if (MessageTrackingRequestHandler.IsMessageTrackingRequest(httpContext.Request))
					{
						httpHandler = new MessageTrackingRequestHandler();
					}
					else
					{
						httpHandler = new EwsProxyRequestHandler();
					}
				}
				else if (HttpProxyGlobals.ProtocolType == ProtocolType.RpcHttp)
				{
					if (RpcHttpRequestHandler.CanHandleRequest(httpContext.Request))
					{
						httpHandler = new RpcHttpRequestHandler();
					}
					else
					{
						httpHandler = new RpcHttpProxyRequestHandler();
					}
				}
				else if (HttpProxyGlobals.ProtocolType == ProtocolType.Eas)
				{
					httpHandler = new EasProxyRequestHandler();
				}
				else if (HttpProxyGlobals.ProtocolType == ProtocolType.Oab)
				{
					httpHandler = new OabProxyRequestHandler();
				}
				else if (HttpProxyGlobals.ProtocolType == ProtocolType.PowerShell || HttpProxyGlobals.ProtocolType == ProtocolType.PowerShellLiveId)
				{
					httpHandler = new RemotePowerShellProxyRequestHandler();
				}
				else if (HttpProxyGlobals.ProtocolType == ProtocolType.ReportingWebService)
				{
					httpHandler = new ReportingWebServiceProxyRequestHandler();
				}
				else if (HttpProxyGlobals.ProtocolType == ProtocolType.Psws)
				{
					httpHandler = new PswsProxyRequestHandler();
				}
				else if (HttpProxyGlobals.ProtocolType == ProtocolType.Xrop)
				{
					httpHandler = new XRopProxyRequestHandler();
				}
				else if (HttpProxyGlobals.ProtocolType == ProtocolType.Owa)
				{
					string absolutePath = httpContext.Request.Url.AbsolutePath;
					if (OWAUserPhotoProxyRequestHandler.IsUserPhotoRequest(httpContext.Request))
					{
						httpHandler = new OWAUserPhotoProxyRequestHandler();
					}
					else if (absolutePath.EndsWith("service.svc", StringComparison.OrdinalIgnoreCase) || absolutePath.IndexOf("/service.svc/", StringComparison.OrdinalIgnoreCase) != -1)
					{
						httpHandler = new EwsJsonProxyRequestHandler();
					}
					else if (absolutePath.EndsWith("ev.owa2", StringComparison.OrdinalIgnoreCase))
					{
						httpHandler = new OwaOeh2ProxyRequestHandler();
					}
					else if (absolutePath.EndsWith("speech.reco", StringComparison.OrdinalIgnoreCase))
					{
						httpHandler = new SpeechRecoProxyRequestHandler();
					}
					else if (absolutePath.EndsWith("lang.owa", StringComparison.OrdinalIgnoreCase))
					{
						httpHandler = new OwaLanguagePostProxyRequestHandler();
					}
					else if (absolutePath.EndsWith("ev.owa", StringComparison.OrdinalIgnoreCase) && httpContext.Request.RawUrl.IndexOf("ns=EwsProxy", StringComparison.OrdinalIgnoreCase) > 0)
					{
						httpHandler = new EwsProxyRequestHandler(true);
					}
					else
					{
						httpHandler = new OwaProxyRequestHandler();
					}
				}
				else if (HttpProxyGlobals.ProtocolType == ProtocolType.PushNotifications)
				{
					httpHandler = new PushNotificationsProxyRequestHandler();
				}
				else if (HttpProxyGlobals.ProtocolType == ProtocolType.OutlookService)
				{
					httpHandler = new OutlookServiceProxyRequestHandler();
				}
				else if (HttpProxyGlobals.ProtocolType == ProtocolType.SnackyService)
				{
					httpHandler = new SnackyServiceProxyRequestHandler();
				}
				else if (HttpProxyGlobals.ProtocolType == ProtocolType.E4e)
				{
					httpHandler = new E4eProxyRequestHandler();
				}
				else
				{
					if (HttpProxyGlobals.ProtocolType != ProtocolType.O365SuiteService)
					{
						throw new InvalidOperationException("Unknown protocol type " + HttpProxyGlobals.ProtocolType);
					}
					httpHandler = new O365SuiteServiceProxyRequestHandler();
				}
				result = httpHandler;
			}
			finally
			{
				long currentLatency = LatencyTracker.FromHttpContext(httpContext).GetCurrentLatency(LatencyTrackerKey.ProxyModuleLatency);
				if (currentLatency > 100L)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLoggerBase<RequestDetailsLogger>.GetCurrent(httpContext), "SelectHandler", currentLatency);
				}
			}
			return result;
		}

		private IHttpHandler SelectHandlerForUnauthenticatedRequest(HttpContext httpContext)
		{
			IHttpHandler result;
			try
			{
				IHttpHandler httpHandler = null;
				if (HttpProxyGlobals.ProtocolType == ProtocolType.Autodiscover)
				{
					if (httpContext.Request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase))
					{
						if (ProtocolHelper.IsOAuthMetadataRequest(httpContext.Request.Url.AbsolutePath))
						{
							httpHandler = new AuthMetadataHttpHandler(httpContext);
						}
						else if (ProtocolHelper.IsAutodiscoverV2Request(httpContext.Request.Url.AbsolutePath))
						{
							httpHandler = new AutodiscoverProxyRequestHandler();
						}
					}
					else
					{
						httpHandler = new AutodiscoverProxyRequestHandler();
					}
				}
				else if (HttpProxyGlobals.ProtocolType == ProtocolType.Ews)
				{
					string httpMethod = httpContext.Request.HttpMethod;
					if (!httpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase) && !httpMethod.Equals("HEAD", StringComparison.OrdinalIgnoreCase))
					{
						httpHandler = new EwsProxyRequestHandler();
					}
				}
				else if (HttpProxyGlobals.ProtocolType == ProtocolType.Ecp)
				{
					if (EDiscoveryExportToolProxyRequestHandler.IsEDiscoveryExportToolProxyRequest(httpContext.Request))
					{
						httpHandler = new EDiscoveryExportToolProxyRequestHandler();
					}
					else if (BEResourceRequestHandler.CanHandle(httpContext.Request))
					{
						httpHandler = new BEResourceRequestHandler();
					}
					else if (EcpProxyRequestHandler.IsCrossForestDelegatedRequest(httpContext.Request))
					{
						httpHandler = new EcpProxyRequestHandler
						{
							IsCrossForestDelegated = true
						};
					}
					else if (!httpContext.Request.Path.StartsWith("/ecp/auth/", StringComparison.OrdinalIgnoreCase) && !httpContext.Request.Path.Equals("/ecp/ping.ecp", StringComparison.OrdinalIgnoreCase))
					{
						httpHandler = new Return401RequestHandler();
					}
				}
				else if (HttpProxyGlobals.ProtocolType == ProtocolType.RpcHttp)
				{
					httpHandler = new RpcHttpRequestHandler();
				}
				else if (HttpProxyGlobals.ProtocolType == ProtocolType.Xrop)
				{
					httpHandler = new XRopProxyRequestHandler();
				}
				else if (HttpProxyGlobals.ProtocolType == ProtocolType.E4e)
				{
					httpHandler = new E4eProxyRequestHandler();
				}
				else if (AnonymousCalendarProxyRequestHandler.IsAnonymousCalendarRequest(httpContext.Request))
				{
					httpHandler = new AnonymousCalendarProxyRequestHandler();
				}
				else if (HttpProxyGlobals.ProtocolType == ProtocolType.Owa && WopiRequestPathHandler.IsWopiRequest(httpContext.Request, AuthCommon.IsFrontEnd))
				{
					httpHandler = new WopiProxyRequestHandler();
				}
				else if (OwaExtensibilityProxyRequestHandler.IsOwaExtensibilityRequest(httpContext.Request))
				{
					httpHandler = new OwaExtensibilityProxyRequestHandler();
				}
				else if (OwaCobrandingRedirProxyRequestHandler.IsCobrandingRedirRequest(httpContext.Request))
				{
					httpHandler = new OwaCobrandingRedirProxyRequestHandler();
				}
				else if (HttpProxyGlobals.ProtocolType == ProtocolType.Owa && OwaResourceProxyRequestHandler.CanHandle(httpContext.Request))
				{
					httpHandler = new OwaResourceProxyRequestHandler();
				}
				else if (HttpProxyGlobals.ProtocolType == ProtocolType.PowerShellGateway)
				{
					httpHandler = new PsgwProxyRequestHandler();
				}
				result = httpHandler;
			}
			finally
			{
				long currentLatency = LatencyTracker.FromHttpContext(httpContext).GetCurrentLatency(LatencyTrackerKey.ProxyModuleLatency);
				if (currentLatency > 100L)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLoggerBase<RequestDetailsLogger>.GetCurrent(httpContext), "SelectHandler", currentLatency);
				}
			}
			return result;
		}

		private void OnAuthenticateRequest(object sender, EventArgs e)
		{
			HttpApplication httpApplication = (HttpApplication)sender;
			HttpContext httpContext = httpApplication.Context;
			Diagnostics.SendWatsonReportOnUnhandledException(delegate()
			{
				LatencyTracker latencyTracker = LatencyTracker.FromHttpContext(httpContext);
				latencyTracker.StartTracking(LatencyTrackerKey.AuthenticationLatency, false);
				if (Microsoft.Exchange.Diagnostics.Components.HttpProxy.ExTraceGlobals.VerboseTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					Microsoft.Exchange.Diagnostics.Components.HttpProxy.ExTraceGlobals.VerboseTracer.TraceDebug<string, Uri, object>((long)this.GetHashCode(), "[ProxyModule::OnAuthenticateRequest]: Method {0}; Url {1}; Context {2};", httpContext.Request.HttpMethod, httpContext.Request.Url, httpContext.Items[Constants.TraceContextKey]);
				}
				this.OnAuthenticateInternal(httpApplication);
				long currentLatency = LatencyTracker.FromHttpContext(httpContext).GetCurrentLatency(LatencyTrackerKey.ProxyModuleLatency);
				if (currentLatency > 100L)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLoggerBase<RequestDetailsLogger>.GetCurrent(httpContext), "OnAuthenticate", currentLatency);
				}
			}, new Diagnostics.LastChanceExceptionHandler(RequestDetailsLogger.LastChanceExceptionHandler));
		}

		private void OnBeginRequest(object sender, EventArgs e)
		{
			HttpApplication httpApplication = (HttpApplication)sender;
			HttpContext httpContext = httpApplication.Context;
			Diagnostics.SendWatsonReportOnUnhandledException(delegate()
			{
				LatencyTracker latencyTracker = new LatencyTracker();
				latencyTracker.StartTracking(LatencyTrackerKey.ProxyModuleLatency, false);
				AspNetHelper.AddTimestampHeaderIfNecessary(httpContext.Request.Headers, "X-FrontEnd-Begin");
				if (Microsoft.Exchange.Diagnostics.Components.HttpProxy.ExTraceGlobals.VerboseTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					Microsoft.Exchange.Diagnostics.Components.HttpProxy.ExTraceGlobals.VerboseTracer.TraceDebug<string, Uri, int>((long)this.GetHashCode(), "[ProxyModule::OnBeginRequest]: Method {0}; Url {1}; Context {2};", httpContext.Request.HttpMethod, httpContext.Request.Url, httpContext.GetHashCode());
				}
				if (HealthCheckResponder.Instance.IsHealthCheckRequest(httpContext))
				{
					HealthCheckResponder.Instance.CheckHealthStateAndRespond(httpContext);
					return;
				}
				RequestDetailsLogger requestDetailsLogger = RequestDetailsLoggerBase<RequestDetailsLogger>.InitializeRequestLogger();
				requestDetailsLogger.LogCurrentTime("BeginRequest");
				httpContext.Items[Constants.TraceContextKey] = httpContext.GetHashCode();
				httpContext.Items[Constants.LatencyTrackerContextKeyName] = latencyTracker;
				requestDetailsLogger.ActivityScope.UpdateFromMessage(httpContext.Request);
				requestDetailsLogger.ActivityScope.SerializeTo(httpContext.Response);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SetCurrent(httpContext, requestDetailsLogger);
				httpContext.Items[typeof(ActivityScope)] = requestDetailsLogger.ActivityScope;
				httpContext.Items[Constants.RequestIdHttpContextKeyName] = requestDetailsLogger.ActivityScope.ActivityId;
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(requestDetailsLogger, HttpProxyMetadata.Protocol, HttpProxyGlobals.ProtocolType);
				requestDetailsLogger.SafeLogUriData(httpContext.Request.Url);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(requestDetailsLogger, ServiceCommonMetadata.HttpMethod, httpContext.Request.HttpMethod);
				string requestCorrelationId = AspNetHelper.GetRequestCorrelationId(httpContext);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(requestDetailsLogger, "CorrelationID", requestCorrelationId);
				httpContext.Response.AppendToLog(Constants.CorrelationIdKeyForIISLogs + requestCorrelationId + ";");
				string cookieValueAndSetIfNull = ClientIdCookie.GetCookieValueAndSetIfNull(httpContext);
				httpContext.Response.AppendToLog(string.Format("&{0}={1}", "ClientId", cookieValueAndSetIfNull));
				UrlUtilities.SaveOriginalRequestHostSchemePortToContext(httpContext);
				try
				{
					this.OnBeginRequestInternal(httpApplication);
				}
				catch (Exception ex)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(requestDetailsLogger, "OnBeginRequestInternal", ex.ToString());
					requestDetailsLogger.AsyncCommit(false);
					throw;
				}
			}, new Diagnostics.LastChanceExceptionHandler(RequestDetailsLogger.LastChanceExceptionHandler));
		}

		private void OnPostAuthorizeRequest(object sender, EventArgs e)
		{
			HttpApplication httpApplication = (HttpApplication)sender;
			HttpContext httpContext = httpApplication.Context;
			Diagnostics.SendWatsonReportOnUnhandledException(delegate()
			{
				if (Microsoft.Exchange.Diagnostics.Components.HttpProxy.ExTraceGlobals.VerboseTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					Microsoft.Exchange.Diagnostics.Components.HttpProxy.ExTraceGlobals.VerboseTracer.TraceDebug((long)this.GetHashCode(), "[ProxyModule::OnPostAuthorizeRequest]: Method {0}; Url {1}; Username {2}; Context {3};", new object[]
					{
						httpContext.Request.HttpMethod,
						httpContext.Request.Url,
						(httpContext.User == null) ? string.Empty : httpContext.User.Identity.GetSafeName(true),
						httpContext.GetTraceContext()
					});
				}
				this.OnPostAuthorizeInternal(httpApplication);
				LatencyTracker latencyTracker = LatencyTracker.FromHttpContext(httpContext);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(RequestDetailsLoggerBase<RequestDetailsLogger>.GetCurrent(httpContext), ServiceLatencyMetadata.HttpPipelineLatency, latencyTracker.GetCurrentLatency(LatencyTrackerKey.ProxyModuleLatency));
				long currentLatency = latencyTracker.GetCurrentLatency(LatencyTrackerKey.AuthenticationLatency);
				PerfCounters.UpdateMovingAveragePerformanceCounter(PerfCounters.HttpProxyCountersInstance.MovingAverageAuthenticationLatency, currentLatency);
				latencyTracker.StartTracking(LatencyTrackerKey.ModuleToHandlerSwitchingLatency, false);
			}, new Diagnostics.LastChanceExceptionHandler(RequestDetailsLogger.LastChanceExceptionHandler));
		}

		private void SetResponseHeaders(RequestDetailsLogger logger, HttpContext httpContext)
		{
			if (logger != null && !logger.IsDisposed && logger.ShouldSendDebugResponseHeaders())
			{
				ServiceCommonMetadataPublisher.PublishMetadata();
				if (httpContext != null)
				{
					logger.PushDebugInfoToResponseHeaders(httpContext);
				}
			}
		}

		private void OnPreSendRequestHeaders(object sender, EventArgs e)
		{
			HttpApplication httpApplication = (HttpApplication)sender;
			HttpContext httpContext = httpApplication.Context;
			Diagnostics.SendWatsonReportOnUnhandledException(delegate()
			{
				if (httpContext != null && httpContext.Response != null && httpContext.Response.Headers != null)
				{
					AspNetHelper.AddTimestampHeaderIfNecessary(httpContext.Response.Headers, "X-FrontEnd-End");
					RequestDetailsLogger current = RequestDetailsLoggerBase<RequestDetailsLogger>.GetCurrent(httpContext);
					if (current != null && !current.IsDisposed)
					{
						this.SetResponseHeaders(current, httpContext);
					}
					if (httpContext.Request.GetHttpRequestBase().IsProbeRequest())
					{
						RequestFailureContext requestFailureContext = null;
						if (httpContext.Items.Contains(RequestFailureContext.HttpContextKeyName))
						{
							requestFailureContext = (RequestFailureContext)httpContext.Items[RequestFailureContext.HttpContextKeyName];
						}
						else if (httpContext.Response.StatusCode > 400 && httpContext.Response.StatusCode < 600)
						{
							LiveIdAuthResult? liveIdAuthResult = null;
							LiveIdAuthResult value;
							if (httpContext.Items.Contains("LiveIdBasicAuthResult") && Enum.TryParse<LiveIdAuthResult>((string)httpContext.Items["LiveIdBasicAuthResult"], true, out value))
							{
								liveIdAuthResult = new LiveIdAuthResult?(value);
							}
							requestFailureContext = new RequestFailureContext(RequestFailureContext.RequestFailurePoint.FrontEnd, httpContext.Response.StatusCode, httpContext.Response.StatusDescription, string.Empty, null, null, liveIdAuthResult);
						}
						if (requestFailureContext != null)
						{
							requestFailureContext.UpdateResponse(httpContext.Response);
						}
					}
					ProxyRequestHandler proxyRequestHandler = httpContext.CurrentHandler as ProxyRequestHandler;
					if (proxyRequestHandler != null)
					{
						proxyRequestHandler.ResponseHeadersSent = true;
					}
				}
			}, new Diagnostics.LastChanceExceptionHandler(RequestDetailsLogger.LastChanceExceptionHandler));
		}

		private void OnEndRequest(object sender, EventArgs e)
		{
			HttpApplication httpApplication = (HttpApplication)sender;
			HttpContext httpContext = httpApplication.Context;
			Diagnostics.SendWatsonReportOnUnhandledException(delegate()
			{
				LatencyTracker latencyTracker = LatencyTracker.FromHttpContext(httpContext);
				RequestDetailsLogger current = RequestDetailsLoggerBase<RequestDetailsLogger>.GetCurrent(httpContext);
				int traceContext = httpContext.GetTraceContext();
				if (HttpProxyGlobals.ProtocolType != ProtocolType.Mapi)
				{
					OwaProxyRequestHandler.TryAddUnAuthenticatedPLTRequestPostDataToUriQueryOfIISLog(httpContext);
				}
				if (httpContext.Response != null && current != null)
				{
					httpContext.Response.AppendToLog(Constants.RequestIdKeyForIISLogs + current.ActivityId.ToString() + ";");
				}
				if (HealthCheckResponder.Instance.IsHealthCheckRequest(httpContext))
				{
					return;
				}
				if (httpContext.Response.StatusCode == 404 && httpContext.Response.SubStatusCode == 13)
				{
					httpContext.Response.StatusCode = 507;
				}
				if (Microsoft.Exchange.Diagnostics.Components.HttpProxy.ExTraceGlobals.VerboseTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					Microsoft.Exchange.Diagnostics.Components.HttpProxy.ExTraceGlobals.VerboseTracer.TraceDebug((long)this.GetHashCode(), "[ProxyModule::OnEndRequest]: Method {0}; Url {1}; Username {2}; Context {3};", new object[]
					{
						httpContext.Request.HttpMethod,
						httpContext.Request.Url,
						(httpContext.User == null) ? string.Empty : httpContext.User.Identity.GetSafeName(true),
						traceContext
					});
				}
				if (latencyTracker != null)
				{
					long currentLatency = latencyTracker.GetCurrentLatency(LatencyTrackerKey.HandlerToModuleSwitchingLatency);
					if (currentLatency >= 0L)
					{
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(current, HttpProxyMetadata.HandlerToModuleSwitchingLatency, currentLatency);
					}
				}
				ProxyRequestHandler proxyRequestHandler = httpContext.CurrentHandler as ProxyRequestHandler;
				if (proxyRequestHandler != null && !proxyRequestHandler.IsDisposed)
				{
					current.AppendGenericInfo("DisposeProxyRequestHandler", "ProxyModule::OnEndRequest");
					proxyRequestHandler.Dispose();
				}
				string value = httpContext.Items["AnonymousRequestFilterModule"] as string;
				if (!string.IsNullOrEmpty(value))
				{
					current.AppendGenericInfo("AnonymousRequestFilterModule", value);
				}
				try
				{
					this.OnEndRequestInternal(httpApplication);
				}
				finally
				{
					if (current != null && !current.IsDisposed)
					{
						IActivityScope activityScope = current.ActivityScope;
						if (activityScope != null)
						{
							if (!string.IsNullOrEmpty(activityScope.TenantId))
							{
								httpContext.Items["AuthenticatedUserOrganization"] = activityScope.TenantId;
							}
							ProxyModule.FinalizeRequestLatencies(httpContext, current, activityScope, latencyTracker, traceContext);
						}
						current.LogCurrentTime("EndRequest");
						current.AsyncCommit(false);
					}
					if (Microsoft.Exchange.Diagnostics.Components.HttpProxy.ExTraceGlobals.VerboseTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						Microsoft.Exchange.Diagnostics.Components.HttpProxy.ExTraceGlobals.VerboseTracer.TraceDebug(0L, "[ProxyModule::OnEndRequest]: Method {0}; Url {1}; OnEndRequestLatency {2}; Context {3};", new object[]
						{
							httpContext.Request.HttpMethod,
							httpContext.Request.Url,
							(latencyTracker != null) ? latencyTracker.GetCurrentLatency(LatencyTrackerKey.ProxyModuleLatency).ToString() : "Unknown",
							traceContext
						});
					}
				}
			}, new Diagnostics.LastChanceExceptionHandler(RequestDetailsLogger.LastChanceExceptionHandler));
		}
	}
}
