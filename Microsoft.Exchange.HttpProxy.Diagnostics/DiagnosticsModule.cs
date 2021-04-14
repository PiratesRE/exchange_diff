using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.HttpProxy
{
	public class DiagnosticsModule : IHttpModule
	{
		internal RequestLogger Logger { get; private set; }

		public void Init(HttpApplication application)
		{
			application.BeginRequest += this.BeginRequest;
			application.EndRequest += this.EndRequest;
		}

		public void Dispose()
		{
		}

		internal void BeginRequest(object sender, EventArgs e)
		{
			this.ExecuteHandlerIfModuleIsEnabled(delegate
			{
				HttpApplication httpApplication = (HttpApplication)sender;
				HttpContextBase context = new HttpContextWrapper(httpApplication.Context);
				this.InitializeDiagnostics(context);
			});
		}

		internal void EndRequest(object sender, EventArgs e)
		{
			this.ExecuteHandlerIfModuleIsEnabled(delegate
			{
				this.Logger.LatencyTracker.LogElapsedTimeInDetailedLatencyInfo("Diagnosticmodule_EndRequest_Enter");
				HttpApplication httpApplication = (HttpApplication)sender;
				HttpContextBase context = new HttpContextWrapper(httpApplication.Context);
				this.activityScope.End();
				this.AddDiagnosticInfo(context);
				this.FlushDiagnosticInfo(context);
				this.Logger.LatencyTracker.LogElapsedTimeInDetailedLatencyInfo("Diagnosticmodule_EndRequest_End");
			});
		}

		internal void InitializeDiagnostics(HttpContextBase context)
		{
			context.InitializeLogging();
			this.Logger = RequestLogger.GetLogger(context);
			this.Logger.LatencyTracker.LogElapsedTimeInDetailedLatencyInfo("Diagnosticmodule_InitializeDiagnostics_Start");
			this.activityScope = (context.Items[typeof(ActivityScope)] as IActivityScope);
			if (this.activityScope == null)
			{
				this.activityScope = ActivityContext.GetCurrentActivityScope();
				if (this.activityScope == null)
				{
					this.activityScope = ActivityContext.Start(null);
				}
				context.Items[typeof(ActivityScope)] = this.activityScope;
			}
			this.AddDiagnosticHeaders(context.Request);
			this.Logger.LatencyTracker.LogElapsedTimeInDetailedLatencyInfo("Diagnosticmodule_InitializeDiagnostics_End");
		}

		internal void FlushDiagnosticInfo(HttpContextBase context)
		{
			this.Logger.Flush();
		}

		private void ExecuteHandlerIfModuleIsEnabled(ExWatson.MethodDelegate methodDelegate)
		{
			if (HttpProxySettings.DiagnosticsEnabled.Value)
			{
				Diagnostics.SendWatsonReportOnUnhandledException(methodDelegate, new Diagnostics.LastChanceExceptionHandler(this.LastChanceExceptionHandler));
			}
		}

		private void AddDiagnosticInfo(HttpContextBase context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			this.Logger.LogField(LogKey.DateTime, DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffZ"));
			this.Logger.LogField(LogKey.RequestId, this.activityScope.ActivityId);
			this.Logger.LogField(LogKey.MajorVersion, Constants.VersionMajor);
			this.Logger.LogField(LogKey.MinorVersion, Constants.VersionMinor);
			this.Logger.LogField(LogKey.BuildVersion, Constants.VersionBuild);
			this.Logger.LogField(LogKey.RevisionVersion, Constants.VersionRevision);
			this.Logger.LogField(LogKey.Protocol, HttpProxyGlobals.ProtocolType);
			IIdentity identity = (context.User != null) ? context.User.Identity : null;
			if (identity != null)
			{
				this.Logger.LogField(LogKey.AuthenticationType, identity.AuthenticationType);
				this.Logger.LogField(LogKey.IsAuthenticated, identity.IsAuthenticated);
				this.Logger.LogField(LogKey.AuthenticatedUser, identity.Name);
			}
			this.Logger.LogField(LogKey.UserAgent, context.Request.UserAgent);
			this.Logger.LogField(LogKey.ClientIpAddress, context.Request.UserHostAddress);
			this.Logger.LogField(LogKey.ClientRequestId, context.Request.Headers[WellKnownHeader.XRequestId]);
			this.Logger.LogField(LogKey.UrlHost, context.Request.Url.DnsSafeHost);
			this.Logger.LogField(LogKey.UrlStem, context.Request.Url.LocalPath);
			this.Logger.LogField(LogKey.UrlQuery, context.Request.Url.Query);
			this.Logger.LogField(LogKey.Method, context.Request.HttpMethod);
			this.Logger.LogField(LogKey.ProtocolAction, context.Request.QueryString["Action"]);
			this.Logger.LogField(LogKey.ServerHostName, Constants.MachineName);
			this.Logger.LogField(LogKey.HttpStatus, context.Response.StatusCode);
			this.Logger.LogField(LogKey.BackEndGenericInfo, context.Response.Headers[WellKnownHeader.XBackendHeaderPrefix]);
			string value = context.Items["AnonymousRequestFilterModule"] as string;
			if (!string.IsNullOrEmpty(value))
			{
				this.Logger.AppendGenericInfo("AnonymousRequestFilterModule", value);
			}
			NativeProxyLogHelper.PublishNativeProxyStatistics(context);
			this.Logger.LatencyTracker.LogElapsedTimeInMilliseconds(LogKey.TotalRequestTime);
		}

		private void AddDiagnosticHeaders(HttpRequestBase request)
		{
			this.activityScope.SerializeMinimalTo(request);
		}

		private void LastChanceExceptionHandler(Exception ex)
		{
			if (this.Logger != null)
			{
				this.Logger.LastChanceExceptionHandler(ex);
			}
		}

		private IActivityScope activityScope;
	}
}
