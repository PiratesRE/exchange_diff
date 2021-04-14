using System;
using System.IO;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.HttpProxy.EventLogs;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.HttpProxy
{
	internal static class Diagnostics
	{
		internal static ExEventLog Logger
		{
			get
			{
				return Diagnostics.logger;
			}
		}

		internal static bool LogExceptionWithTrace(ExEventLog.EventTuple tuple, string periodicKey, Trace tagTracer, object thisObject, string traceFormat, Exception exception)
		{
			tagTracer.TraceError<Exception>((long)((thisObject == null) ? 0 : thisObject.GetHashCode()), traceFormat, exception);
			string text = exception.ToString();
			if (text.Length > 32000)
			{
				text = text.Substring(0, 2000) + "...\n" + text.Substring(text.Length - 20000, 20000);
			}
			return Diagnostics.logger.LogEvent(tuple, periodicKey, new object[]
			{
				HttpProxyGlobals.ProtocolType,
				text
			});
		}

		internal static bool LogEventWithTrace(ExEventLog.EventTuple tuple, string periodicKey, Trace tagTracer, object thisObject, string traceFormat, params object[] messageArgs)
		{
			tagTracer.TraceDebug((long)((thisObject == null) ? 0 : thisObject.GetHashCode()), traceFormat, messageArgs);
			return Diagnostics.logger.LogEvent(tuple, periodicKey, messageArgs);
		}

		internal static void SendWatsonReportOnUnhandledException(ExWatson.MethodDelegate methodDelegate)
		{
			Diagnostics.SendWatsonReportOnUnhandledException(methodDelegate, null);
		}

		internal static void SendWatsonReportOnUnhandledException(ExWatson.MethodDelegate methodDelegate, Diagnostics.LastChanceExceptionHandler exceptionHandler)
		{
			WatsonReportAction action = Diagnostics.RegisterAdditionalWatsonData();
			try
			{
				ExWatson.SendReportOnUnhandledException(methodDelegate, delegate(object exception)
				{
					bool flag = Diagnostics.SendWatsonReports.Value;
					Exception ex = exception as Exception;
					if (ex != null)
					{
						if (exceptionHandler != null)
						{
							exceptionHandler(ex);
						}
						ExTraceGlobals.ExceptionTracer.TraceError<Exception>(0L, "Encountered unhandled exception: {0}", ex);
						flag = Diagnostics.IsSendReportValid(ex);
						if (flag)
						{
							Diagnostics.LogExceptionWithTrace(FrontEndHttpProxyEventLogConstants.Tuple_InternalServerError, null, ExTraceGlobals.ExceptionTracer, null, "Encountered unhandled exception: {0}", ex);
							ExWatson.SetWatsonReportAlreadySent(ex);
						}
					}
					ExTraceGlobals.ExceptionTracer.TraceError<bool>(0L, "SendWatsonReportOnUnhandledException isSendReportValid: {0}", flag);
					return flag;
				}, ReportOptions.None);
			}
			finally
			{
				ExWatson.UnregisterReportAction(action, WatsonActionScope.Thread);
			}
		}

		internal static void TraceErrorOnUnhandledException(ExWatson.MethodDelegate methodDelegate)
		{
			ExWatson.SendReportOnUnhandledException(methodDelegate, delegate(object exception)
			{
				ExTraceGlobals.ExceptionTracer.TraceError(0L, "Unhandled exception, Exception details: {0}", new object[]
				{
					exception
				});
				return false;
			});
		}

		internal static void InitializeWatsonReporting()
		{
			ExTraceGlobals.ExceptionTracer.TraceDebug<bool, bool>(0L, "sendWatsonReports: {0} filterExceptionsFromWatsonReporting: {1}", Diagnostics.SendWatsonReports.Value, Diagnostics.FilterExceptionsFromWatsonReport.Value);
			ExWatson.Register(ExEnvironment.IsTest ? "E12" : "E12IIS");
		}

		internal static void ReportException(Exception exception, ExEventLog.EventTuple eventTuple, object eventObject, string traceFormat)
		{
			bool flag = exception is AccessViolationException;
			if (Diagnostics.IsSendReportValid(exception))
			{
				Diagnostics.LogExceptionWithTrace(eventTuple, null, ExTraceGlobals.ExceptionTracer, eventObject, traceFormat, exception);
				ExWatson.HandleException(new UnhandledExceptionEventArgs(exception, flag), ReportOptions.None);
				ExWatson.SetWatsonReportAlreadySent(exception);
			}
			else
			{
				ExTraceGlobals.ExceptionTracer.TraceError<Exception>(0L, traceFormat, exception);
			}
			if (flag)
			{
				Environment.Exit(1);
			}
		}

		internal static string BuildFullExceptionTrace(Exception exception)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (exception != null)
			{
				stringBuilder.AppendFormat("Exception Class: \"{0}\", Message: \"{1}\"\n", exception.GetType().FullName, exception.Message);
				exception = exception.InnerException;
			}
			return stringBuilder.ToString();
		}

		internal static WatsonReportAction RegisterAdditionalWatsonData()
		{
			string watsonExtraData = Diagnostics.GetWatsonExtraData();
			WatsonReportAction watsonReportAction = new WatsonExtraDataReportAction(watsonExtraData);
			ExWatson.RegisterReportAction(watsonReportAction, WatsonActionScope.Thread);
			return watsonReportAction;
		}

		private static string GetWatsonExtraData()
		{
			return "<none>";
		}

		private static bool IsSendReportValid(Exception exception)
		{
			if (ExWatson.IsWatsonReportAlreadySent(exception))
			{
				return false;
			}
			bool flag = Diagnostics.SendWatsonReports.Value;
			if (flag && Diagnostics.FilterExceptionsFromWatsonReport.Value)
			{
				if (exception is HttpException)
				{
					flag = false;
				}
				else if (exception is System.ServiceModel.QuotaExceededException)
				{
					flag = false;
				}
				else if (exception is DataValidationException)
				{
					flag = false;
				}
				else if (exception is DataSourceOperationException)
				{
					flag = false;
				}
				else if (exception is StoragePermanentException || exception is StorageTransientException)
				{
					flag = false;
				}
				else if (exception is ServiceDiscoveryTransientException)
				{
					flag = false;
				}
				else if (exception is IOException)
				{
					flag = false;
				}
				else if (exception is OutOfMemoryException)
				{
					flag = false;
				}
				else if (exception is ADTransientException)
				{
					flag = false;
				}
				else if (exception is ThreadAbortException)
				{
					flag = false;
				}
				else if (exception.StackTrace.Contains("Microsoft.Exchange.Diagnostics.FaultInjection.FaultInjectionTrace.InjectException"))
				{
					flag = false;
				}
			}
			ExTraceGlobals.ExceptionTracer.TraceDebug<bool>(0L, "IsSendReportValid isSendReportValid: {0}", flag);
			return flag;
		}

		private const string FaultInjectionFrame = "Microsoft.Exchange.Diagnostics.FaultInjection.FaultInjectionTrace.InjectException";

		private const string EventSource = "MSExchange Front End HTTP Proxy";

		private static readonly BoolAppSettingsEntry SendWatsonReports = new BoolAppSettingsEntry(HttpProxySettings.Prefix("SendWatsonReports"), true, ExTraceGlobals.VerboseTracer);

		private static readonly BoolAppSettingsEntry FilterExceptionsFromWatsonReport = new BoolAppSettingsEntry(HttpProxySettings.Prefix("FilterExceptionsFromWatsonReport"), true, ExTraceGlobals.VerboseTracer);

		private static ExEventLog logger = new ExEventLog(ExTraceGlobals.ExceptionTracer.Category, "MSExchange Front End HTTP Proxy");

		public delegate void LastChanceExceptionHandler(Exception unhandledException);
	}
}
