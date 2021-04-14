using System;
using System.Collections.Generic;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalance.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DiagnosticLogger : DisposeTrackableBase, ILogger, IDisposeTrackable, IDisposable
	{
		public DiagnosticLogger(ILogger logger)
		{
			this.logger = logger;
			this.Logs = new Report.ListWithToString<DiagnosticLog>();
		}

		public IList<DiagnosticLog> Logs { get; private set; }

		public void Log(string source, MigrationEventType eventType, object context, string format, params object[] args)
		{
			this.AddLogEntry(eventType, null, format, args);
			this.logger.Log(source, eventType, context, format, args);
		}

		public void LogEvent(MigrationEventType eventType, ExEventLog.EventTuple eventId, params string[] args)
		{
			this.logger.LogEvent(eventType, eventId, args);
		}

		public void LogEvent(MigrationEventType eventType, Exception ex, params string[] args)
		{
			this.logger.LogEvent(eventType, ex, args);
		}

		public void LogEvent(MigrationEventType eventType, params string[] args)
		{
			this.logger.LogEvent(eventType, args);
		}

		public void LogEvent(MigrationEventType eventType, ExEventLog.EventTuple eventId, Exception ex, params string[] args)
		{
			this.logger.LogEvent(eventType, eventId, ex, args);
		}

		public void LogTerseEvent(MigrationEventType eventType, params string[] args)
		{
			this.logger.LogTerseEvent(eventType, args);
		}

		public void LogTerseEvent(MigrationEventType eventType, ExEventLog.EventTuple eventId, params string[] args)
		{
			this.logger.LogTerseEvent(eventType, eventId, args);
		}

		public void LogTerseEvent(MigrationEventType eventType, Exception ex, params string[] args)
		{
			this.logger.LogTerseEvent(eventType, ex, args);
		}

		public void LogTerseEvent(MigrationEventType eventType, ExEventLog.EventTuple eventId, Exception ex, params string[] args)
		{
			this.logger.LogTerseEvent(eventType, eventId, ex, args);
		}

		public void Log(MigrationEventType eventType, Exception exception, string format, params object[] args)
		{
			this.AddLogEntry(eventType, exception, format, args);
			this.logger.Log(eventType, exception, format, args);
		}

		public void Log(MigrationEventType logLevel, string entryString, params object[] formatArgs)
		{
			this.AddLogEntry(logLevel, null, entryString, formatArgs);
			this.logger.Log(logLevel, entryString, formatArgs);
		}

		public void LogError(Exception exception, string entryString, params object[] formatArgs)
		{
			this.AddLogEntry(MigrationEventType.Error, exception, entryString, formatArgs);
			this.logger.LogError(exception, entryString, formatArgs);
		}

		public void LogVerbose(string formatString, params object[] formatArgs)
		{
			this.Log(MigrationEventType.Verbose, formatString, formatArgs);
		}

		public void LogWarning(string formatString, params object[] formatArgs)
		{
			this.Log(MigrationEventType.Warning, formatString, formatArgs);
		}

		public void LogInformation(string formatString, params object[] formatArgs)
		{
			this.Log(MigrationEventType.Information, formatString, formatArgs);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DiagnosticLogger>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		private void AddLogEntry(MigrationEventType eventType, Exception exception, string format, object[] args)
		{
			string logEntry;
			if (args == null || args.Length == 0)
			{
				logEntry = format;
			}
			else
			{
				logEntry = string.Format(format, args);
			}
			this.Logs.Add(new DiagnosticLog
			{
				Exception = exception,
				Level = eventType,
				LogEntry = logEntry
			});
		}

		private readonly ILogger logger;
	}
}
