using System;
using Microsoft.Exchange.Diagnostics.Components.DxStore;
using Microsoft.Exchange.DxStore.Common;
using Microsoft.Exchange.DxStore.HA.Events;

namespace Microsoft.Exchange.DxStore.HA
{
	public class DistributedStoreEventLogger : IDxStoreEventLogger
	{
		public DistributedStoreEventLogger(bool isLogToConsole = false)
		{
			this.isLogToConsole = isLogToConsole;
		}

		public DxStoreHACrimsonEvent GetEventBySeverity(DxEventSeverity severity)
		{
			switch (severity)
			{
			case DxEventSeverity.Error:
				return DxStoreHACrimsonEvents.ServerOperationError;
			case DxEventSeverity.Warning:
				return DxStoreHACrimsonEvents.ServerOperationWarning;
			default:
				return DxStoreHACrimsonEvents.ServerOperationInfo;
			}
		}

		public void Log(DxEventSeverity severity, int id, string formatString, params object[] args)
		{
			DxStoreHACrimsonEvent eventBySeverity = this.GetEventBySeverity(severity);
			string text = string.Format(formatString, args);
			eventBySeverity.LogGeneric(new object[]
			{
				id,
				text
			});
			string text2 = string.Format("[{0}] {1}: {2}", severity, id, text);
			if (this.isLogToConsole)
			{
				Console.WriteLine(text2);
			}
			switch (severity)
			{
			case DxEventSeverity.Error:
				ExTraceGlobals.EventLoggerTracer.TraceError((long)id, text2);
				return;
			case DxEventSeverity.Warning:
				ExTraceGlobals.EventLoggerTracer.TraceWarning((long)id, text2);
				return;
			default:
				ExTraceGlobals.EventLoggerTracer.TraceDebug((long)id, text2);
				return;
			}
		}

		public void LogPeriodic(string periodicKey, TimeSpan periodicDuration, DxEventSeverity severity, int id, string formatString, params object[] args)
		{
			DxStoreHACrimsonEvent eventBySeverity = this.GetEventBySeverity(severity);
			string text = string.Format(formatString, args);
			eventBySeverity.LogPeriodicGeneric(periodicKey, periodicDuration, new object[]
			{
				id,
				text
			});
			if (this.isLogToConsole)
			{
				Console.WriteLine("[{0}] {1}: (P) {2}", severity, id, text);
			}
		}

		private readonly bool isLogToConsole;
	}
}
