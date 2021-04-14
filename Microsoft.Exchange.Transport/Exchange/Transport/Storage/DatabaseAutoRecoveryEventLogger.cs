using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport.Storage
{
	internal sealed class DatabaseAutoRecoveryEventLogger : IDatabaseAutoRecoveryEventLogger
	{
		public void LogDatabaseRecoveryActionNone(string databaseInstanceName, string databasePath, string logFilePath)
		{
			DatabaseAutoRecoveryEventLogger.eventLogger.LogEvent(TransportEventLogConstants.Tuple_DatabaseRecoveryActionNone, null, new object[]
			{
				databaseInstanceName,
				databasePath,
				logFilePath
			});
			string notificationReason = string.Format("{0}: MSExchangeTransport has detected a critical storage error but will not take any recovery actions. Issues encountered with this database ({1}) and associated transaction logs ({2}).", databaseInstanceName, databasePath, logFilePath);
			EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "CF9BC4E8-193F-4856-97EE-0127B67410ED", null, notificationReason, ResultSeverityLevel.Error, false);
		}

		public void LogDatabaseRecoveryActionMove(string databaseInstanceName, string databasePath, string databaseMovePath)
		{
			DatabaseAutoRecoveryEventLogger.eventLogger.LogEvent(TransportEventLogConstants.Tuple_DatabaseRecoveryActionMove, null, new object[]
			{
				databaseInstanceName,
				Strings.DatabaseMoved(databasePath, databaseMovePath)
			});
			string notificationReason = string.Format("{0}: MSExchangeTransport has detected a critical storage error and has taken an automated recovery action.  This recovery action will not be repeated until the target folders are renamed or deleted. {1}", databaseInstanceName, Strings.DatabaseMoved(databasePath, databaseMovePath));
			EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "DatabaseMoved", null, notificationReason, ResultSeverityLevel.Error, false);
		}

		public void LogDatabaseRecoveryActionMove(string databaseInstanceName, string databasePath, string databaseMovePath, string logFilePath, string moveLogFilePath)
		{
			DatabaseAutoRecoveryEventLogger.eventLogger.LogEvent(TransportEventLogConstants.Tuple_DatabaseRecoveryActionMove, null, new object[]
			{
				databaseInstanceName,
				Strings.DatabaseAndDatabaseLogMoved(databasePath, databaseMovePath, logFilePath, moveLogFilePath)
			});
			string notificationReason = string.Format("{0}: MSExchangeTransport has detected a critical storage error and has taken an automated recovery action.  This recovery action will not be repeated until the target folders are renamed or deleted. {1}", databaseInstanceName, Strings.DatabaseAndDatabaseLogMoved(databasePath, databaseMovePath, logFilePath, moveLogFilePath));
			EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "DatabaseMoved", null, notificationReason, ResultSeverityLevel.Error, false);
		}

		public void LogDatabaseRecoveryActionDelete(string databaseInstanceName, string databasePath)
		{
			DatabaseAutoRecoveryEventLogger.eventLogger.LogEvent(TransportEventLogConstants.Tuple_DatabaseRecoveryActionDelete, null, new object[]
			{
				databaseInstanceName,
				Strings.DatabaseDeleted(databasePath)
			});
		}

		public void LogDatabaseRecoveryActionDelete(string databaseInstanceName, string databasePath, string logFilePath)
		{
			DatabaseAutoRecoveryEventLogger.eventLogger.LogEvent(TransportEventLogConstants.Tuple_DatabaseRecoveryActionDelete, null, new object[]
			{
				databaseInstanceName,
				Strings.DatabaseAndDatabaseLogDeleted(databasePath, logFilePath)
			});
		}

		public void LogDatabaseRecoveryActionFailed(string databaseInstanceName, DatabaseRecoveryAction databaseRecoveryAction, string failureReason)
		{
			string text;
			if (databaseRecoveryAction == DatabaseRecoveryAction.Move)
			{
				text = Strings.DatabaseRecoveryActionMove;
			}
			else if (databaseRecoveryAction == DatabaseRecoveryAction.Delete)
			{
				text = Strings.DatabaseRecoveryActionDelete;
			}
			else
			{
				text = Strings.DatabaseRecoveryActionNone;
			}
			DatabaseAutoRecoveryEventLogger.eventLogger.LogEvent(TransportEventLogConstants.Tuple_DatabaseRecoveryActionFailed, null, new object[]
			{
				databaseInstanceName,
				text,
				failureReason
			});
			string notificationReason = string.Format("{0}: MSExchangeTransport has detected a critical storage error but failed to complete the desired recovery action on {1} due to error {2}.", databaseInstanceName, text, failureReason);
			EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "DatabaseRecoveryActionFailed", null, notificationReason, ResultSeverityLevel.Error, false);
		}

		public void DatabaseRecoveryActionFailedRegistryAccessDenied(string databaseInstanceName, string registryKeyPath, string errorMessage)
		{
			DatabaseAutoRecoveryEventLogger.eventLogger.LogEvent(TransportEventLogConstants.Tuple_DatabaseRecoveryActionFailedRegistryAccessDenied, null, new object[]
			{
				databaseInstanceName,
				registryKeyPath,
				errorMessage
			});
			string notificationReason = string.Format("{0}: MSExchangeTransport has detected a critical storage error and will now stop.  The service attempted to update the registry key ({1}) used to initiate automated recovery but failed with the following error: {2}.", databaseInstanceName, registryKeyPath, errorMessage);
			EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "DatabaseRecoveryActionFailedRegistryAccessDenied", null, notificationReason, ResultSeverityLevel.Error, false);
		}

		public void DataBaseCorruptionDetected(string databaseInstanceName, string registryKeyPath)
		{
			DatabaseAutoRecoveryEventLogger.eventLogger.LogEvent(TransportEventLogConstants.Tuple_DataBaseCorruptionDetected, null, new object[]
			{
				databaseInstanceName,
				registryKeyPath
			});
		}

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.ExpoTracer.Category, TransportEventLog.GetEventSource());
	}
}
