using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class NullAnchorLogger : DisposeTrackableBase, ILogger, IDisposeTrackable, IDisposable
	{
		static NullAnchorLogger()
		{
			NullAnchorLogger.Instance.SuppressDisposeTracker();
		}

		private NullAnchorLogger()
		{
		}

		void ILogger.Log(MigrationEventType eventType, Exception exception, string format, params object[] args)
		{
		}

		void ILogger.Log(MigrationEventType eventType, string format, params object[] args)
		{
		}

		void ILogger.Log(string source, MigrationEventType eventType, object context, string format, params object[] args)
		{
		}

		void ILogger.LogError(Exception exception, string formatString, params object[] formatArgs)
		{
		}

		void ILogger.LogEvent(MigrationEventType eventType, params string[] args)
		{
		}

		void ILogger.LogEvent(MigrationEventType eventType, ExEventLog.EventTuple eventId, params string[] args)
		{
		}

		void ILogger.LogEvent(MigrationEventType eventType, Exception ex, params string[] args)
		{
		}

		void ILogger.LogEvent(MigrationEventType eventType, ExEventLog.EventTuple eventId, Exception ex, params string[] args)
		{
		}

		void ILogger.LogInformation(string formatString, params object[] formatArgs)
		{
		}

		void ILogger.LogTerseEvent(MigrationEventType eventType, params string[] args)
		{
		}

		void ILogger.LogTerseEvent(MigrationEventType eventType, ExEventLog.EventTuple eventId, params string[] args)
		{
		}

		void ILogger.LogTerseEvent(MigrationEventType eventType, Exception ex, params string[] args)
		{
		}

		void ILogger.LogTerseEvent(MigrationEventType eventType, ExEventLog.EventTuple eventId, Exception ex, params string[] args)
		{
		}

		void ILogger.LogVerbose(string formatString, params object[] formatArgs)
		{
		}

		void ILogger.LogWarning(string formatString, params object[] formatArgs)
		{
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<NullAnchorLogger>(this);
		}

		public static readonly ILogger Instance = new NullAnchorLogger();
	}
}
