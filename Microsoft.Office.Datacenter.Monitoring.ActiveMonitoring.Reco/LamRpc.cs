using System;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public class LamRpc : ILamRpc
	{
		public RpcGetThrottlingStatisticsImpl.ThrottlingStatistics GetThrottlingStatistics(string serverName, RecoveryActionId actionId, string resourceName, int maxAllowedInOneHour, int maxAllowedInOneDay, bool isStopSearchWhenLimitExceeds, bool isCountFailedActions, int timeoutInMsec)
		{
			return RpcGetThrottlingStatisticsImpl.SendRequest(serverName, actionId, resourceName, maxAllowedInOneHour, maxAllowedInOneDay, isStopSearchWhenLimitExceeds, isCountFailedActions, 30000);
		}

		public void UpdateRecoveryActionEntry(string serverName, RecoveryActionEntry entry, int timeoutInMsec)
		{
			RpcUpdateRecoveryActionEntryImpl.SendRequest(serverName, entry, timeoutInMsec);
		}

		public RpcSetThrottlingInProgressImpl.Reply SetThrottlingInProgress(string serverName, RpcSetThrottlingInProgressImpl.ThrottlingProgressInfo progressInfo, bool isClear, bool isForce, int timeoutInMsec)
		{
			return RpcSetThrottlingInProgressImpl.SendRequest(serverName, progressInfo, isClear, isForce, timeoutInMsec);
		}

		public void SetWorkerProcessInfo(string serverName, RpcSetWorkerProcessInfoImpl.WorkerProcessInfo info, int timeoutInMsec = 30000)
		{
			RpcSetWorkerProcessInfoImpl.SendRequest(serverName, info, timeoutInMsec);
		}
	}
}
