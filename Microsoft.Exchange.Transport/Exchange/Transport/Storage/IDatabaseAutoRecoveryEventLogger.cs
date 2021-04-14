using System;

namespace Microsoft.Exchange.Transport.Storage
{
	internal interface IDatabaseAutoRecoveryEventLogger
	{
		void LogDatabaseRecoveryActionNone(string databaseInstanceName, string databasePath, string logFilePath);

		void LogDatabaseRecoveryActionMove(string databaseInstanceName, string databasePath, string databaseMovePath);

		void LogDatabaseRecoveryActionMove(string databaseInstanceName, string databasePath, string databaseMovePath, string logFilePath, string moveLogFilePath);

		void LogDatabaseRecoveryActionDelete(string databaseInstanceName, string databasePath);

		void LogDatabaseRecoveryActionDelete(string databaseInstanceName, string databasePath, string logFilePath);

		void LogDatabaseRecoveryActionFailed(string databaseInstanceName, DatabaseRecoveryAction databaseRecoveryAction, string failureReason);

		void DatabaseRecoveryActionFailedRegistryAccessDenied(string databaseInstanceName, string registryKeyPath, string errorMessage);

		void DataBaseCorruptionDetected(string databaseInstanceName, string registryKeyPath);
	}
}
