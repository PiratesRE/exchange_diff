using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.Diagnostics.FaultInjection;
using Microsoft.Exchange.Diagnostics.LatencyDetection;

namespace Microsoft.Exchange.AirSync
{
	internal static class AirSyncDiagnostics
	{
		public static ExEventLog EventLogger
		{
			get
			{
				return AirSyncDiagnostics.eventLogger;
			}
		}

		public static LatencyDetectionContextFactory AirSyncLatencyDetectionContextFactory
		{
			get
			{
				return AirSyncDiagnostics.airSyncLatencyDetectionContextFactory;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (AirSyncDiagnostics.faultInjectionTracer == null)
				{
					AirSyncDiagnostics.faultInjectionTracer = new FaultInjectionTrace(AirSyncDiagnostics.airSyncComponentGuid, AirSyncDiagnostics.tagFaultInjection);
					AirSyncDiagnostics.faultInjectionTracer.RegisterExceptionInjectionCallback(new ExceptionInjectionCallback(AirSyncDiagnostics.Callback));
				}
				return AirSyncDiagnostics.faultInjectionTracer;
			}
		}

		public static TroubleshootingContext TroubleshootingContext
		{
			get
			{
				if (AirSyncDiagnostics.troubleshootingContext == null)
				{
					AirSyncDiagnostics.troubleshootingContext = new TroubleshootingContext("MSExchange ActiveSync");
				}
				return AirSyncDiagnostics.troubleshootingContext;
			}
		}

		public static bool CheckAndSetThreadTracing(string userDN)
		{
			if (userDN == null)
			{
				return false;
			}
			if (AirSyncDiagnostics.traceVersion != ExTraceConfiguration.Instance.Version)
			{
				AirSyncDiagnostics.LoadThreadTracingConfig();
			}
			if (!AirSyncDiagnostics.traceConfig.PerThreadTracingConfigured)
			{
				return false;
			}
			if (AirSyncDiagnostics.userDNList == null || !AirSyncDiagnostics.userDNList.Contains(userDN))
			{
				return false;
			}
			BaseTrace.CurrentThreadSettings.EnableTracing();
			return true;
		}

		public static void ClearThreadTracing()
		{
			if (BaseTrace.CurrentThreadSettings.IsEnabled)
			{
				BaseTrace.CurrentThreadSettings.DisableTracing();
			}
		}

		public static void SetThreadTracing()
		{
			if (AirSyncDiagnostics.traceConfig.PerThreadTracingConfigured)
			{
				BaseTrace.CurrentThreadSettings.EnableTracing();
			}
		}

		public static void LogPeriodicEvent(ExEventLog.EventTuple tuple, string eventKey, params string[] messageArgs)
		{
			AirSyncDiagnostics.TraceInfo<uint, string>(ExTraceGlobals.ProtocolTracer, null, "LogPeriodicEvent eventId:{0} eventKey:{1}", tuple.EventId, eventKey);
			if (messageArgs != null)
			{
				for (int i = 0; i < messageArgs.Length; i++)
				{
					if (messageArgs[i].Length > 32000)
					{
						messageArgs[i] = messageArgs[i].Remove(32000).TrimEnd(new char[0]);
					}
				}
			}
			if (!AirSyncDiagnostics.eventLogger.LogEvent(tuple, eventKey, messageArgs))
			{
				AirSyncDiagnostics.TraceError<uint>(ExTraceGlobals.ProtocolTracer, null, "Failed to log periodic event {0}", tuple.EventId);
			}
		}

		public static void LogEvent(ExEventLog.EventTuple tuple, params string[] messageArgs)
		{
			AirSyncDiagnostics.TraceInfo<uint>(ExTraceGlobals.ProtocolTracer, null, "LogEvent eventId:{0}", tuple.EventId);
			if (messageArgs != null)
			{
				for (int i = 0; i < messageArgs.Length; i++)
				{
					if (messageArgs[i].Length > 32000)
					{
						messageArgs[i] = messageArgs[i].Remove(32000).TrimEnd(new char[0]);
					}
				}
			}
			if (TestHooks.EventLog_LogEvent != null)
			{
				TestHooks.EventLog_LogEvent(tuple, null, messageArgs);
				return;
			}
			if (!AirSyncDiagnostics.eventLogger.LogEvent(tuple, null, messageArgs))
			{
				AirSyncDiagnostics.TraceError<uint>(ExTraceGlobals.ProtocolTracer, null, "Failed to log event {0}", tuple.EventId);
			}
		}

		public static void Assert(bool condition, string format, params object[] parameters)
		{
		}

		public static void Assert(bool condition)
		{
		}

		public static void InMemoryTraceOperationCompleted()
		{
			if (AirSyncDiagnostics.IsInMemoryTracingEnabled())
			{
				AirSyncDiagnostics.TroubleshootingContext.TraceOperationCompletedAndUpdateContext();
			}
		}

		public static void SendWatson(Exception exception)
		{
			AirSyncDiagnostics.SendWatson(exception, true);
		}

		public static void SendWatson(Exception exception, bool terminating)
		{
			AirSyncDiagnostics.DoWithExtraWatsonData(delegate
			{
				if (AirSyncDiagnostics.IsInMemoryTracingEnabled())
				{
					AirSyncDiagnostics.TroubleshootingContext.TraceOperationCompletedAndUpdateContext();
					AirSyncDiagnostics.TroubleshootingContext.SendExceptionReportWithTraces(exception, terminating);
					return;
				}
				if (exception != TroubleshootingContext.FaultInjectionInvalidOperationException)
				{
					ExWatson.SendReport(exception, terminating ? ReportOptions.ReportTerminateAfterSend : ReportOptions.None, null);
				}
			});
		}

		private static void DoWithExtraWatsonData(Action action)
		{
			Command currentCommand = Command.CurrentCommand;
			string text = null;
			if (currentCommand != null && GlobalSettings.IncludeRequestInWatson)
			{
				text = "Request: \r\n" + currentCommand.Request.GetHeadersAsString();
				if (currentCommand.Request.XmlDocument != null && currentCommand.Request.XmlDocument.DocumentElement != null)
				{
					text = text + "\r\n" + currentCommand.Request.XmlDocument.DocumentElement.OuterXml;
				}
				else
				{
					text += "\r\n[No Body]";
				}
			}
			WatsonExtraDataReportAction watsonExtraDataReportAction = string.IsNullOrEmpty(text) ? null : new WatsonExtraDataReportAction(text);
			if (watsonExtraDataReportAction != null)
			{
				ExWatson.RegisterReportAction(watsonExtraDataReportAction, WatsonActionScope.Thread);
			}
			try
			{
				action();
			}
			finally
			{
				if (watsonExtraDataReportAction != null)
				{
					ExWatson.UnregisterReportAction(watsonExtraDataReportAction, WatsonActionScope.Thread);
				}
			}
		}

		public static void SendInMemoryTraceWatson(Exception exception)
		{
			AirSyncDiagnostics.DoWithExtraWatsonData(delegate
			{
				if (AirSyncDiagnostics.IsInMemoryTracingEnabled())
				{
					AirSyncDiagnostics.TroubleshootingContext.TraceOperationCompletedAndUpdateContext();
					AirSyncDiagnostics.TroubleshootingContext.SendTroubleshootingReportWithTraces(exception);
					return;
				}
				if (exception != TroubleshootingContext.FaultInjectionInvalidOperationException)
				{
					ExWatson.SendReport(exception, ReportOptions.DoNotCollectDumps, null);
				}
			});
		}

		public static ISyncLogger GetSyncLogger()
		{
			IAirSyncContext airSyncContext = (Command.CurrentCommand == null) ? null : Command.CurrentCommand.Context;
			if (airSyncContext != null)
			{
				return airSyncContext;
			}
			return TracingLogger.Singleton;
		}

		public static void TraceInfo(Trace tracer, object objectToHash, string message)
		{
			AirSyncDiagnostics.GetSyncLogger().Information(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), message);
		}

		public static void TraceInfo(Trace tracer, object objectToHash, string formatString, params object[] args)
		{
			AirSyncDiagnostics.GetSyncLogger().Information(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, args);
		}

		public static void TraceInfo<T0>(Trace tracer, object objectToHash, string formatString, T0 arg0)
		{
			AirSyncDiagnostics.GetSyncLogger().Information<T0>(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0);
		}

		public static void TraceInfo<T0, T1>(Trace tracer, object objectToHash, string formatString, T0 arg0, T1 arg1)
		{
			AirSyncDiagnostics.GetSyncLogger().Information<T0, T1>(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0, arg1);
		}

		public static void TraceInfo<T0, T1, T2>(Trace tracer, object objectToHash, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			AirSyncDiagnostics.GetSyncLogger().Information<T0, T1, T2>(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0, arg1, arg2);
		}

		public static void TraceDebug(Trace tracer, object objectToHash, string message)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceDebug(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), message);
		}

		public static void TraceDebug(Trace tracer, int lid, object objectToHash, string message)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceDebug(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), message);
		}

		public static void TraceDebug(Trace tracer, object objectToHash, string formatString, params object[] args)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceDebug(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, args);
		}

		public static void TraceDebug<T0>(Trace tracer, object objectToHash, string formatString, T0 arg0)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceDebug<T0>(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0);
		}

		public static void TraceDebug(Trace tracer, int lid, object objectToHash, string formatString, params object[] args)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceDebug(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, args);
		}

		public static void TraceDebug<T0>(Trace tracer, int lid, object objectToHash, string formatString, T0 arg0)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceDebug<T0>(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0);
		}

		public static void TraceDebug<T0, T1>(Trace tracer, object objectToHash, string formatString, T0 arg0, T1 arg1)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceDebug<T0, T1>(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0, arg1);
		}

		public static void TraceDebug<T0, T1>(Trace tracer, int lid, object objectToHash, string formatString, T0 arg0, T1 arg1)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceDebug<T0, T1>(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0, arg1);
		}

		public static void TraceDebug<T0, T1, T2>(Trace tracer, object objectToHash, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceDebug<T0, T1, T2>(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0, arg1, arg2);
		}

		public static void TraceDebug<T0, T1, T2>(Trace tracer, int lid, object objectToHash, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceDebug<T0, T1, T2>(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0, arg1, arg2);
		}

		public static void TraceError(Trace tracer, object objectToHash, string message)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceError(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), message);
		}

		public static void TraceError(Trace tracer, int lid, object objectToHash, string message)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceError(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), message);
		}

		public static void TraceError(Trace tracer, object objectToHash, string formatString, params object[] args)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceError(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, args);
		}

		public static void TraceError<T0>(Trace tracer, object objectToHash, string formatString, T0 arg0)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceError<T0>(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0);
		}

		public static void TraceError(Trace tracer, int lid, object objectToHash, string formatString, params object[] args)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceError(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, args);
		}

		public static void TraceError<T0>(Trace tracer, int lid, object objectToHash, string formatString, T0 arg0)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceError<T0>(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0);
		}

		public static void TraceError<T0, T1>(Trace tracer, object objectToHash, string formatString, T0 arg0, T1 arg1)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceError<T0, T1>(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0, arg1);
		}

		public static void TraceError<T0, T1>(Trace tracer, int lid, object objectToHash, string formatString, T0 arg0, T1 arg1)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceError<T0, T1>(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0, arg1);
		}

		public static void TraceError<T0, T1, T2>(Trace tracer, object objectToHash, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceError<T0, T1, T2>(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0, arg1, arg2);
		}

		public static void TraceError<T0, T1, T2>(Trace tracer, int lid, object objectToHash, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceError<T0, T1, T2>(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0, arg1, arg2);
		}

		public static void TraceFunction(Trace tracer, object objectToHash, string message)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceFunction(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), message);
		}

		public static void TraceFunction(Trace tracer, int lid, object objectToHash, string message)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceFunction(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), message);
		}

		public static void TraceFunction(Trace tracer, object objectToHash, string formatString, params object[] args)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceFunction(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, args);
		}

		public static void TraceFunction<T0>(Trace tracer, object objectToHash, string formatString, T0 arg0)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceFunction<T0>(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0);
		}

		public static void TraceFunction(Trace tracer, int lid, object objectToHash, string formatString, params object[] args)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceFunction(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, args);
		}

		public static void TraceFunction<T0>(Trace tracer, int lid, object objectToHash, string formatString, T0 arg0)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceFunction<T0>(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0);
		}

		public static void TraceFunction<T0, T1>(Trace tracer, object objectToHash, string formatString, T0 arg0, T1 arg1)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceFunction<T0, T1>(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0, arg1);
		}

		public static void TraceFunction<T0, T1>(Trace tracer, int lid, object objectToHash, string formatString, T0 arg0, T1 arg1)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceFunction<T0, T1>(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0, arg1);
		}

		public static void TraceFunction<T0, T1, T2>(Trace tracer, object objectToHash, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceFunction<T0, T1, T2>(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0, arg1, arg2);
		}

		public static void TraceFunction<T0, T1, T2>(Trace tracer, int lid, object objectToHash, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceFunction<T0, T1, T2>(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0, arg1, arg2);
		}

		public static void TracePfd(Trace tracer, object objectToHash, string message)
		{
			AirSyncDiagnostics.GetSyncLogger().TracePfd(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), message);
		}

		public static void TracePfd(Trace tracer, int lid, object objectToHash, string message)
		{
			AirSyncDiagnostics.GetSyncLogger().TracePfd(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), message);
		}

		public static void TracePfd(Trace tracer, object objectToHash, string formatString, params object[] args)
		{
			AirSyncDiagnostics.GetSyncLogger().TracePfd(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, args);
		}

		public static void TracePfd<T0>(Trace tracer, object objectToHash, string formatString, T0 arg0)
		{
			AirSyncDiagnostics.GetSyncLogger().TracePfd<T0>(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0);
		}

		public static void TracePfd(Trace tracer, int lid, object objectToHash, string formatString, params object[] args)
		{
			AirSyncDiagnostics.GetSyncLogger().TracePfd(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, args);
		}

		public static void TracePfd<T0>(Trace tracer, int lid, object objectToHash, string formatString, T0 arg0)
		{
			AirSyncDiagnostics.GetSyncLogger().TracePfd<T0>(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0);
		}

		public static void TracePfd<T0, T1>(Trace tracer, object objectToHash, string formatString, T0 arg0, T1 arg1)
		{
			AirSyncDiagnostics.GetSyncLogger().TracePfd<T0, T1>(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0, arg1);
		}

		public static void TracePfd<T0, T1>(Trace tracer, int lid, object objectToHash, string formatString, T0 arg0, T1 arg1)
		{
			AirSyncDiagnostics.GetSyncLogger().TracePfd<T0, T1>(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0, arg1);
		}

		public static void TracePfd<T0, T1, T2>(Trace tracer, object objectToHash, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			AirSyncDiagnostics.GetSyncLogger().TracePfd<T0, T1, T2>(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0, arg1, arg2);
		}

		public static void TracePfd<T0, T1, T2>(Trace tracer, int lid, object objectToHash, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			AirSyncDiagnostics.GetSyncLogger().TracePfd<T0, T1, T2>(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0, arg1, arg2);
		}

		public static void TraceWarning(Trace tracer, object objectToHash, string message)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceWarning(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), message);
		}

		public static void TraceWarning(Trace tracer, int lid, object objectToHash, string message)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceWarning(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), message);
		}

		public static void TraceWarning(Trace tracer, object objectToHash, string formatString, params object[] args)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceWarning(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, args);
		}

		public static void TraceWarning<T0>(Trace tracer, object objectToHash, string formatString, T0 arg0)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceWarning<T0>(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0);
		}

		public static void TraceWarning(Trace tracer, int lid, object objectToHash, string formatString, params object[] args)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceWarning(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, args);
		}

		public static void TraceWarning<T0>(Trace tracer, int lid, object objectToHash, string formatString, T0 arg0)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceWarning<T0>(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0);
		}

		public static void TraceWarning<T0, T1>(Trace tracer, object objectToHash, string formatString, T0 arg0, T1 arg1)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceWarning<T0, T1>(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0, arg1);
		}

		public static void TraceWarning<T0, T1>(Trace tracer, int lid, object objectToHash, string formatString, T0 arg0, T1 arg1)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceWarning<T0, T1>(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0, arg1);
		}

		public static void TraceWarning<T0, T1, T2>(Trace tracer, object objectToHash, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceWarning<T0, T1, T2>(tracer, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0, arg1, arg2);
		}

		public static void TraceWarning<T0, T1, T2>(Trace tracer, int lid, object objectToHash, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			AirSyncDiagnostics.GetSyncLogger().TraceWarning<T0, T1, T2>(tracer, lid, (long)((objectToHash == null) ? 0 : objectToHash.GetHashCode()), formatString, arg0, arg1, arg2);
		}

		public static bool IsTraceEnabled(TraceType traceType, Trace tracer)
		{
			return tracer.IsTraceEnabled(traceType);
		}

		public static void TraceBinaryData(Trace tracer, object obj, byte[] bytes, int length)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (!tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(48);
			StringBuilder stringBuilder2 = new StringBuilder(16);
			int i = 0;
			while (i < length)
			{
				stringBuilder.Length = 0;
				stringBuilder2.Length = 0;
				int j = 0;
				while (i < length)
				{
					if (j >= 16)
					{
						break;
					}
					stringBuilder.Append(bytes[i].ToString("X2", CultureInfo.InvariantCulture));
					stringBuilder.Append(' ');
					stringBuilder2.Append((char)((bytes[i] < 32 || bytes[i] > 127) ? 46 : bytes[i]));
					j++;
					i++;
				}
				while (j < 16)
				{
					stringBuilder.Append("   ");
					stringBuilder2.Append(' ');
					j++;
				}
				AirSyncDiagnostics.TraceDebug<StringBuilder, StringBuilder>(tracer, obj, "{0} {1}", stringBuilder, stringBuilder2);
			}
		}

		public static void TraceXmlBody(Trace tracer, object obj, XmlDocument xml)
		{
			if (xml == null)
			{
				throw new ArgumentNullException("xml");
			}
			if (!tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				return;
			}
			string text = AirSyncUtility.BuildOuterXml(xml);
			string[] separator = new string[]
			{
				"\r\n"
			};
			string[] array = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			foreach (string arg in array)
			{
				AirSyncDiagnostics.TraceDebug<string>(tracer, obj, "    {0}", arg);
			}
		}

		public static bool IsInMemoryTracingEnabled()
		{
			if (AirSyncDiagnostics.traceVersion != ExTraceConfiguration.Instance.Version)
			{
				AirSyncDiagnostics.LoadThreadTracingConfig();
			}
			return AirSyncDiagnostics.traceConfig.InMemoryTracingEnabled;
		}

		public static T TraceTest<T>(uint faultLid)
		{
			T result = default(T);
			AirSyncDiagnostics.FaultInjectionTracer.TraceTest<T>(faultLid, ref result);
			return result;
		}

		public static void FaultInjectionPoint(uint faultLid, Action productAction, Action faultInjectionAction)
		{
			if (AirSyncDiagnostics.TraceTest<bool>(faultLid))
			{
				faultInjectionAction();
				return;
			}
			productAction();
		}

		private static void LoadThreadTracingConfig()
		{
			AirSyncDiagnostics.traceConfig = ExTraceConfiguration.Instance;
			AirSyncDiagnostics.traceVersion = AirSyncDiagnostics.traceConfig.Version;
			AirSyncDiagnostics.traceConfig.CustomParameters.TryGetValue("UserDN", out AirSyncDiagnostics.userDNList);
		}

		private static Exception Callback(string exceptionType)
		{
			Exception result = null;
			if (exceptionType != null)
			{
				if (exceptionType.Equals("System.FormatException", StringComparison.OrdinalIgnoreCase))
				{
					result = Constants.FaultInjectionFormatException;
				}
				else if (exceptionType.Equals("System.InvalidOperationExceptionInMemory", StringComparison.OrdinalIgnoreCase))
				{
					result = TroubleshootingContext.FaultInjectionInvalidOperationException;
				}
			}
			return result;
		}

		public const string AssemblyVersion = "15.00.1497.012";

		private const string AirSyncComponent = "MSExchange ActiveSync";

		private const string UserDNParamString = "UserDN";

		private const int MaxEventLogPerStringLength = 32000;

		private static readonly TimeSpan DefaultMinAirSyncThreshold = TimeSpan.FromMinutes(3.0);

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.RequestsTracer.Category, "MSExchange ActiveSync");

		private static ExTraceConfiguration traceConfig;

		private static int traceVersion = -1;

		private static List<string> userDNList;

		private static LatencyDetectionContextFactory airSyncLatencyDetectionContextFactory = LatencyDetectionContextFactory.CreateFactory("AirSync", AirSyncDiagnostics.DefaultMinAirSyncThreshold, AirSyncDiagnostics.DefaultMinAirSyncThreshold);

		private static FaultInjectionTrace faultInjectionTracer;

		private static Guid airSyncComponentGuid = new Guid("5e88fb2c-0a36-41f2-a710-c911bfe18e44");

		private static int tagFaultInjection = 14;

		private static TroubleshootingContext troubleshootingContext;
	}
}
