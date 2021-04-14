using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Logging
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GlobalSyncLogSession : SyncLogSession
	{
		public GlobalSyncLogSession(SyncLog syncLog, LogRowFormatter row) : base(syncLog, row)
		{
		}

		public void LogInformation(TSLID logEntryId, Guid subscriptionGuid, object user, string format, params object[] arguments)
		{
			base.LogEvent(logEntryId, SyncLoggingLevel.Information, subscriptionGuid, user, null, null, format, arguments);
		}

		public void LogInformation(TSLID logEntryId, Trace trace, long id, Guid subscriptionGuid, object user, string format, params object[] arguments)
		{
			this.LogInformation(logEntryId, subscriptionGuid, user, format, arguments);
			if (trace.IsTraceEnabled(TraceType.DebugTrace))
			{
				trace.TraceDebug(id, this.MakeGlobalTracerMessage(subscriptionGuid, user, format, arguments));
			}
		}

		public void LogInformation(TSLID logEntryId, Trace trace, Guid subscriptionGuid, object user, string format, params object[] arguments)
		{
			this.LogInformation(logEntryId, trace, 0L, subscriptionGuid, user, format, arguments);
		}

		public void LogVerbose(TSLID logEntryId, Guid subscriptionGuid, object user, string format, params object[] arguments)
		{
			base.LogEvent(logEntryId, SyncLoggingLevel.Verbose, subscriptionGuid, user, null, null, format, arguments);
		}

		public void LogVerbose(TSLID logEntryId, Trace trace, long id, Guid subscriptionGuid, object user, string format, params object[] arguments)
		{
			this.LogVerbose(logEntryId, subscriptionGuid, user, format, arguments);
			if (trace.IsTraceEnabled(TraceType.DebugTrace))
			{
				trace.TraceDebug(id, this.MakeGlobalTracerMessage(subscriptionGuid, user, format, arguments));
			}
		}

		public void LogVerbose(TSLID logEntryId, Trace trace, Guid subscriptionGuid, object user, string format, params object[] arguments)
		{
			this.LogVerbose(logEntryId, trace, 0L, subscriptionGuid, user, format, arguments);
		}

		public void LogError(TSLID logEntryId, Guid subscriptionGuid, object user, string format, params object[] arguments)
		{
			base.LogEvent(logEntryId, SyncLoggingLevel.Error, subscriptionGuid, user, null, null, format, arguments);
		}

		public void LogError(TSLID logEntryId, Trace trace, long id, Guid subscriptionGuid, object user, string format, params object[] arguments)
		{
			this.LogError(logEntryId, subscriptionGuid, user, format, arguments);
			if (trace.IsTraceEnabled(TraceType.DebugTrace))
			{
				trace.TraceDebug(id, this.MakeGlobalTracerMessage(subscriptionGuid, user, format, arguments));
			}
		}

		public void LogError(TSLID logEntryId, Trace trace, Guid subscriptionGuid, object user, string format, params object[] arguments)
		{
			this.LogError(logEntryId, trace, 0L, subscriptionGuid, user, format, arguments);
		}

		public void LogRawData(TSLID logEntryId, Guid subscriptionGuid, object user, string format, params object[] arguments)
		{
			base.LogEvent(logEntryId, SyncLoggingLevel.RawData, subscriptionGuid, user, null, null, format, arguments);
		}

		public void LogRawData(TSLID logEntryId, Trace trace, long id, Guid subscriptionGuid, object user, string format, params object[] arguments)
		{
			this.LogRawData(logEntryId, subscriptionGuid, user, format, arguments);
			if (trace.IsTraceEnabled(TraceType.DebugTrace))
			{
				trace.TraceDebug(id, this.MakeGlobalTracerMessage(subscriptionGuid, user, format, arguments));
			}
		}

		public void LogRawData(TSLID logEntryId, Trace trace, Guid subscriptionGuid, object user, string format, params object[] arguments)
		{
			this.LogRawData(logEntryId, trace, 0L, subscriptionGuid, user, format, arguments);
		}

		public void LogDebugging(TSLID logEntryId, Guid subscriptionGuid, object user, string format, params object[] arguments)
		{
			base.LogEvent(logEntryId, SyncLoggingLevel.Debugging, subscriptionGuid, user, null, null, format, arguments);
		}

		public void LogDebugging(TSLID logEntryId, Trace trace, long id, Guid subscriptionGuid, object user, string format, params object[] arguments)
		{
			this.LogDebugging(logEntryId, subscriptionGuid, user, format, arguments);
			if (trace.IsTraceEnabled(TraceType.DebugTrace))
			{
				trace.TraceDebug(id, this.MakeGlobalTracerMessage(subscriptionGuid, user, format, arguments));
			}
		}

		public void LogDebugging(TSLID logEntryId, Trace trace, Guid subscriptionGuid, object user, string format, params object[] arguments)
		{
			this.LogDebugging(logEntryId, trace, 0L, subscriptionGuid, user, format, arguments);
		}

		private string MakeGlobalTracerMessage(Guid subscriptionGuid, object user, string format, object[] arguments)
		{
			string result;
			try
			{
				string text = string.Format(SyncLogSession.TracerFormatProvider.Instance, format, arguments);
				result = string.Format(SyncLogSession.TracerFormatProvider.Instance, "{0} [Subscription: {1}, User: {2}]", new object[]
				{
					text,
					subscriptionGuid.ToString(),
					user
				});
			}
			catch (FormatException exception)
			{
				base.ReportWatson("Malformed logging format found.", exception);
				result = string.Empty;
			}
			return result;
		}
	}
}
