using System;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	internal enum AmRpcOperationHint
	{
		Gsfd,
		Mount,
		Remount,
		Dismount,
		MoveEx,
		GetPAM,
		AcllDirect,
		AcllDirect2,
		AcllDirect3,
		MountDirectEx,
		DismountDirect,
		SwitchOver,
		IsRunning,
		GetAmRole,
		ReportSystemEvent,
		CheckThirdPartyListener,
		GetAutomountConsensusState,
		SetAutomountConsensusState,
		MoveAllDatabases,
		MoveEx2,
		AmRefreshConfiguration,
		MoveEx3,
		MoveAllDatabases2,
		MountWithAmFlags,
		ReportServiceKill,
		GetDeferredRecoveryEntries,
		MoveAllDatabases3,
		GenericRpc
	}
}
