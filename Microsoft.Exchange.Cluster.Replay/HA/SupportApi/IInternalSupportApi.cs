using System;
using System.ServiceModel;

namespace Microsoft.Exchange.HA.SupportApi
{
	[ServiceContract(Namespace = "http://Microsoft.Exchange.HA.SupportApi")]
	public interface IInternalSupportApi
	{
		[OperationContract]
		void DisconnectCopier(Guid dbGuid);

		[OperationContract]
		void ConnectCopier(Guid dbGuid);

		[OperationContract]
		void SetFailedAndSuspended(Guid dbGuid, bool fSuspendCopy, uint errorEventId, string failedMsg);

		[OperationContract]
		void TriggerShutdownSwitchover();

		[OperationContract]
		void IgnoreGranularCompletions(Guid dbGuid);

		[OperationContract]
		bool Ping();

		[OperationContract]
		void ReloadRegistryParameters();

		[OperationContract]
		void TriggerLogSourceCorruption(Guid dbGuid, bool granular, bool granularRepairFails, int countOfLogsBeforeCorruption);

		[OperationContract]
		void SetCopyProperty(Guid dbGuid, string propName, string propVal);

		[OperationContract]
		void TriggerConfigUpdater();

		[OperationContract]
		void TriggerDumpster(Guid dbGuid, DateTime inspectorTime);

		[OperationContract]
		void TriggerDumpsterEx(Guid dbGuid, bool fTriggerSafetyNet, DateTime failoverTimeUtc, DateTime startTimeUtc, DateTime endTimeUtc, long lastLogGenBeforeActivation, long numLogsLost);

		[OperationContract]
		void DoDumpsterRedeliveryIfRequired(Guid dbGuid);

		[OperationContract]
		void TriggerServerLocatorRestart();

		[OperationContract]
		void TriggerTruncation(Guid dbGuid);
	}
}
