using System;
using System.Web;
using System.Web.Configuration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class PerformanceConsoleModule : IHttpModule
	{
		void IHttpModule.Init(HttpApplication application)
		{
			this.showPerformanceConsole = StringComparer.OrdinalIgnoreCase.Equals("true", WebConfigurationManager.AppSettings["ShowPerformanceConsole"]);
			if (this.showPerformanceConsole)
			{
				EcpEventLogConstants.Tuple_EcpPerformanceConsoleEnabled.LogEvent(new object[0]);
			}
			this.logPerformanceData = StringComparer.OrdinalIgnoreCase.Equals("true", WebConfigurationManager.AppSettings["LogPerformanceData"]);
			if (this.logPerformanceData)
			{
				EcpEventLogConstants.Tuple_EcpPerformanceIisLogEnabled.LogEvent(new object[0]);
			}
			if (EcpEventLogConstants.Tuple_EcpPerformanceEventLogHighEnabled.IsEnabled())
			{
				EcpEventLogConstants.Tuple_EcpPerformanceEventLogHighEnabled.LogEvent(new object[0]);
			}
			else
			{
				EcpEventLogConstants.Tuple_EcpPerformanceEventLogMediumEnabled.LogEvent(new object[0]);
			}
			application.BeginRequest += this.Application_BeginRequest;
			application.PostAuthenticateRequest += this.Application_PostAuthenticateRequest;
			application.PostAuthorizeRequest += this.Application_PostAuthorizeRequest;
			application.PostResolveRequestCache += this.Application_PostResolveRequestCache;
			application.PostMapRequestHandler += this.Application_PostMapRequestHandler;
			application.PostAcquireRequestState += this.Application_PostAcquireRequestState;
			application.PostRequestHandlerExecute += this.Application_PostRequestHandlerExecute;
			application.PostReleaseRequestState += this.Application_PostReleaseRequestState;
			application.PostUpdateRequestCache += this.Application_PostUpdateRequestCache;
			application.PostLogRequest += this.Application_PostLogRequest;
			application.EndRequest += this.Application_EndRequest;
			application.PreSendRequestContent += this.Application_PreSendRequestContent;
		}

		void IHttpModule.Dispose()
		{
		}

		private void Application_BeginRequest(object sender, EventArgs e)
		{
			PerfRecord.Current = new PerfRecord(HttpContext.Current.Request.Path);
			PerfRecord.Current.StepStarted(RequestNotification.AuthenticateRequest);
		}

		private void Application_PostAuthenticateRequest(object sender, EventArgs e)
		{
			PerfRecord.Current.StepStarted(RequestNotification.AuthorizeRequest);
		}

		private void Application_PostAuthorizeRequest(object sender, EventArgs e)
		{
			PerfRecord.Current.StepStarted(RequestNotification.ResolveRequestCache);
		}

		private void Application_PostResolveRequestCache(object sender, EventArgs e)
		{
			PerfRecord.Current.StepStarted(RequestNotification.MapRequestHandler);
		}

		private void Application_PostMapRequestHandler(object sender, EventArgs e)
		{
			PerfRecord.Current.StepStarted(RequestNotification.AcquireRequestState);
		}

		private void Application_PostAcquireRequestState(object sender, EventArgs e)
		{
			PerfRecord.Current.StepStarted(RequestNotification.ExecuteRequestHandler);
		}

		private void Application_PostRequestHandlerExecute(object sender, EventArgs e)
		{
			PerfRecord.Current.StepStarted(RequestNotification.ReleaseRequestState);
		}

		private void Application_PostReleaseRequestState(object sender, EventArgs e)
		{
			PerfRecord.Current.StepStarted(RequestNotification.UpdateRequestCache);
		}

		private void Application_PostUpdateRequestCache(object sender, EventArgs e)
		{
			PerfRecord.Current.StepStarted(RequestNotification.LogRequest);
		}

		private void Application_PostLogRequest(object sender, EventArgs e)
		{
			if (PerfRecord.Current != null)
			{
				PerfRecord.Current.StepCompleted();
			}
		}

		private void Application_EndRequest(object sender, EventArgs e)
		{
			if (PerfRecord.Current != null)
			{
				PerfRecord.Current.EndRequest();
				if (this.logPerformanceData)
				{
					PerfRecord.Current.AppendToIisLog();
				}
			}
		}

		private void Application_PreSendRequestContent(object sender, EventArgs e)
		{
			if (!this.showPerformanceConsole)
			{
				if (!this.logPerformanceData)
				{
					return;
				}
			}
			try
			{
				if (PerfRecord.Current != null)
				{
					HttpContext.Current.Response.AppendHeader("X-msExchPerf", PerfRecord.Current.ToString());
				}
			}
			catch (HttpException)
			{
			}
		}

		private bool logPerformanceData;

		private bool showPerformanceConsole;
	}
}
