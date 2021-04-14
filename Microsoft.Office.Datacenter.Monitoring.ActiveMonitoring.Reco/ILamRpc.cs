using System;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public interface ILamRpc
	{
		RpcGetThrottlingStatisticsImpl.ThrottlingStatistics GetThrottlingStatistics(string serverName, RecoveryActionId actionId, string resourceName, int maxAllowedInOneHour, int maxAllowedInOneDay, bool isStopSearchWhenLimitExceeds, bool isCountFailedActions, int timeoutInMsec = 30000);

		void UpdateRecoveryActionEntry(string serverName, RecoveryActionEntry entry, int timeoutInMsec = 30000);

		RpcSetThrottlingInProgressImpl.Reply SetThrottlingInProgress(string serverName, RpcSetThrottlingInProgressImpl.ThrottlingProgressInfo progressInfo, bool isClear, bool isForce = false, int timeoutInMsec = 30000);

		void SetWorkerProcessInfo(string serverName, RpcSetWorkerProcessInfoImpl.WorkerProcessInfo info, int timeoutInMsec = 30000);
	}
}
