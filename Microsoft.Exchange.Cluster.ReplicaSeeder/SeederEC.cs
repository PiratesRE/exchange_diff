using System;

public enum SeederEC : int
{
	EcSuccess,
	EcError,
	EcInvalidInput,
	EcOOMem,
	EcNotEnoughDisk,
	EcFailAcqRight,
	EcDirDoesNotExist,
	EcLogAlreadyExist,
	EcJtxAlreadyExist,
	EcDBNotFound,
	EcStoreNotOnline,
	EcNoOnlineEdb,
	EcSeedingCancelled,
	EcOverlappedWriteErr,
	EcMdbAlreadyExist,
	JetErrFileIOBeyondEOF = -4001,
	EcTestAborted = 15,
	EcTargetDbFileInUse,
	EcDeviceNotReady,
	EcCommunicationsError
}
