using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.InstantMessaging;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public sealed class OwaDiagnostics
	{
		private OwaDiagnostics()
		{
		}

		public static ExEventLog Logger
		{
			get
			{
				return OwaDiagnostics.logger.Value;
			}
		}

		public static bool LogEvent(ExEventLog.EventTuple tuple, string periodicKey, params object[] eventLogParams)
		{
			return OwaDiagnostics.Logger.LogEvent(tuple, periodicKey, eventLogParams);
		}

		public static bool LogEvent(ExEventLog.EventTuple tuple)
		{
			return OwaDiagnostics.Logger.LogEvent(tuple, string.Empty, new object[0]);
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition, string format, params object[] parameters)
		{
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition)
		{
		}

		public static void TracePfd(int lid, string message, params object[] parameters)
		{
			if (!ExTraceGlobals.CoreTracer.IsTraceEnabled(TraceType.PfdTrace))
			{
				return;
			}
			ExTraceGlobals.CoreTracer.TracePfd((long)lid, string.Concat(new object[]
			{
				"PFD OWA ",
				lid,
				" - ",
				message
			}), parameters);
		}

		public static void PublishMonitoringEventNotification(string serviceName, string component, string message, ResultSeverityLevel severity)
		{
			if (Globals.IsPreCheckinApp)
			{
				return;
			}
			if (component == Feature.InstantMessage.ToString())
			{
				if (severity == ResultSeverityLevel.Error || severity == ResultSeverityLevel.Critical)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError(0L, message);
				}
				else if (severity == ResultSeverityLevel.Warning)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceWarning(0L, message);
				}
				else
				{
					ExTraceGlobals.InstantMessagingTracer.TraceInformation(0, 0L, message);
				}
			}
			EventNotificationItem eventNotificationItem = new EventNotificationItem(serviceName, component, null, message, severity);
			eventNotificationItem.Publish(false);
		}

		public static void SendWatsonReportsForGrayExceptions(GrayException.UserCodeDelegate tryCode, Func<Exception, bool> ignoreFunc)
		{
			GrayException.MapAndReportGrayExceptions(tryCode, delegate(Exception exception)
			{
				if (ignoreFunc(exception))
				{
					ExWatson.SetWatsonReportAlreadySent(exception);
					return true;
				}
				return GrayException.IsGrayException(exception);
			});
		}

		public static void SendWatsonReportsForGrayExceptions(GrayException.UserCodeDelegate tryCode)
		{
			GrayException.MapAndReportGrayExceptions(tryCode, delegate(Exception exception)
			{
				if (OwaDiagnostics.CanIgnoreExceptionForWatsonReport(exception))
				{
					ExWatson.SetWatsonReportAlreadySent(exception);
					return true;
				}
				return GrayException.IsGrayException(exception);
			});
		}

		public static bool CanIgnoreExceptionForWatsonReport(Exception exception)
		{
			return exception is InstantMessagingException || exception is OwaPermanentException || exception is OwaTransientException || exception is StoragePermanentException || exception is StorageTransientException || exception is ADTransientException || exception is DataValidationException || exception is DataSourceOperationException;
		}

		internal static void CheckAndSetThreadTracing(string userDN)
		{
			if (userDN == null)
			{
				throw new ArgumentNullException("userDN");
			}
			ExTraceConfiguration instance = ExTraceConfiguration.Instance;
			lock (OwaDiagnostics.traceLock)
			{
				if (OwaDiagnostics.traceVersion != instance.Version)
				{
					OwaDiagnostics.traceVersion = instance.Version;
					OwaDiagnostics.traceUserTable = null;
					List<string> list = null;
					if (instance.CustomParameters.TryGetValue(OwaDiagnostics.userDNKeyName, out list) && list != null)
					{
						OwaDiagnostics.traceUserTable = new Hashtable(list.Count, StringComparer.OrdinalIgnoreCase);
						for (int i = 0; i < list.Count; i++)
						{
							OwaDiagnostics.traceUserTable[list[i]] = list[i];
						}
					}
				}
				if (instance.PerThreadTracingConfigured && OwaDiagnostics.traceUserTable != null && OwaDiagnostics.traceUserTable[userDN] != null)
				{
					BaseTrace.CurrentThreadSettings.EnableTracing();
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Enabled filtered tracing for user DN = '{0}'", userDN);
				}
			}
		}

		internal static void ClearThreadTracing()
		{
			if (BaseTrace.CurrentThreadSettings.IsEnabled)
			{
				BaseTrace.CurrentThreadSettings.DisableTracing();
			}
		}

		private static readonly string userDNKeyName = "UserLegacyDN";

		private static Lazy<ExEventLog> logger = new Lazy<ExEventLog>(() => new ExEventLog(ExTraceGlobals.CoreTracer.Category, "MSExchange OWA"));

		private static int traceVersion = -1;

		private static object traceLock = new object();

		private static Hashtable traceUserTable;
	}
}
