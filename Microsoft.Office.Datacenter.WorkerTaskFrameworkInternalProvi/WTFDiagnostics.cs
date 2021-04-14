using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Diagnostics.Components.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public static class WTFDiagnostics
	{
		public static ExEventLog EventLogger
		{
			get
			{
				return WTFDiagnostics.eventLogger;
			}
		}

		private static TroubleshootingContext TroubleshootingContext
		{
			get
			{
				if (WTFDiagnostics.troubleshootingContext == null)
				{
					WTFDiagnostics.troubleshootingContext = new TroubleshootingContext("Active Monitoring");
				}
				return WTFDiagnostics.troubleshootingContext;
			}
		}

		public static void InMemoryTraceOperationCompleted()
		{
			if (ExTraceConfiguration.Instance.InMemoryTracingEnabled)
			{
				WTFDiagnostics.TroubleshootingContext.TraceOperationCompletedAndUpdateContext();
			}
		}

		public static void SendWatson(Exception exception)
		{
			WTFDiagnostics.SendWatson(exception, true);
		}

		public static void SendWatson(Exception exception, bool terminating)
		{
			WTFLogger.Instance.Flush();
			if (ExTraceConfiguration.Instance.InMemoryTracingEnabled)
			{
				WTFDiagnostics.TroubleshootingContext.TraceOperationCompletedAndUpdateContext();
				WTFDiagnostics.TroubleshootingContext.SendExceptionReportWithTraces(exception, terminating);
				return;
			}
			if (exception != TroubleshootingContext.FaultInjectionInvalidOperationException)
			{
				ExWatson.SendReport(exception, terminating ? ReportOptions.ReportTerminateAfterSend : ReportOptions.None, null);
			}
		}

		public static void SendInMemoryTraceWatson(Exception exception)
		{
			if (ExTraceConfiguration.Instance.InMemoryTracingEnabled)
			{
				WTFDiagnostics.TroubleshootingContext.TraceOperationCompletedAndUpdateContext();
				WTFDiagnostics.TroubleshootingContext.SendTroubleshootingReportWithTraces(exception);
				return;
			}
			if (exception != TroubleshootingContext.FaultInjectionInvalidOperationException)
			{
				ExWatson.SendReport(exception, ReportOptions.DoNotCollectDumps, null);
			}
		}

		public static bool IsTraceEnabled(TraceType traceType, Trace tracer, TracingContext context)
		{
			return context != null && !context.IsDisabled && tracer.IsTraceEnabled(traceType);
		}

		public static WTFLogComponent MapTracerToLogComponent(Trace tracer)
		{
			WTFLogComponent wtflogComponent = null;
			if (!WTFDiagnostics.tracerToLogComponentMap.TryGetValue(tracer, out wtflogComponent))
			{
				if (WTFDiagnostics.IsLogComponentEnabled(tracer))
				{
					wtflogComponent = new WTFLogComponent(tracer.Category, tracer.TraceTag, string.Empty, true);
				}
				else
				{
					wtflogComponent = new WTFLogComponent(tracer.Category, tracer.TraceTag, string.Empty, false);
				}
				WTFDiagnostics.tracerToLogComponentMap.TryAdd(tracer, wtflogComponent);
			}
			return wtflogComponent;
		}

		public static void TraceInformation(Trace tracer, TracingContext context, string message, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Information, component, context, message, methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceInformation(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceInformation<T>(Trace tracer, TracingContext context, string formatString, T arg0, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Information, component, context, string.Format(formatString, arg0), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceInformation(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceInformation<T0, T1>(Trace tracer, TracingContext context, string formatString, T0 arg0, T1 arg1, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Information, component, context, string.Format(formatString, arg0, arg1), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceInformation(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceInformation<T0, T1, T2>(Trace tracer, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Information, component, context, string.Format(formatString, arg0, arg1, arg2), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceInformation(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceInformation<T0, T1, T2, T3>(Trace tracer, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, T3 arg3, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Information, component, context, string.Format(formatString, new object[]
			{
				arg0,
				arg1,
				arg2,
				arg3
			}), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceInformation(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceInformation<T0, T1, T2, T3, T4>(Trace tracer, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Information, component, context, string.Format(formatString, new object[]
			{
				arg0,
				arg1,
				arg2,
				arg3,
				arg4
			}), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceInformation(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceInformation(Trace tracer, TracingContext context, string formatString, object[] args, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Information, component, context, string.Format(formatString, args), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceInformation(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceDebug(Trace tracer, TracingContext context, string message, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Debug, component, context, message, methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceDebug(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceDebug<T>(Trace tracer, TracingContext context, string formatString, T arg0, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Debug, component, context, string.Format(formatString, arg0), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceDebug(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceDebug<T0, T1>(Trace tracer, TracingContext context, string formatString, T0 arg0, T1 arg1, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Debug, component, context, string.Format(formatString, arg0, arg1), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceDebug(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceDebug<T0, T1, T2>(Trace tracer, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Debug, component, context, string.Format(formatString, arg0, arg1, arg2), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceDebug(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceDebug<T0, T1, T2, T3>(Trace tracer, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, T3 arg3, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Debug, component, context, string.Format(formatString, new object[]
			{
				arg0,
				arg1,
				arg2,
				arg3
			}), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceDebug(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceDebug<T0, T1, T2, T3, T4>(Trace tracer, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Debug, component, context, string.Format(formatString, new object[]
			{
				arg0,
				arg1,
				arg2,
				arg3,
				arg4
			}), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceDebug(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceDebug(Trace tracer, TracingContext context, string formatString, object[] args, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Debug, component, context, string.Format(formatString, args), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceDebug(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceError(Trace tracer, TracingContext context, string message, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Error, component, context, message, methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceError(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceError<T0>(Trace tracer, TracingContext context, string formatString, T0 arg0, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Error, component, context, string.Format(formatString, arg0), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceError(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceError<T0, T1>(Trace tracer, TracingContext context, string formatString, T0 arg0, T1 arg1, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Error, component, context, string.Format(formatString, arg0, arg1), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceError(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceError<T0, T1, T2>(Trace tracer, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Error, component, context, string.Format(formatString, arg0, arg1, arg2), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceError(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceError<T0, T1, T2, T3>(Trace tracer, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, T3 arg3, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Error, component, context, string.Format(formatString, new object[]
			{
				arg0,
				arg1,
				arg2,
				arg3
			}), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceError(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceError<T0, T1, T2, T3, T4>(Trace tracer, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Error, component, context, string.Format(formatString, new object[]
			{
				arg0,
				arg1,
				arg2,
				arg3,
				arg4
			}), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceError(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceError(Trace tracer, TracingContext context, string formatString, object[] args, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Error, component, context, string.Format(formatString, args), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceError(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceWarning(Trace tracer, TracingContext context, string message, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Warning, component, context, message, methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceWarning(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceWarning<T0>(Trace tracer, TracingContext context, string formatString, T0 arg0, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Warning, component, context, string.Format(formatString, arg0), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceWarning(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceWarning<T0, T1>(Trace tracer, TracingContext context, string formatString, T0 arg0, T1 arg1, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Warning, component, context, string.Format(formatString, arg0, arg1), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceWarning(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceWarning<T0, T1, T2>(Trace tracer, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Warning, component, context, string.Format(formatString, arg0, arg1, arg2), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceWarning(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceWarning<T0, T1, T2, T3>(Trace tracer, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, T3 arg3, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Warning, component, context, string.Format(formatString, new object[]
			{
				arg0,
				arg1,
				arg2,
				arg3
			}), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceWarning(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceWarning<T0, T1, T2, T3, T4>(Trace tracer, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Warning, component, context, string.Format(formatString, new object[]
			{
				arg0,
				arg1,
				arg2,
				arg3,
				arg4
			}), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceWarning(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceWarning(Trace tracer, TracingContext context, string formatString, object[] args, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Warning, component, context, string.Format(formatString, args), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceWarning(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceFunction(Trace tracer, TracingContext context, string message, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Information, component, context, message, methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceFunction(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceFunction<T0>(Trace tracer, TracingContext context, string formatString, T0 arg0, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Information, component, context, string.Format(formatString, arg0), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceFunction(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceFunction<T0, T1>(Trace tracer, TracingContext context, string formatString, T0 arg0, T1 arg1, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Information, component, context, string.Format(formatString, arg0, arg1), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceFunction(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceFunction<T0, T1, T2>(Trace tracer, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Information, component, context, string.Format(formatString, arg0, arg1, arg2), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceFunction(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceFunction<T0, T1, T2, T3>(Trace tracer, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, T3 arg3, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Information, component, context, string.Format(formatString, new object[]
			{
				arg0,
				arg1,
				arg2,
				arg3
			}), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceFunction(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceFunction<T0, T1, T2, T3, T4>(Trace tracer, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Information, component, context, string.Format(formatString, new object[]
			{
				arg0,
				arg1,
				arg2,
				arg3,
				arg4
			}), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceFunction(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceFunction(Trace tracer, TracingContext context, string formatString, object[] args, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Information, component, context, string.Format(formatString, args), methodName, sourceFilePath, sourceLineNumber);
			tracer.TraceFunction(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TracePfd(Trace tracer, TracingContext context, string message, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Information, component, context, message, methodName, sourceFilePath, sourceLineNumber);
			tracer.TracePfd(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TracePfd<T0>(Trace tracer, TracingContext context, string formatString, T0 arg0, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Information, component, context, string.Format(formatString, arg0), methodName, sourceFilePath, sourceLineNumber);
			tracer.TracePfd(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TracePfd<T0, T1>(Trace tracer, TracingContext context, string formatString, T0 arg0, T1 arg1, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Information, component, context, string.Format(formatString, arg0, arg1), methodName, sourceFilePath, sourceLineNumber);
			tracer.TracePfd(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TracePfd<T0, T1, T2>(Trace tracer, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Information, component, context, string.Format(formatString, arg0, arg1, arg2), methodName, sourceFilePath, sourceLineNumber);
			tracer.TracePfd(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TracePfd(Trace tracer, TracingContext context, string formatString, object[] args, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Information, component, context, string.Format(formatString, args), methodName, sourceFilePath, sourceLineNumber);
			tracer.TracePfd(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TracePerformance(Trace tracer, TracingContext context, string message, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogComponent component = WTFDiagnostics.MapTracerToLogComponent(tracer);
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(WTFLogger.LogLevel.Information, component, context, message, methodName, sourceFilePath, sourceLineNumber);
			tracer.TracePerformance(context.LId, (long)context.Id, workItemContext.ToString());
			WTFLogger.Instance.LogWithContext(component, workItemContext);
		}

		public static void TraceInformation(WTFLogComponent logComponent, TracingContext context, string message, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogInformation(logComponent, context, message, methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceInformation<T>(WTFLogComponent logComponent, TracingContext context, string formatString, T arg0, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogInformation(logComponent, context, string.Format(formatString, arg0), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceInformation<T0, T1>(WTFLogComponent logComponent, TracingContext context, string formatString, T0 arg0, T1 arg1, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogInformation(logComponent, context, string.Format(formatString, arg0, arg1), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceInformation<T0, T1, T2>(WTFLogComponent logComponent, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogInformation(logComponent, context, string.Format(formatString, arg0, arg1, arg2), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceInformation<T0, T1, T2, T3>(WTFLogComponent logComponent, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, T3 arg3, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogInformation(logComponent, context, string.Format(formatString, new object[]
			{
				arg0,
				arg1,
				arg2,
				arg3
			}), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceInformation<T0, T1, T2, T3, T4>(WTFLogComponent logComponent, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogInformation(logComponent, context, string.Format(formatString, new object[]
			{
				arg0,
				arg1,
				arg2,
				arg3,
				arg4
			}), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceInformation(WTFLogComponent logComponent, TracingContext context, string formatString, object[] args, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogInformation(logComponent, context, string.Format(formatString, args), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceDebug(WTFLogComponent logComponent, TracingContext context, string message, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogDebug(logComponent, context, message, methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceDebug<T>(WTFLogComponent logComponent, TracingContext context, string formatString, T arg0, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogDebug(logComponent, context, string.Format(formatString, arg0), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceDebug<T0, T1>(WTFLogComponent logComponent, TracingContext context, string formatString, T0 arg0, T1 arg1, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogDebug(logComponent, context, string.Format(formatString, arg0, arg1), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceDebug<T0, T1, T2>(WTFLogComponent logComponent, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogDebug(logComponent, context, string.Format(formatString, arg0, arg1, arg2), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceDebug<T0, T1, T2, T3>(WTFLogComponent logComponent, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, T3 arg3, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogDebug(logComponent, context, string.Format(formatString, new object[]
			{
				arg0,
				arg1,
				arg2,
				arg3
			}), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceDebug<T0, T1, T2, T3, T4>(WTFLogComponent logComponent, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogDebug(logComponent, context, string.Format(formatString, new object[]
			{
				arg0,
				arg1,
				arg2,
				arg3,
				arg4
			}), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceDebug(WTFLogComponent logComponent, TracingContext context, string formatString, object[] args, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogDebug(logComponent, context, string.Format(formatString, args), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceError(WTFLogComponent logComponent, TracingContext context, string message, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogError(logComponent, context, message, methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceError<T0>(WTFLogComponent logComponent, TracingContext context, string formatString, T0 arg0, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogError(logComponent, context, string.Format(formatString, arg0), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceError<T0, T1>(WTFLogComponent logComponent, TracingContext context, string formatString, T0 arg0, T1 arg1, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogError(logComponent, context, string.Format(formatString, arg0, arg1), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceError<T0, T1, T2>(WTFLogComponent logComponent, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogError(logComponent, context, string.Format(formatString, arg0, arg1, arg2), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceError<T0, T1, T2, T3>(WTFLogComponent logComponent, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, T3 arg3, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogError(logComponent, context, string.Format(formatString, new object[]
			{
				arg0,
				arg1,
				arg2,
				arg3
			}), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceError<T0, T1, T2, T3, T4>(WTFLogComponent logComponent, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogError(logComponent, context, string.Format(formatString, new object[]
			{
				arg0,
				arg1,
				arg2,
				arg3,
				arg4
			}), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceError(WTFLogComponent logComponent, TracingContext context, string formatString, object[] args, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogError(logComponent, context, string.Format(formatString, args), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceWarning(WTFLogComponent logComponent, TracingContext context, string message, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogWarning(logComponent, context, message, methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceWarning<T0>(WTFLogComponent logComponent, TracingContext context, string formatString, T0 arg0, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogWarning(logComponent, context, string.Format(formatString, arg0), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceWarning<T0, T1>(WTFLogComponent logComponent, TracingContext context, string formatString, T0 arg0, T1 arg1, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogWarning(logComponent, context, string.Format(formatString, arg0, arg1), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceWarning<T0, T1, T2>(WTFLogComponent logComponent, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogWarning(logComponent, context, string.Format(formatString, arg0, arg1, arg2), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceWarning<T0, T1, T2, T3>(WTFLogComponent logComponent, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, T3 arg3, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogWarning(logComponent, context, string.Format(formatString, new object[]
			{
				arg0,
				arg1,
				arg2,
				arg3
			}), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceWarning<T0, T1, T2, T3, T4>(WTFLogComponent logComponent, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogWarning(logComponent, context, string.Format(formatString, new object[]
			{
				arg0,
				arg1,
				arg2,
				arg3,
				arg4
			}), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceWarning(WTFLogComponent logComponent, TracingContext context, string formatString, object[] args, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogWarning(logComponent, context, string.Format(formatString, args), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceFunction(WTFLogComponent logComponent, TracingContext context, string message, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogInformation(logComponent, context, message, methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceFunction<T0>(WTFLogComponent logComponent, TracingContext context, string formatString, T0 arg0, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogInformation(logComponent, context, string.Format(formatString, arg0), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceFunction<T0, T1>(WTFLogComponent logComponent, TracingContext context, string formatString, T0 arg0, T1 arg1, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogInformation(logComponent, context, string.Format(formatString, arg0, arg1), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceFunction<T0, T1, T2>(WTFLogComponent logComponent, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogInformation(logComponent, context, string.Format(formatString, arg0, arg1, arg2), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceFunction<T0, T1, T2, T3>(WTFLogComponent logComponent, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, T3 arg3, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogInformation(logComponent, context, string.Format(formatString, new object[]
			{
				arg0,
				arg1,
				arg2,
				arg3
			}), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceFunction<T0, T1, T2, T3, T4>(WTFLogComponent logComponent, TracingContext context, string formatString, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogInformation(logComponent, context, string.Format(formatString, new object[]
			{
				arg0,
				arg1,
				arg2,
				arg3,
				arg4
			}), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void TraceFunction(WTFLogComponent logComponent, TracingContext context, string formatString, object[] args, WTFLogger notUsed = null, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			if (context == null || context.IsDisabled)
			{
				return;
			}
			WTFLogger.Instance.LogInformation(logComponent, context, string.Format(formatString, args), methodName, sourceFilePath, sourceLineNumber);
		}

		public static void FaultInjectionTraceTest(FaultInjectionLid lid)
		{
			Microsoft.Exchange.Diagnostics.Components.WorkerTaskFramework.ExTraceGlobals.FaultInjectionTracer.TraceTest((uint)lid);
		}

		public static void FaultInjectionTraceTest<T>(FaultInjectionLid lid, ref T objectToChange)
		{
			Microsoft.Exchange.Diagnostics.Components.WorkerTaskFramework.ExTraceGlobals.FaultInjectionTracer.TraceTest<T>((uint)lid, ref objectToChange);
		}

		public static void FaultInjectionTraceTest<T>(FaultInjectionLid lid, T objectToCompare)
		{
			Microsoft.Exchange.Diagnostics.Components.WorkerTaskFramework.ExTraceGlobals.FaultInjectionTracer.TraceTest<T>((uint)lid, objectToCompare);
		}

		private static Exception Callback(string exceptionType)
		{
			Exception result = null;
			if (exceptionType != null && exceptionType.Equals("System.InvalidOperationExceptionInMemory", StringComparison.OrdinalIgnoreCase))
			{
				result = TroubleshootingContext.FaultInjectionInvalidOperationException;
			}
			return result;
		}

		private static bool IsLogComponentEnabled(Trace tracer)
		{
			return Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring.ExTraceGlobals.RPSTracer != tracer && !(Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring.ExTraceGlobals.GenericHelperTracer.Category == tracer.Category);
		}

		private const string ActiveMonitoringComponent = "Active Monitoring";

		private static ExEventLog eventLogger = new ExEventLog(WTFLog.Core.Category, "Active Monitoring");

		private static TroubleshootingContext troubleshootingContext;

		private static ConcurrentDictionary<Trace, WTFLogComponent> tracerToLogComponentMap = new ConcurrentDictionary<Trace, WTFLogComponent>();
	}
}
