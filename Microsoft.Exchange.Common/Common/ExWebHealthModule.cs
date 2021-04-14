using System;
using System.Web;
using System.Web.Hosting;
using Microsoft.Exchange.Diagnostics.Components.Common;

namespace Microsoft.Exchange.Common
{
	public sealed class ExWebHealthModule : IHttpModule
	{
		public ExWebHealthModule()
		{
			this.Handler = new ExWebHealthHandler(HostingEnvironment.ApplicationVirtualPath.Substring(1));
			this.TimeoutReportHandler = new ExWebTimeoutReportHandler();
		}

		internal ExWebHealthHandler Handler { get; private set; }

		internal ExWebTimeoutReportHandler TimeoutReportHandler { get; private set; }

		public void Dispose()
		{
		}

		public void Init(HttpApplication context)
		{
			ExTraceGlobals.WebHealthTracer.TraceDebug(0L, "ExWebHealthModule.Init()");
			context.BeginRequest += this.OnBeginRequest;
		}

		private void OnBeginRequest(object sender, EventArgs e)
		{
			ExTraceGlobals.WebHealthTracer.TraceDebug(0L, "ExWebHealthModule.OnBeginRequest()");
			HttpContext context = ((HttpApplication)sender).Context;
			if (this.IsLocalRequest(context.Request) && this.IsHealthPageRequest(context.Request))
			{
				try
				{
					try
					{
						ExTraceGlobals.WebHealthTracer.TraceDebug<string>(0L, "ExWebHealthModule.OnBeginRequest() Start request for {0}", HostingEnvironment.ApplicationVirtualPath);
						this.Handler.ProcessHealth(new ExWebHealthResponseWrapper(context.Response));
					}
					catch (Exception arg)
					{
						ExTraceGlobals.WebHealthTracer.TraceError<string, Exception>(0L, "ExWebHealthModule.OnBeginRequest() Encountered exception request for {0}. Error:{1}", HostingEnvironment.ApplicationVirtualPath, arg);
					}
					return;
				}
				finally
				{
					ExTraceGlobals.WebHealthTracer.TraceDebug<string>(0L, "ExWebHealthModule.OnBeginRequest() End request for {0}", HostingEnvironment.ApplicationVirtualPath);
					context.Response.End();
				}
			}
			if (this.IsReportTimeoutPageRequest(context.Request))
			{
				try
				{
					ExTraceGlobals.WebHealthTracer.TraceDebug<string>(0L, "ExWebHealthModule.OnBeginRequest() Start request processing for report timeout page: {0}", context.Request.FilePath);
					this.TimeoutReportHandler.Process(context);
				}
				catch (Exception arg2)
				{
					ExTraceGlobals.WebHealthTracer.TraceError<string, Exception>(0L, "ExWebHealthModule.OnBeginRequest() Encountered exception request for {0}. Error:{1}", context.Request.FilePath, arg2);
				}
				finally
				{
					ExTraceGlobals.WebHealthTracer.TraceDebug<string>(0L, "ExWebHealthModule.OnBeginRequest() End request for {0}", context.Request.FilePath);
					context.Response.End();
				}
			}
		}

		private bool IsLocalRequest(HttpRequest webRequest)
		{
			return webRequest.IsLocal && string.Equals("localhost", webRequest.Url.Host, StringComparison.OrdinalIgnoreCase);
		}

		private bool IsHealthPageRequest(HttpRequest webRequest)
		{
			return string.Equals(webRequest.AppRelativeCurrentExecutionFilePath, "~/exhealth.check", StringComparison.OrdinalIgnoreCase);
		}

		private bool IsReportTimeoutPageRequest(HttpRequest webRequest)
		{
			string text = webRequest.QueryString["realm"];
			return !string.IsNullOrEmpty(text) && text.Equals("exhealth.reporttimeout", StringComparison.OrdinalIgnoreCase);
		}

		internal const string HealthPageName = "exhealth.check";

		internal const string ReportTimeoutPageName = "exhealth.reporttimeout";

		internal const string HealthPage = "~/exhealth.check";

		private const string LocalHost = "localhost";
	}
}
