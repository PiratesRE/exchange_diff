using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CmdletLogAdapter : DisposeTrackableBase, ILogger, IDisposeTrackable, IDisposable
	{
		public CmdletLogAdapter(ILogger secondaryLog, Action<LocalizedString> verboseLogDelegate, Action<LocalizedString> warningLogDelegate, Action<LocalizedString> debugLogDelegate)
		{
			this.verboseLogDelegate = (verboseLogDelegate ?? new Action<LocalizedString>(this.VoidLog));
			this.warningLogDelegate = (warningLogDelegate ?? new Action<LocalizedString>(this.VoidLog));
			this.debugLogDelegate = (debugLogDelegate ?? new Action<LocalizedString>(this.VoidLog));
			this.secondaryLog = (secondaryLog ?? NullAnchorLogger.Instance);
		}

		public void Log(MigrationEventType eventType, Exception exception, string format, params object[] args)
		{
			Action<LocalizedString> logDelegateFromEventType = this.GetLogDelegateFromEventType(eventType);
			if (exception != null)
			{
				this.LogOnDelegate(logDelegateFromEventType, eventType, CmdletLogAdapter.GetMessageWithException(exception, format, args), new object[0]);
			}
			else
			{
				this.LogOnDelegate(logDelegateFromEventType, eventType, format, args);
			}
			this.secondaryLog.Log(eventType, exception, format, args);
		}

		public void Log(MigrationEventType eventType, string format, params object[] args)
		{
			Action<LocalizedString> logDelegateFromEventType = this.GetLogDelegateFromEventType(eventType);
			this.LogOnDelegate(logDelegateFromEventType, eventType, format, args);
			this.secondaryLog.Log(eventType, format, args);
		}

		public void Log(string source, MigrationEventType eventType, object context, string format, params object[] args)
		{
			Action<LocalizedString> logDelegateFromEventType = this.GetLogDelegateFromEventType(eventType);
			this.LogOnDelegate(logDelegateFromEventType, eventType, format, args);
			this.secondaryLog.Log(source, eventType, context, format, args);
		}

		public void LogError(Exception exception, string formatString, params object[] formatArgs)
		{
			this.LogOnDelegate(this.warningLogDelegate, MigrationEventType.Error, CmdletLogAdapter.GetMessageWithException(exception, formatString, formatArgs), new object[0]);
			this.secondaryLog.LogError(exception, formatString, formatArgs);
		}

		public void LogEvent(MigrationEventType eventType, params string[] args)
		{
			this.secondaryLog.LogEvent(eventType, args);
		}

		public void LogEvent(MigrationEventType eventType, ExEventLog.EventTuple eventId, params string[] args)
		{
			this.secondaryLog.LogEvent(eventType, eventId, args);
		}

		public void LogEvent(MigrationEventType eventType, Exception ex, params string[] args)
		{
			this.secondaryLog.LogEvent(eventType, ex, args);
		}

		public void LogEvent(MigrationEventType eventType, ExEventLog.EventTuple eventId, Exception ex, params string[] args)
		{
			this.secondaryLog.LogEvent(eventType, eventId, ex, args);
		}

		public void LogInformation(string formatString, params object[] formatArgs)
		{
			this.LogOnDelegate(this.verboseLogDelegate, MigrationEventType.Information, formatString, formatArgs);
			this.secondaryLog.LogInformation(formatString, formatArgs);
		}

		public void LogTerseEvent(MigrationEventType eventType, params string[] args)
		{
			this.secondaryLog.LogTerseEvent(eventType, args);
		}

		public void LogTerseEvent(MigrationEventType eventType, ExEventLog.EventTuple eventId, params string[] args)
		{
			this.secondaryLog.LogTerseEvent(eventType, eventId, args);
		}

		public void LogTerseEvent(MigrationEventType eventType, Exception ex, params string[] args)
		{
			this.secondaryLog.LogTerseEvent(eventType, ex, args);
		}

		public void LogTerseEvent(MigrationEventType eventType, ExEventLog.EventTuple eventId, Exception ex, params string[] args)
		{
			this.secondaryLog.LogTerseEvent(eventType, eventId, ex, args);
		}

		public void LogVerbose(string formatString, params object[] formatArgs)
		{
			this.LogOnDelegate(this.verboseLogDelegate, MigrationEventType.Verbose, formatString, formatArgs);
			this.secondaryLog.LogVerbose(formatString, formatArgs);
		}

		public void LogWarning(string formatString, params object[] formatArgs)
		{
			this.secondaryLog.LogWarning(formatString, formatArgs);
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CmdletLogAdapter>(this);
		}

		private static string GetMessageWithException(Exception exception, string format, object[] args)
		{
			return string.Format("{0}. Exception: {1}", string.Format(format, args), exception);
		}

		private Action<LocalizedString> GetLogDelegateFromEventType(MigrationEventType eventType)
		{
			switch (eventType)
			{
			case MigrationEventType.Error:
			case MigrationEventType.Warning:
				return this.warningLogDelegate;
			case MigrationEventType.Information:
			case MigrationEventType.Verbose:
				return this.verboseLogDelegate;
			default:
				return this.debugLogDelegate;
			}
		}

		private void LogOnDelegate(Action<LocalizedString> logDelegate, MigrationEventType logType, string message, params object[] formatArgs)
		{
			string arg;
			if (formatArgs.Length == 0)
			{
				arg = message;
			}
			else
			{
				arg = string.Format(message, formatArgs);
			}
			logDelegate(new LocalizedString(string.Format("[{0}] {1}", logType, arg)));
		}

		private void VoidLog(LocalizedString obj)
		{
		}

		private readonly Action<LocalizedString> debugLogDelegate;

		private readonly ILogger secondaryLog;

		private readonly Action<LocalizedString> verboseLogDelegate;

		private readonly Action<LocalizedString> warningLogDelegate;
	}
}
