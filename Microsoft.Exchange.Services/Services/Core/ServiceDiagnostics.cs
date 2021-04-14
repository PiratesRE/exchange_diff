using System;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.EventLogs;

namespace Microsoft.Exchange.Services.Core
{
	internal static class ServiceDiagnostics
	{
		internal static ExEventLog Logger
		{
			get
			{
				return ServiceDiagnostics.logger.Value;
			}
		}

		internal static bool LogExceptionWithTrace(ExEventLog.EventTuple tuple, string periodicKey, Microsoft.Exchange.Diagnostics.Trace tagTracer, object thisObject, string traceFormat, Exception exception)
		{
			tagTracer.TraceError<Exception>((long)((thisObject == null) ? 0 : thisObject.GetHashCode()), traceFormat, exception);
			string text = exception.ToString();
			if (text.Length > 32000)
			{
				text = text.Substring(0, 2000) + "...\n" + text.Substring(text.Length - 20000, 20000);
			}
			return ServiceDiagnostics.Logger.LogEvent(tuple, periodicKey, new object[]
			{
				text
			});
		}

		internal static bool LogEventWithTrace(ExEventLog.EventTuple tuple, string periodicKey, Microsoft.Exchange.Diagnostics.Trace tagTracer, object thisObject, string traceFormat, params object[] messageArgs)
		{
			tagTracer.TraceDebug((long)((thisObject == null) ? 0 : thisObject.GetHashCode()), traceFormat, messageArgs);
			return ServiceDiagnostics.Logger.LogEvent(tuple, periodicKey, messageArgs);
		}

		[Conditional("DEBUG")]
		internal static void Assert(bool condition, string format, params object[] parameters)
		{
		}

		[Conditional("DEBUG")]
		internal static void Assert(bool condition)
		{
		}

		internal static bool IsNonEmptyArray(Array array)
		{
			return array != null && array.Length != 0;
		}

		internal static object HandleNullObjectTrace(object objectToDisplay)
		{
			return objectToDisplay ?? "Object is null";
		}

		internal static void SendWatsonReportOnUnhandledException(ExWatson.MethodDelegate methodDelegate)
		{
			ServiceDiagnostics.RegisterAdditionalWatsonData();
			try
			{
				ExWatson.SendReportOnUnhandledException(methodDelegate, delegate(object exception)
				{
					bool flag = ServiceDiagnostics.sendWatsonReportsEnabled;
					Exception ex = exception as Exception;
					if (ex != null)
					{
						ExTraceGlobals.ExceptionTracer.TraceError<Exception>(0L, "Encountered unhandled exception: {0}", ex);
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(RequestDetailsLogger.Current, ex, "ServiceDiagnostics_SendWatsonReportOnUnhandledException");
						flag = ServiceDiagnostics.IsSendReportValid(ex);
						if (flag)
						{
							ServiceDiagnostics.LogExceptionWithTrace(ServicesEventLogConstants.Tuple_InternalServerError, null, ExTraceGlobals.ExceptionTracer, null, "Encountered unhandled exception: {0}", ex);
							ExWatson.SetWatsonReportAlreadySent(ex);
						}
					}
					ExTraceGlobals.ExceptionTracer.TraceError<bool>(0L, "SendWatsonReportOnUnhandledException isSendReportValid: {0}", flag);
					return flag;
				}, ReportOptions.None);
			}
			catch (ThreadAbortException)
			{
				Thread.ResetAbort();
				throw new BailOutException();
			}
			finally
			{
				ExWatson.ClearReportActions(WatsonActionScope.Thread);
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
				Exception ex = exception as Exception;
				if (ex != null)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(RequestDetailsLogger.Current, ex, "ServiceDiagnostics_TraceErrorOnUnhandledException");
				}
				return false;
			});
		}

		internal static void SendWatsonReportOnUnhandledExceptionIfRequired(ExWatson.MethodDelegate methodDelegate)
		{
			ServiceDiagnostics.RegisterAdditionalWatsonData();
			try
			{
				ExWatson.SendReportOnUnhandledException(methodDelegate, delegate(object exception)
				{
					bool flag = ServiceDiagnostics.sendWatsonReportsEnabled;
					Exception ex = exception as Exception;
					if (ex != null)
					{
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(RequestDetailsLogger.Current, ex, "ServiceDiagnostics_SendWatsonReportOnUnhandledExceptionIfRequired");
						flag = ServiceDiagnostics.IsSendReportValid(ex);
						if (ex.Data != null)
						{
							if (ex.Data.Contains("NeverGenerateWatson"))
							{
								flag = false;
							}
							else
							{
								LocalizedException ex2 = ex as LocalizedException;
								if (ex2 != null)
								{
									if (ex2 is ServicePermanentException)
									{
										flag = false;
									}
									else
									{
										ExceptionMappingBase exceptionMapping = ServiceErrors.GetExceptionMapper().GetExceptionMapping(ex2);
										if (!exceptionMapping.ReportException)
										{
											flag = false;
										}
									}
								}
							}
							ExWatson.SetWatsonReportAlreadySent(ex);
							ex.Data["DisposeServiceCommmandTag"] = null;
						}
						if (flag)
						{
							ServiceDiagnostics.LogExceptionWithTrace(ServicesEventLogConstants.Tuple_InternalServerError, null, ExTraceGlobals.ExceptionTracer, null, "Encountered unhandled exception: {0}", ex);
						}
						ExTraceGlobals.ExceptionTracer.TraceError<bool, string, string>(0L, "[ServiceDiagnostics::SendWatsonReportOnUnhandledExceptionIfRequired] Encountered unhandled exception.  Watson generated? {0}, Class: '{1}', Message: '{2}'", flag, ex.GetType().FullName, ex.Message);
					}
					return flag;
				}, ReportOptions.None);
			}
			catch (ThreadAbortException)
			{
				Thread.ResetAbort();
				throw new BailOutException();
			}
			finally
			{
				ExWatson.ClearReportActions(WatsonActionScope.Thread);
			}
		}

		internal static bool DoesExceptionDataContainTag(Exception exception, string dataTag)
		{
			while (exception != null)
			{
				if (exception.Data != null && exception.Data.Contains(dataTag))
				{
					return true;
				}
				exception = exception.InnerException;
			}
			return false;
		}

		internal static void InitializeWatsonReporting(bool sendWatsonReports, bool filterExceptionsFromWatsonReporting)
		{
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<bool, bool>(0L, "sendWatsonReports: {0} filterExceptionsFromWatsonReporting: {1}", sendWatsonReports, filterExceptionsFromWatsonReporting);
			ExWatson.Register(ExEnvironment.IsTest ? "E12" : "E12IIS");
			ServiceDiagnostics.sendWatsonReportsEnabled = sendWatsonReports;
			ServiceDiagnostics.watsonReportsFiltered = filterExceptionsFromWatsonReporting;
		}

		internal static void ReportException(Exception exception, ExEventLog.EventTuple eventTuple, object eventObject, string traceFormat)
		{
			bool flag = exception is AccessViolationException;
			if (exception != null)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(RequestDetailsLogger.Current, exception, "ServiceDiagnostics_ReportException");
			}
			if (ServiceDiagnostics.IsSendReportValid(exception))
			{
				ServiceDiagnostics.LogExceptionWithTrace(eventTuple, null, ExTraceGlobals.ExceptionTracer, eventObject, traceFormat, exception);
				ExWatson.HandleException(new UnhandledExceptionEventArgs(exception, flag), ReportOptions.None);
				ExWatson.SetWatsonReportAlreadySent(exception);
			}
			else
			{
				ExTraceGlobals.ExceptionTracer.TraceError<Exception>(0L, traceFormat, exception);
			}
			if (flag)
			{
				Environment.FailFast(traceFormat, exception);
			}
		}

		private static bool IsFaultInjectionException(Exception exception)
		{
			return exception != null && exception.StackTrace != null && exception.StackTrace.Contains("Microsoft.Exchange.Diagnostics.FaultInjection.FaultInjectionTrace.InjectException");
		}

		private static bool IsSendReportValid(Exception exception)
		{
			if (ServiceDiagnostics.IsFaultInjectionException(exception))
			{
				return false;
			}
			if (ExWatson.IsWatsonReportAlreadySent(exception))
			{
				return false;
			}
			bool flag = ServiceDiagnostics.sendWatsonReportsEnabled;
			if (flag && ServiceDiagnostics.watsonReportsFiltered)
			{
				if (exception is CommunicationException)
				{
					flag = false;
				}
				else if (exception is HttpException)
				{
					flag = false;
				}
				else if (!ServiceDiagnostics.ReportTimeoutException && exception is TimeoutException)
				{
					flag = false;
				}
				else if (exception is BailOutException)
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
				else if (exception.GetType().FullName == "System.ServiceModel.Diagnostics.PlainXmlWriter+MaxSizeExceededException")
				{
					flag = false;
				}
				else if (exception is FaultException)
				{
					FaultException ex = (FaultException)exception;
					if (EWSSettings.IsFromEwsAssemblies(exception.Source))
					{
						flag = false;
					}
					else if (EWSSettings.FaultExceptionDueToAuthorizationManager)
					{
						flag = false;
					}
					else if (ex.Code != null && ex.Code.SubCode != null)
					{
						if (ex.Code.SubCode.Name == "DestinationUnreachable")
						{
							flag = false;
						}
						else if (ex.Code.SubCode.Name == "ActionNotSupported")
						{
							flag = false;
						}
					}
				}
				else if ((exception is UriFormatException || exception is XmlException) && !EWSSettings.IsFromEwsAssemblies(exception.Source))
				{
					flag = false;
				}
			}
			ExTraceGlobals.ExceptionTracer.TraceDebug<bool>(0L, "IsSendReportValid isSendReportValid: {0}", flag);
			return flag;
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

		internal static void RegisterAdditionalWatsonData()
		{
			string watsonExtraData = ServiceDiagnostics.GetWatsonExtraData();
			ExWatson.ClearReportActions(WatsonActionScope.Thread);
			ExWatson.RegisterReportAction(new WatsonExtraDataReportAction(watsonExtraData), WatsonActionScope.Thread);
		}

		private static string GetWatsonExtraData()
		{
			return string.Format("[TID:{0};Action:{1};RC:{2};Caller:{3}]", new object[]
			{
				EWSSettings.RequestThreadId,
				(CallContext.Current == null || CallContext.Current.SoapAction == null) ? "<NULL>" : CallContext.Current.SoapAction,
				EWSSettings.RequestCorrelation,
				(CallContext.Current == null || CallContext.Current.Budget == null) ? "<NULL>" : CallContext.Current.Budget.Owner.ToString()
			});
		}

		internal static bool ReportTimeoutException
		{
			get
			{
				return ServiceDiagnostics.reportTimeoutException;
			}
			set
			{
				ServiceDiagnostics.reportTimeoutException = value;
			}
		}

		private const string DisposeServiceCommmandTag = "DisposeServiceCommmandTag";

		private const string FaultInjectionFrame = "Microsoft.Exchange.Diagnostics.FaultInjection.FaultInjectionTrace.InjectException";

		private const string EventSource = "MSExchange Web Services";

		internal const string NeverGenerateWatsonCommmandTag = "NeverGenerateWatson";

		private static Lazy<ExEventLog> logger = new Lazy<ExEventLog>(() => new ExEventLog(ExTraceGlobals.PerformanceMonitorTracer.Category, "MSExchange Web Services"));

		private static bool sendWatsonReportsEnabled = true;

		private static bool watsonReportsFiltered = true;

		private static bool reportTimeoutException = true;
	}
}
