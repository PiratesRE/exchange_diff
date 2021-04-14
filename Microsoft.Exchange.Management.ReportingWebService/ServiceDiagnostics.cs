using System;
using System.Data.Services;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ReportingWebService;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal static class ServiceDiagnostics
	{
		internal static void InitializeWatsonReporting(bool sendWatsonReports)
		{
			ExWatson.Register("E12");
			ServiceDiagnostics.sendWatsonReportsEnabled = sendWatsonReports;
		}

		internal static void ReportUnhandledException(Exception exception, HttpContext httpContext)
		{
			Exception ex = exception;
			if (exception is DataServiceException && exception.InnerException != null)
			{
				ex = exception.InnerException;
			}
			if (HttpContext.Current != null && HttpContext.Current.Response != null)
			{
				HttpContext.Current.Response.AddHeader("X-RWS-Error", (ex != null) ? ex.GetType().ToString() : string.Empty);
			}
			bool flag = !(exception is DataServiceException) || (exception is DataServiceException && ((DataServiceException)exception).ErrorCode.Equals(ReportingErrorCode.UnknownError.ToString(), StringComparison.InvariantCultureIgnoreCase));
			string text;
			int num;
			if (flag)
			{
				text = string.Format("UnknownError&ExceptionType={0}&ExceptionMessage={1}", (ex != null) ? ex.GetType().ToString() : string.Empty, (ex != null) ? ex.Message : string.Empty);
				num = 500;
			}
			else
			{
				text = ((DataServiceException)exception).ErrorCode;
				num = ((DataServiceException)exception).StatusCode;
			}
			RwsPerfCounters.RequestErrors.Increment();
			string text2 = (httpContext != null && httpContext.Request != null) ? httpContext.Request.Url.ToString() : string.Empty;
			ReportingWebServiceEventLogConstants.Tuple_RequestFailed.LogPeriodicEvent(EventLogExtension.GetPeriodicKeyPerUser(), new object[]
			{
				EventLogExtension.GetUserNameToLog(),
				text2,
				num.ToString(),
				text,
				ex,
				(ActivityContext.ActivityId != null) ? ActivityContext.ActivityId.Value.ToString() : "NoActivityId"
			});
			if (ServiceDiagnostics.sendWatsonReportsEnabled && ServiceDiagnostics.IsRWSException(exception) && !(exception is DataServiceException))
			{
				RwsPerfCounters.SendWatson.Increment();
				ExWatson.AddExtraData(text2);
				ExWatson.SendReport(exception);
			}
		}

		internal static void ThrowError(ReportingErrorCode errorCode, string message)
		{
			ServiceDiagnostics.ThrowError(errorCode, message, null);
		}

		internal static void ThrowError(ReportingErrorCode errorCode, string message, Exception exception)
		{
			ExTraceGlobals.ReportingWebServiceTracer.TraceError(0L, message);
			throw new DataServiceException((int)ReportingErrors.HttpStatusCodes[errorCode], errorCode.ToString(), message, string.Empty, exception);
		}

		private static bool IsRWSException(Exception exception)
		{
			return exception.StackTrace.Contains("Microsoft.Exchange.Management.ReportingWebService") || (exception.InnerException != null && exception.InnerException.StackTrace.Contains("Microsoft.Exchange.Management.ReportingWebService"));
		}

		internal const string RwsErrorHeaderName = "X-RWS-Error";

		internal const string RwsNamespace = "Microsoft.Exchange.Management.ReportingWebService";

		private static bool sendWatsonReportsEnabled = true;
	}
}
