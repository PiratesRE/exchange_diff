using System;
using System.Text;
using System.Web;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.DiagnosticsModules.EventLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.DiagnosticsModules;

namespace Microsoft.Exchange.Configuration.DiagnosticsModules
{
	public class ErrorLoggingModule : IHttpModule
	{
		void IHttpModule.Init(HttpApplication application)
		{
			application.Error += ErrorLoggingModule.OnLogError;
		}

		void IHttpModule.Dispose()
		{
		}

		private static void OnLogError(object source, EventArgs args)
		{
			Logger.EnterFunction(ExTraceGlobals.ErrorLoggingModuleTracer, "ErrorLoggingModule.OnLogError");
			try
			{
				HttpContext httpContext = HttpContext.Current;
				Exception exceptionFromHttpContext = ErrorLoggingModule.GetExceptionFromHttpContext(httpContext);
				if (exceptionFromHttpContext != null)
				{
					HttpLogger.SafeAppendGenericError("Http", exceptionFromHttpContext, new Func<Exception, bool>(KnownException.IsUnhandledException));
					ErrorLoggingModule.LogVerbose("Log unhandled exception: {0}", new object[]
					{
						exceptionFromHttpContext.ToString()
					});
					Logger.LogEvent(ErrorLoggingModule.eventLogger, TaskEventLogConstants.Tuple_ErrorLogging_UnhandledException, null, new object[]
					{
						exceptionFromHttpContext.Message,
						httpContext.Server.UrlDecode(httpContext.Request.RawUrl),
						ErrorLoggingModule.GetExceptionDetail(exceptionFromHttpContext)
					});
				}
			}
			catch (Exception ex)
			{
				try
				{
					ErrorLoggingModule.LogVerbose("Exception when Log unhandled exception: {0}", new object[]
					{
						ex.ToString()
					});
				}
				catch (Exception)
				{
				}
			}
			Logger.ExitFunction(ExTraceGlobals.ErrorLoggingModuleTracer, "ErrorLoggingModule.OnLogError");
		}

		private static Exception GetExceptionFromHttpContext(HttpContext context)
		{
			if (context.Error != null)
			{
				return context.Error;
			}
			Exception[] allErrors = context.AllErrors;
			if (allErrors == null | allErrors.Length == 0)
			{
				return null;
			}
			return allErrors[0];
		}

		private static string GetExceptionDetail(Exception ex)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (Exception ex2 = ex; ex2 != null; ex2 = ex2.InnerException)
			{
				stringBuilder.AppendFormat("{0}\r\n{1}\r\n", ex2.Message, ex2.StackTrace);
			}
			return stringBuilder.ToString();
		}

		private static void LogVerbose(string message, params object[] args)
		{
			Logger.TraceInformation(ExTraceGlobals.ErrorLoggingModuleTracer, message, args);
		}

		private static readonly ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.ErrorLoggingModuleTracer.Category, "MSExchange Error Logging Module", "MSExchange Management");
	}
}
