using System;
using System.DirectoryServices.Protocols;
using System.IO;
using System.Web;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Diagnostics.FaultInjection;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Autodiscover
{
	public class Global : HttpApplication
	{
		private void Application_Start(object sender, EventArgs e)
		{
			Globals.InitializeMultiPerfCounterInstance("AutoDisc");
			Microsoft.Exchange.Diagnostics.Components.Autodiscover.ExTraceGlobals.FrameworkTracer.TraceDebug<string>((long)this.GetHashCode(), "[Application_Start()] Privileges.RemoveAllExcept(,,\"{0}\");", "MSExchangeAutodiscoverAppPool");
			string[] privilegesToKeep = new string[]
			{
				"SeAuditPrivilege",
				"SeChangeNotifyPrivilege",
				"SeCreateGlobalPrivilege"
			};
			int num = Privileges.RemoveAllExcept(privilegesToKeep, "MSExchangeAutodiscoverAppPool");
			if (num != 0)
			{
				Microsoft.Exchange.Diagnostics.Components.Autodiscover.ExTraceGlobals.FrameworkTracer.TraceError<TimeSpan, int>((long)this.GetHashCode(), "[Application_Start()] 'Failed to remove privileges, exiting. Time=\"{0}\",win32Error=\"{1}\"'", ExDateTime.Now.TimeOfDay, num);
				Environment.Exit(num);
			}
			PerformanceCounters.Initialize();
			ExchangeDiagnosticsHelper.RegisterDiagnosticsComponents();
			Microsoft.Exchange.Diagnostics.Components.Autodiscover.ExTraceGlobals.FrameworkTracer.TraceDebug((long)this.GetHashCode(), "[Application_Start()] 'ApplicationStarted'");
			Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_InfoWebApplicationStart, Common.PeriodicKey, new object[0]);
			string text = ExEnvironment.IsTest ? "E12" : "E12IIS";
			ExWatson.Register(text);
			Microsoft.Exchange.Diagnostics.Components.Autodiscover.ExTraceGlobals.FaultInjectionTracer.RegisterExceptionInjectionCallback(new ExceptionInjectionCallback(FaultInjection.Callback));
			Microsoft.Exchange.Diagnostics.Components.Autodiscover.ExTraceGlobals.FrameworkTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[Application_Start()] ExWatson.Register(\"{0}\");AppName=\"{1}\";", text, ExWatson.AppName);
		}

		private void Application_BeginRequest(object sender, EventArgs e)
		{
			RequestDetailsLoggerBase<RequestDetailsLogger>.InitializeRequestLogger();
			RequestDetailsLoggerBase<RequestDetailsLogger>.Current.ActivityScope.UpdateFromMessage(HttpContext.Current.Request);
			RequestDetailsLoggerBase<RequestDetailsLogger>.Current.ActivityScope.SerializeTo(HttpContext.Current.Response);
			string sourceCafeServer = CafeHelper.GetSourceCafeServer(HttpContext.Current.Request);
			if (!string.IsNullOrEmpty(sourceCafeServer))
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.Current.Set(AutoDiscoverMetadata.FrontEndServer, sourceCafeServer);
			}
			Microsoft.Exchange.Diagnostics.Components.Autodiscover.ExTraceGlobals.FrameworkTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[Application_BeginRequest()] Request.Path=\"{0}\"; Request.PhysicalPath=\"{1}\";", base.Request.Path, base.Request.PhysicalPath);
			int num = base.Request.Path.IndexOf('\\');
			string fullPath = Path.GetFullPath(base.Request.PhysicalPath);
			if (num >= 0 || fullPath != base.Request.PhysicalPath)
			{
				Microsoft.Exchange.Diagnostics.Components.Autodiscover.ExTraceGlobals.FrameworkTracer.TraceError<int, string>((long)this.GetHashCode(), "[Application_BeginRequest()] IndexOf('\\')={0};FullPath=\"{1}\"; // Throwing 404 HttpException", num, fullPath);
				this.exception = new HttpException(404, string.Empty);
				throw this.exception;
			}
		}

		private void Application_EndRequest(object sender, EventArgs e)
		{
			RequestDetailsLogger requestDetailsLogger = RequestDetailsLoggerBase<RequestDetailsLogger>.Current;
			if (requestDetailsLogger != null && !requestDetailsLogger.IsDisposed)
			{
				HttpApplication httpApplication = (HttpApplication)sender;
				HttpContext context = httpApplication.Context;
				string text = context.Items["AutodiscoverRedirectLog"] as string;
				if (text != null)
				{
					requestDetailsLogger.Set(AutoDiscoverMetadata.AutodiscoverRedirect, text);
				}
				string text2 = context.Items["LiveIdBasicAuthResult"] as string;
				if (!string.IsNullOrEmpty(text2) && !string.Equals(text2, LiveIdAuthResult.Success.ToString(), StringComparison.OrdinalIgnoreCase))
				{
					string text3 = context.Items["LiveIdBasicLog"] as string;
					if (!string.IsNullOrEmpty(text3))
					{
						int num = text3.IndexOf(" ---> ");
						if (num != -1)
						{
							text3 = text3.Substring(0, num);
						}
					}
					context.Response.Headers["X-AutoDiscovery-Error"] = string.Format("LiveIdBasicAuth:{0}:{1};", text2, text3);
				}
				requestDetailsLogger.AsyncCommit();
			}
		}

		private void Application_Error(object sender, EventArgs e)
		{
			HttpApplication httpApplication = (HttpApplication)sender;
			HttpContext context = httpApplication.Context;
			Exception ex = httpApplication.Server.GetLastError();
			Microsoft.Exchange.Diagnostics.Components.Autodiscover.ExTraceGlobals.FrameworkTracer.TraceDebug<Type>((long)this.GetHashCode(), "[ReportException()] lastException.GetType()=\"{0}\"", ex.GetType());
			if (this.exception != ex)
			{
				if (ex is HttpUnhandledException)
				{
					if (ex.InnerException != null)
					{
						Microsoft.Exchange.Diagnostics.Components.Autodiscover.ExTraceGlobals.FrameworkTracer.TraceError((long)this.GetHashCode(), "[Application_Error()] lastException=HttpUnhandledException && lastException.InnerException!=null");
						ex = ex.InnerException;
					}
				}
				else if (ex is ADTransientException && ex.InnerException is LdapException)
				{
					Microsoft.Exchange.Diagnostics.Components.Autodiscover.ExTraceGlobals.FrameworkTracer.TraceError((long)this.GetHashCode(), "[Application_Error()] lastException=ADTransientException && lastException.InnerException=LdapException");
					ex = ex.InnerException;
				}
				Common.ReportException(ex, this, context);
			}
		}

		private void Application_End(object sender, EventArgs e)
		{
			try
			{
				ExchangeDiagnosticsHelper.UnRegisterDiagnosticsComponents();
			}
			finally
			{
				RequestDetailsLogger.FlushQueuedFileWrites();
				Microsoft.Exchange.Diagnostics.Components.Autodiscover.ExTraceGlobals.FrameworkTracer.TraceDebug((long)this.GetHashCode(), "[Application_End()] 'Autodiscover service stopped'");
				Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_InfoWebApplicationStop, Common.PeriodicKey, new object[]
				{
					"Autodiscover service stopped"
				});
			}
		}

		private Exception exception;
	}
}
