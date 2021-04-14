using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ILogger : IDisposeTrackable, IDisposable
	{
		void LogEvent(MigrationEventType eventType, params string[] args);

		void LogEvent(MigrationEventType eventType, ExEventLog.EventTuple eventId, params string[] args);

		void LogEvent(MigrationEventType eventType, Exception ex, params string[] args);

		void LogEvent(MigrationEventType eventType, ExEventLog.EventTuple eventId, Exception ex, params string[] args);

		void LogTerseEvent(MigrationEventType eventType, params string[] args);

		void LogTerseEvent(MigrationEventType eventType, ExEventLog.EventTuple eventId, params string[] args);

		void LogTerseEvent(MigrationEventType eventType, Exception ex, params string[] args);

		void LogTerseEvent(MigrationEventType eventType, ExEventLog.EventTuple eventId, Exception ex, params string[] args);

		void LogError(Exception exception, string formatString, params object[] formatArgs);

		void LogVerbose(string formatString, params object[] formatArgs);

		void LogWarning(string formatString, params object[] formatArgs);

		void LogInformation(string formatString, params object[] formatArgs);

		void Log(MigrationEventType eventType, Exception exception, string format, params object[] args);

		void Log(MigrationEventType eventType, string format, params object[] args);

		void Log(string source, MigrationEventType eventType, object context, string format, params object[] args);
	}
}
