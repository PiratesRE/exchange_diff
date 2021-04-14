using System;
using System.Diagnostics;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Servicelets.AuthAdmin.Messages;

namespace Microsoft.Exchange.Servicelets.AuthAdmin
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AuthAdminLogger : AnchorLogger
	{
		public AuthAdminLogger(string applicationName, AnchorConfig config, ExEventLog eventLog) : base(applicationName, config, eventLog)
		{
		}

		public void LogEventLog(ExEventLog.EventTuple eventId, params object[] args)
		{
			this.LogEventLog(eventId, null, args);
		}

		public void LogEventLog(ExEventLog.EventTuple eventId, Exception exception, params object[] args)
		{
			MigrationEventType eventType = MigrationEventType.Information;
			EventLogEntryType entryType = eventId.EntryType;
			switch (entryType)
			{
			case EventLogEntryType.Warning:
				eventType = MigrationEventType.Warning;
				goto IL_2A;
			case (EventLogEntryType)3:
				break;
			case EventLogEntryType.Information:
				goto IL_2A;
			default:
				if (entryType == EventLogEntryType.SuccessAudit)
				{
					goto IL_2A;
				}
				break;
			}
			eventType = MigrationEventType.Error;
			IL_2A:
			string formatString = "Event " + (eventId.EventId & 65535U).ToString();
			if (exception == null)
			{
				base.EventLogger.LogEvent(eventId, string.Empty, args);
				base.Log(eventType, formatString, args);
				return;
			}
			base.EventLogger.LogEvent(eventId, string.Empty, new object[]
			{
				exception,
				args
			});
			base.Log(eventType, exception, formatString, args);
		}

		protected override ExEventLog.EventTuple? EventIdFromLogLevel(MigrationEventType eventType)
		{
			switch (eventType)
			{
			case MigrationEventType.Error:
				return new ExEventLog.EventTuple?(MSExchangeAuthAdminEventLogConstants.Tuple_CriticalError);
			case MigrationEventType.Warning:
				return new ExEventLog.EventTuple?(MSExchangeAuthAdminEventLogConstants.Tuple_Warning);
			case MigrationEventType.Information:
				return new ExEventLog.EventTuple?(MSExchangeAuthAdminEventLogConstants.Tuple_Information);
			default:
				return null;
			}
		}
	}
}
