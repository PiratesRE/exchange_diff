using System;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public enum RecoveryActionId
	{
		None,
		RestartService,
		RecycleApplicationPool,
		ServerFailover,
		ForceReboot,
		WatsonDump,
		DatabaseFailover,
		MoveClusterGroup,
		ControlService,
		TakeComponentOffline,
		TakeComponentOnline,
		ResumeCatalog,
		Escalate,
		PutDCInMM,
		PutMultipleDCInMM,
		CheckDCInMMEscalate,
		KillProcess,
		RenameNTDSPowerOff,
		MomtRestart,
		RpcClientAccessRestart,
		RemoteForceReboot,
		RestartFastNode,
		DeleteFile,
		SetNetworkAdapter,
		AddRoute,
		ClusterNodeHammerDown,
		ClearLsassCache,
		Watson,
		CollectAndMerge,
		RaiseFailureItem,
		PotentialOneCopyRemoteServerRestartResponder,
		Any = 9999
	}
}
