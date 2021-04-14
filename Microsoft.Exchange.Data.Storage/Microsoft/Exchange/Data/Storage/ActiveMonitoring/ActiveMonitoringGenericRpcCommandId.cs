using System;

namespace Microsoft.Exchange.Data.Storage.ActiveMonitoring
{
	internal enum ActiveMonitoringGenericRpcCommandId
	{
		Unknown,
		GetLastBugcheckStatus,
		GetRecoveryActionStatus,
		GetMonitorHealthStatus,
		SetServerMonitor,
		RequestObserver,
		CancelObserver,
		ObserverHeartbeat,
		UpdateHealthStatus,
		GetServerComponentStatus,
		GetMonitoringItemIdentity,
		GetMonitoringItemHelp,
		InvokeMonitoringProbe,
		GetRecoveryActionQuotaInfo,
		LogCrimsonEvent,
		LockHealthSetEscalationStateIfRequired,
		SetHealthSetEscalationState,
		GetThrottlingStatistics,
		UpdateRecoveryActionEntry,
		SetThrottlingInProgress,
		SetWorkerProcessInfo,
		GetCrimsonMostRecentResultInfo
	}
}
