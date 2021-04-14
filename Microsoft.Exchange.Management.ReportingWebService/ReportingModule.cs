using System;
using System.Data.Services;
using System.Globalization;
using System.Security.Principal;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics.Components.ReportingWebService;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal class ReportingModule : IHttpModule
	{
		public void Init(HttpApplication context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			ExTraceGlobals.ReportingWebServiceTracer.TraceDebug((long)this.GetHashCode(), "ReportingModule.Init");
			context.BeginRequest += this.OnBeginRequest;
			context.EndRequest += this.OnEndRequest;
			if (ReportingModule.NewAuthZMethodEnabled.Value && !Datacenter.IsForefrontForOfficeDatacenter())
			{
				context.AuthorizeRequest += this.OnAuthorizeRequest;
			}
		}

		void IHttpModule.Dispose()
		{
			this.averageRequestTime.Dispose();
		}

		private void OnBeginRequest(object sender, EventArgs e)
		{
			ExTraceGlobals.ReportingWebServiceTracer.TraceDebug((long)this.GetHashCode(), "ReportingModule.OnBeginRequest");
			this.requestStartTime = DateTime.UtcNow;
			ReportingModule.activeRequestsCounter.Increment();
			this.averageRequestTime.Start();
			HttpApplication httpApplication = (HttpApplication)sender;
			HttpContext context = httpApplication.Context;
			if (!ActivityContext.IsStarted)
			{
				this.activityScope = ActivityContext.DeserializeFrom(context.Request, null);
			}
			RequestStatistics.CreateRequestRequestStatistics(context);
			this.SetCurrentCulture(context);
			this.AddTrailingSlashToServiceFile(context);
			ExTraceGlobals.ReportingWebServiceTracer.TraceDebug((long)this.GetHashCode(), "ReportingModule.OnBeginRequest - End");
		}

		private void OnEndRequest(object sender, EventArgs e)
		{
			ExTraceGlobals.ReportingWebServiceTracer.TraceDebug((long)this.GetHashCode(), "ReportingModule.OnEndRequest");
			HttpApplication httpApplication = (HttpApplication)sender;
			HttpContext context = httpApplication.Context;
			ReportingVersion.WriteVersionInfoInResponse(context);
			ReportingModule.activeRequestsCounter.Decrement();
			this.averageRequestTime.Stop();
			RequestStatistics requestStatistics = HttpContext.Current.Items[RequestStatistics.RequestStatsKey] as RequestStatistics;
			if (requestStatistics != null)
			{
				requestStatistics.AddStatisticsDataPoint(RequestStatistics.RequestStatItem.RequestResponseTime, this.requestStartTime, DateTime.UtcNow);
				requestStatistics.AddExtendedStatisticsDataPoint("HTTPCODE", context.Response.StatusCode.ToString());
				IPrincipal user = context.User;
				string text = context.Request.Headers["X-SourceCafeServer"];
				ServerLogEvent logEvent = new ServerLogEvent((ActivityContext.ActivityId != null) ? ActivityContext.ActivityId.Value.ToString() : string.Empty, string.IsNullOrEmpty(text) ? string.Empty : text, requestStatistics);
				ServerLogger.Instance.LogEvent(logEvent);
			}
			if (this.activityScope != null && !this.activityScope.IsDisposed)
			{
				this.activityScope.End();
			}
			ExTraceGlobals.ReportingWebServiceTracer.TraceDebug((long)this.GetHashCode(), "ReportingModule.OnEndRequest - End");
		}

		private void OnAuthorizeRequest(object sender, EventArgs e)
		{
			ExTraceGlobals.ReportingWebServiceTracer.TraceDebug((long)this.GetHashCode(), "ReportingModule.OnAuthorizeRequest");
			HttpApplication httpApplication = (HttpApplication)sender;
			HttpContext context = httpApplication.Context;
			RequestStatistics requestStatistics = context.Items[RequestStatistics.RequestStatsKey] as RequestStatistics;
			if (context.Request.IsAuthenticated)
			{
				requestStatistics.AddExtendedStatisticsDataPoint("AuthN", "True");
				try
				{
					RbacPrincipal rbacPrincipal = RbacPrincipalManager.Instance.AcquireRbacPrincipalWrapper(context);
					if (rbacPrincipal != null)
					{
						ExTraceGlobals.ReportingWebServiceTracer.TraceDebug((long)this.GetHashCode(), "[OnAuthorizeRequest] RbacPrincipal != null");
						context.User = rbacPrincipal;
						rbacPrincipal.SetCurrentThreadPrincipal();
						requestStatistics.AddExtendedStatisticsDataPoint("AuthZ", "True");
					}
					else
					{
						ExTraceGlobals.ReportingWebServiceTracer.TraceDebug((long)this.GetHashCode(), "[OnAuthorizeRequest] RbacPrincipal == null");
						context.Response.StatusCode = 401;
						httpApplication.CompleteRequest();
						requestStatistics.AddExtendedStatisticsDataPoint("AuthZ", "False");
					}
					goto IL_138;
				}
				catch (DataServiceException value)
				{
					ExTraceGlobals.ReportingWebServiceTracer.TraceDebug((long)this.GetHashCode(), "[OnAuthorizeRequest] DataServiceException got");
					context.Items.Add(RbacAuthorizationManager.DataServiceExceptionKey, value);
					requestStatistics.AddExtendedStatisticsDataPoint("AuthZ", "False");
					goto IL_138;
				}
			}
			requestStatistics.AddExtendedStatisticsDataPoint("AuthN", "False");
			requestStatistics.AddExtendedStatisticsDataPoint("AuthZ", "False");
			IL_138:
			ExTraceGlobals.ReportingWebServiceTracer.TraceDebug((long)this.GetHashCode(), "ReportingModule.OnAuthorizeRequest - End");
		}

		private void AddTrailingSlashToServiceFile(HttpContext httpContext)
		{
			HttpRequest request = httpContext.Request;
			if (request.FilePath.EndsWith(".svc", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(request.PathInfo))
			{
				string path = request.RawUrl.Insert(request.FilePath.Length, "/");
				httpContext.RewritePath(path);
			}
		}

		private void SetCurrentCulture(HttpContext httpContext)
		{
			CultureInfo defaultCulture = Culture.GetDefaultCulture(httpContext);
			Thread.CurrentThread.CurrentCulture = defaultCulture;
			Thread.CurrentThread.CurrentUICulture = defaultCulture;
		}

		private static readonly BoolAppSettingsEntry NewAuthZMethodEnabled = new BoolAppSettingsEntry("NewAuthZMethodEnabled", false, ExTraceGlobals.ReportingWebServiceTracer);

		private static readonly PerfCounterGroup activeRequestsCounter = new PerfCounterGroup(RwsPerfCounters.ActiveRequests, RwsPerfCounters.ActiveRequestsPeak, RwsPerfCounters.ActiveRequestsTotal);

		private readonly AverageTimePerfCounter averageRequestTime = new AverageTimePerfCounter(RwsPerfCounters.AverageRequestResponseTime, RwsPerfCounters.AverageRequestResponseTimeBase, true);

		private DateTime requestStartTime;

		private ActivityScope activityScope;
	}
}
