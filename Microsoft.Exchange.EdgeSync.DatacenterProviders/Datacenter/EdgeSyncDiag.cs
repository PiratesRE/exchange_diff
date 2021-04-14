using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EdgeSync.Common;
using Microsoft.Exchange.EdgeSync.Logging;

namespace Microsoft.Exchange.EdgeSync.Datacenter
{
	internal class EdgeSyncDiag
	{
		public EdgeSyncDiag(EdgeSyncLogSession logSession, Trace tracer)
		{
			this.logSession = logSession;
			this.tracer = tracer;
		}

		public ExEventLog EventLog
		{
			get
			{
				return EdgeSyncEvents.Log;
			}
		}

		public EdgeSyncLogSession LogSession
		{
			get
			{
				return this.logSession;
			}
		}

		public Trace Tracer
		{
			get
			{
				return this.tracer;
			}
		}

		public static string GetDiagString(string messageFormat, params object[] args)
		{
			return string.Format(CultureInfo.InvariantCulture, messageFormat, args);
		}

		public string LogAndTraceError(string messageFormat, params object[] args)
		{
			string diagString = EdgeSyncDiag.GetDiagString(messageFormat, args);
			this.LogSession.LogEvent(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, null, diagString);
			this.Tracer.TraceError<string>((long)this.GetHashCode(), "{0}", diagString);
			return diagString;
		}

		public void LogAndTraceException(Exception exception, string messageFormat, params object[] args)
		{
			string diagString = EdgeSyncDiag.GetDiagString(messageFormat, args);
			this.LogSession.LogException(EdgeSyncLoggingLevel.Low, EdgeSyncEvent.TargetConnection, exception, diagString);
			this.Tracer.TraceError<string, Exception>((long)this.GetHashCode(), "{0}; Exception: {1}", diagString, exception);
		}

		public void LogAndTraceInfo(EdgeSyncLoggingLevel level, string messageFormat, params object[] args)
		{
			string diagString = EdgeSyncDiag.GetDiagString(messageFormat, args);
			this.LogSession.LogEvent(level, EdgeSyncEvent.TargetConnection, null, diagString);
			this.Tracer.TraceDebug<string>((long)this.GetHashCode(), "{0}", diagString);
		}

		private EdgeSyncLogSession logSession;

		private Trace tracer;
	}
}
