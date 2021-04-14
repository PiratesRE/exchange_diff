using System;

namespace Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery
{
	internal interface ILogAdapter
	{
		void Trace(string messageTemplate, params object[] args);

		void LogError(string messageTemplate, params object[] args);

		void LogException(Exception exception, string additionalMessage, params object[] args);

		void ExecuteMonitoredOperation(Enum logMetadata, Action operation);

		void LogOperationResult(Enum logMetadata, string domain, bool succeeded);

		void LogOperationException(Enum logMetadata, Exception ex);

		void RegisterLogMetaData(string actionName, Type logMetaDataEnumType);
	}
}
