using System;
using System.Web;

namespace Microsoft.Exchange.HttpProxy
{
	internal static class HttpContextDiagnosticsExtensions
	{
		public static LogData GetLogData(this HttpContextBase httpContext)
		{
			LogData logData = httpContext.Items["LogData_da486740-fc1c-4d69-8e87-bfb3757ad732"] as LogData;
			if (logData == null)
			{
				throw new InvalidOperationException("LogData is not initialized.");
			}
			return logData;
		}

		public static void InitializeLogging(this HttpContextBase httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			if (httpContext.Items.Contains("LogData_da486740-fc1c-4d69-8e87-bfb3757ad732"))
			{
				throw new InvalidOperationException("LogData for this request is already initialized in the HttpContext.");
			}
			LogData value = new LogData();
			httpContext.Items.Add("LogData_da486740-fc1c-4d69-8e87-bfb3757ad732", value);
		}

		private const string LogDataKey = "LogData_da486740-fc1c-4d69-8e87-bfb3757ad732";

		private const string LatencyDataKey = "LatencyData_da486740-fc1c-4d69-8e87-bfb3757ad732";
	}
}
